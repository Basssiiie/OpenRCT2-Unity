using System.Collections.Generic;
using OpenRCT2.Bindings;
using OpenRCT2.Bindings.Graphics;
using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Generators.MeshBuilding;
using OpenRCT2.Generators.Sprites;
using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map.Retro
{
    /// <summary>
    /// A generator that generates the surface of a map.
    /// </summary>
    public class SurfaceGenerator : ITileElementGenerator
    {
        // For now we only use these two water sprites.
        static readonly uint _waterImageIndex = GraphicsDataFactory.GetWaterImageIndex();
        const uint _waterRefractionImageIndex = 5053;
        const byte _noWater = 0;

        readonly int _chunkSize;
        readonly Shader? _surfaceShader;
        readonly string _surfaceTextureField;
        readonly Shader? _edgeShader;
        readonly string _edgeTextureField;
        readonly Shader? _waterShader;
        readonly string _waterTextureField;
        readonly string _waterRefractionField;

        List<RequestedImage>? _images;
        TileCache[,]? _tiles;


        public SurfaceGenerator(int chunkSize, Shader? surfaceShader, string surfaceTextureField, Shader? edgeShader, string edgeTextureField, Shader? waterShader, string waterTextureField, string waterRefractionField)
        {
            _chunkSize = chunkSize;
            _surfaceShader = surfaceShader;
            _surfaceTextureField = surfaceTextureField;
            _edgeShader = edgeShader;
            _edgeTextureField = edgeTextureField;
            _waterShader = waterShader;
            _waterTextureField = waterTextureField;
            _waterRefractionField = waterRefractionField;
            _images = new List<RequestedImage>();
        }



        /// <inheritdoc/>
        public void Start(in MapData map)
        {
            _tiles = new TileCache[map.width, map.height];
        }


        /// <inheritdoc/>
        public void Finish(in MapData map)
        {
            Assert.IsNotNull(_tiles, nameof(_tiles));

            int width = _tiles.GetLength(0);
            int height = _tiles.GetLength(1);
            int chunkWidth = (width - 1) / _chunkSize + 1;
            int chunkHeight = (height - 1) / _chunkSize + 1;

            var builder = new MeshBuilder();
            var chunks = new List<(GameObject, List<int>)>();

            for (int chunkX = 0; chunkX < chunkWidth; chunkX++)
            {
                for (int chunkY = 0; chunkY < chunkHeight; chunkY++)
                {
                    CreateChunk(builder, chunks, map, chunkX, chunkY);
                }
            }

            Material[] materials = GenerateSurfaceMaterials();
            foreach (var (obj, materialIndices) in chunks)
            {
                // Apply all used materials in this chunk.
                int materialCount = materialIndices.Count;
                Material[] chunkMaterials = new Material[materialCount];

                for (int i = 0; i < materialCount; i++)
                {
                    int matIdx = materialIndices[i];
                    chunkMaterials[i] = materials[matIdx];
                }

                MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
                renderer.sharedMaterials = chunkMaterials;
            }

            _tiles = null;
            _images = null;
        }


        /// <inheritdoc/>
        public void CreateElement(in MapData map, int x, int y, int index, in TileElementInfo element)
        {
            Assert.IsNotNull(_tiles, nameof(_tiles));

            SurfaceInfo surface = Park.GetSurfaceElementAt(x, y, index);
            // TODO: support multiple surface elements on the same tile
            _tiles[x, y] = new TileCache((ushort)index, element.baseHeight, surface);
        }


        /// <summary>
        /// Creates a map chunk for the specified area.
        /// </summary>
        void CreateChunk(MeshBuilder builder, List<(GameObject, List<int>)> chunks, in MapData map, int chunkX, int chunkY)
        {
            builder.Clear();
            var materialIndices = new List<int>();

            int startX = chunkX * _chunkSize;
            int startY = chunkY * _chunkSize;
            int endX = Mathf.Min(startX + _chunkSize, map.width);
            int endY = Mathf.Min(startY + _chunkSize, map.height);

            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    CreateSurfaceElement(builder, materialIndices, x, y, startX, startY);
                }
            }

            Mesh mesh = builder.ToMesh();

            if (mesh.vertexCount <= 0)
            {
                return;
            }

            mesh.name = $"Chunk ({chunkX}, {chunkY})";
            mesh.RecalculateNormals();

            var obj = new GameObject($"SurfaceChunk ({chunkX}, {chunkY})");
            obj.transform.parent = map.transform;
            obj.transform.localPosition = new Vector3(chunkX * _chunkSize, 0, chunkY * _chunkSize);

            MeshFilter filter = obj.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh;

            chunks.Add((obj, materialIndices));
        }


        /// <summary>
        /// Adds triangles for terrain surface and edges to the specified chunk.
        /// </summary>
        void CreateSurfaceElement(MeshBuilder builder, List<int> materialIndices, int x, int y, int chunkOffsetX, int chunkOffsetY)
        {
            Assert.IsNotNull(_tiles, nameof(_tiles));

            TileCache tile = _tiles[x, y];
            if (tile.baseHeight == 0)
            {
                return;
            }

            // Convert to local chunk space.
            int localX = x - chunkOffsetX;
            int localY = y - chunkOffsetY;

            /* Surface coords to Unity:
                 * 
                 *  ^   W N
                 *  |   S E
                 *  y 
                 *    x -->
                 */

            SurfaceInfo surface = tile.surface;
            int surfaceSubmesh = AddMaterialIndex(materialIndices, PushImageIndex(surface.surfaceIndex, TextureType.Surface));

            SurfaceSlope slope = surface.slope;
            int baseHeight = tile.baseHeight;

            int northHeight = GetSurfaceCorner(localX + 1, localY + 1, baseHeight, slope, SurfaceSlope.NorthUp, out Vertex north);
            int eastHeight = GetSurfaceCorner(localX + 1, localY, baseHeight, slope, SurfaceSlope.EastUp, out Vertex east);
            int southHeight = GetSurfaceCorner(localX, localY, baseHeight, slope, SurfaceSlope.SouthUp, out Vertex south);
            int westHeight = GetSurfaceCorner(localX, localY + 1, baseHeight, slope, SurfaceSlope.WestUp, out Vertex west);

            SurfaceSlope rotatedSlope = slope & SurfaceSlope.WestEastValley;
            if (rotatedSlope == 0 || rotatedSlope == SurfaceSlope.WestEastValley)
            {
                // In these cases the quad has to be rotated to show the correct kind of slope
                builder.AddTriangle(west, north, east, surfaceSubmesh);
                builder.AddTriangle(east, south, west, surfaceSubmesh);
            }
            else
            {
                builder.AddQuad(north, east, south, west, surfaceSubmesh);
            }

            // Water
            int waterHeight = surface.waterHeight;
            if (waterHeight != _noWater)
            {
                float waterVertexHeight = waterHeight * World.CoordsZMultiplier;

                var waterNorth = new Vertex(north.position.x, waterVertexHeight, north.position.z, Vector3.up, north.uv);
                var waterEast = new Vertex(east.position.x, waterVertexHeight, east.position.z, Vector3.up, east.uv);
                var waterSouth = new Vertex(south.position.x, waterVertexHeight, south.position.z, Vector3.up, south.uv);
                var waterWest = new Vertex(west.position.x, waterVertexHeight, west.position.z, Vector3.up, west.uv);

                int waterSubmesh = AddMaterialIndex(materialIndices, PushImageIndex(_waterImageIndex, TextureType.Water));
                builder.AddQuad(waterNorth, waterEast, waterSouth, waterWest, waterSubmesh);
            }

            // Edges
            uint edgeImage = surface.edgeIndex;
            // HACK: only add the material stack index when any of the add-edges succeed.
            // Otherwise it will create inconsistency in materials, because some may not be used.
            int materialStackIndex = PushImageIndex(edgeImage, TextureType.Edge);
            int edgeSubmesh = materialIndices.IndexOf(materialStackIndex);
            bool addIndexOnSuccess = false;
            if (edgeSubmesh == -1)
            {
                edgeSubmesh = materialIndices.Count;
                addIndexOnSuccess = true;
            }

            bool anyEdge =
                  TryAddSurfaceEdge(builder, x, y, 0, 1, north, west, northHeight, westHeight, waterHeight, SurfaceSlope.EastUp, SurfaceSlope.SouthUp, edgeSubmesh)  // Edge northwest
                | TryAddSurfaceEdge(builder, x, y, 1, 0, east, north, eastHeight, northHeight, waterHeight, SurfaceSlope.SouthUp, SurfaceSlope.WestUp, edgeSubmesh)  // Edge northeast
                | TryAddSurfaceEdge(builder, x, y, 0, -1, south, east, southHeight, eastHeight, waterHeight, SurfaceSlope.WestUp, SurfaceSlope.NorthUp, edgeSubmesh)  // Edge southeast
                | TryAddSurfaceEdge(builder, x, y, -1, 0, west, south, westHeight, southHeight, waterHeight, SurfaceSlope.NorthUp, SurfaceSlope.EastUp, edgeSubmesh); // Edge southwest

            if (anyEdge && addIndexOnSuccess)
            {
                materialIndices.Add(materialStackIndex);
            }
        }


        /// <summary>
        /// Tries to add an surface edge to the specified offset.
        /// </summary>
        bool TryAddSurfaceEdge(MeshBuilder builder, int x, int y, int offsetX, int offsetY, Vertex leftTop, Vertex rightTop, int leftTopHeight, int rightTopHeight, int waterHeight, SurfaceSlope leftOtherCorner, SurfaceSlope rightOtherCorner, int submesh)
        {
            Assert.IsNotNull(_tiles, nameof(_tiles));

            TileCache otherTile = _tiles[x + offsetX, y + offsetY];
            SurfaceInfo otherSurface = otherTile.surface;

            int baseHeight = otherTile.baseHeight;
            SurfaceSlope otherSlope = otherSurface.slope;

            if (waterHeight != _noWater && otherSurface.waterHeight != waterHeight)
            {
                // Render water edge at different height
                float waterY = waterHeight * World.CoordsZMultiplier;
                leftTop.position.y = waterY;
                rightTop.position.y = waterY;
            }

            int leftBottomHeight = GetSurfaceCornerHeight(baseHeight, otherSlope, leftOtherCorner);
            int rightBottomHeight = GetSurfaceCornerHeight(baseHeight, otherSlope, rightOtherCorner);
            float leftBottomY = leftBottomHeight * World.TileCoordsZMultiplier;
            float rightBottomY = rightBottomHeight * World.TileCoordsZMultiplier;

            if (leftTop.position.y > leftBottomY || rightTop.position.y > rightBottomY)
            {
                const float VsPerHeightUnit = 4f;
                int u = offsetX == 0 ? x : y; // pick u based on direction

                var normal = new Vector3(offsetX, 0, offsetY);
                leftTop.normal = normal;
                leftTop.uv = new Vector2(u + offsetY, leftTopHeight / VsPerHeightUnit);
                rightTop.normal = normal;
                rightTop.uv = new Vector2(u + offsetX, rightTopHeight / VsPerHeightUnit);

                var leftBottom = new Vertex(leftTop.position.x, leftBottomY, leftTop.position.z, normal, u + offsetY, leftBottomHeight / VsPerHeightUnit);
                var rightBottom = new Vertex(rightTop.position.x, rightBottomY, rightTop.position.z, normal, u + offsetX, rightBottomHeight / VsPerHeightUnit);

                builder.AddQuad(leftTop, rightTop, rightBottom, leftBottom, submesh);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Adds a material from the generator-wide material stack, and returns its
        /// local index for use in submeshes.
        /// </summary>
        int AddMaterialIndex(List<int> materialIndices, int materialIndex)
        {
            int position = materialIndices.IndexOf(materialIndex);
            if (position == -1)
            {
                position = materialIndices.Count;
                materialIndices.Add(materialIndex);
            }
            return position; // == submesh index
        }


        /// <summary>
        /// Gets the specified surface corner as a 3D vertex.
        /// </summary>
        int GetSurfaceCorner(int localX, int localY, int startHeight, SurfaceSlope surfaceSlope, SurfaceSlope surfaceCorner, out Vertex vertex)
        {
            int height = GetSurfaceCornerHeight(startHeight, surfaceSlope, surfaceCorner);

            var position = new Vector3(
                localX * World.TileCoordsXYMultiplier,
                height * World.TileCoordsZMultiplier,
                localY * World.TileCoordsXYMultiplier
            );

            vertex = new Vertex(position, Vector3.zero, new Vector2(localX, localY));
            return height;
        }


        /// <summary>
        /// Gets the height of the specified corner.
        /// </summary>
        int GetSurfaceCornerHeight(int height, SurfaceSlope surfaceSlope, SurfaceSlope surfaceCorner)
        {
            short slope = (short)surfaceSlope;
            short corner = (short)surfaceCorner;

            // Lift corner vertex up
            if ((slope & corner) != 0)
                height += World.TileHeightStep;

            if ((slope & (int)SurfaceSlope.DoubleHeight) != 0)
            {
                const int allcornersup = (int)SurfaceSlope.AllCornersUp;

                int corners = slope & allcornersup; // get just the corners
                int opposite = (corner << 2) % allcornersup;

                // Check if all corners except opposite are raised.
                if (corners == (allcornersup & ~opposite))
                    height += World.TileHeightStep;
            }
            return height;
        }


        /// <summary>
        /// Pushes a image index to the materials stack and returns its list index.
        /// </summary>
        int PushImageIndex(uint imageIndex, TextureType type)
        {
            Assert.IsNotNull(_images, nameof(_images));

            int position = _images.FindIndex(i => i.ImageIndex == imageIndex);
            if (position == -1)
            {
                position = _images.Count;
                _images.Add(new RequestedImage(imageIndex, type));
            }
            return position;
        }


        /// <summary>
        /// Generates the required materials for the surface mesh.
        /// </summary>
        Material[] GenerateSurfaceMaterials()
        {
            Assert.IsNotNull(_images, nameof(_images));

            int count = _images.Count;
            Material[] materials = new Material[count];

            for (int i = 0; i < count; i++)
            {
                RequestedImage image = _images[i];

                SpriteTexture sprite = SpriteFactory.ForImageIndex(image.ImageIndex);
                Texture2D texture = sprite.GetTexture(TextureWrapMode.Repeat);
                Material material;

                switch (image.Type)
                {
                    case TextureType.Surface:
                        material = new Material(_surfaceShader);
                        material.SetTexture(_surfaceTextureField, texture);
                        break;

                    case TextureType.Edge:
                        material = new Material(_edgeShader);
                        material.SetTexture(_edgeTextureField, texture);
                        break;

                    case TextureType.Water:
                        material = new Material(_waterShader);
                        material.SetTexture(_waterTextureField, texture);

                        // HACK: injection of the refraction sprite shouldnt be here.
                        SpriteTexture refraction = SpriteFactory.ForImageIndex(_waterRefractionImageIndex);
                        material.SetTexture(_waterRefractionField, refraction.GetTexture(TextureWrapMode.Repeat));
                        break;

                    default:
                        Debug.LogWarning("Could not parse this image type: " + image.Type);
                        continue;
                }

                materials[i] = material;
            }

            return materials;
        }


        enum TextureType : byte
        {
            Surface = 1,
            Edge = 2,
            Water = 3
        }


        /// <summary>
        /// Cached data for a surface position
        /// </summary>
        readonly struct TileCache
        {
            public readonly ushort index;
            public readonly int baseHeight;
            public readonly SurfaceInfo surface;

            public TileCache(ushort index, int baseHeight, SurfaceInfo surface)
            {
                this.index = index;
                this.baseHeight = baseHeight;
                this.surface = surface;
            }
        }


        readonly struct RequestedImage
        {
            public readonly uint ImageIndex;
            public readonly TextureType Type;


            public RequestedImage(uint imageIndex, TextureType type)
            {
                ImageIndex = imageIndex;
                Type = type;
            }


            public override bool Equals(object obj)
                => obj is RequestedImage image && ImageIndex == image.ImageIndex;


            public override int GetHashCode()
                => (int)ImageIndex;
        }
    }
}
