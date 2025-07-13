using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OpenRCT2.Bindings;
using OpenRCT2.Bindings.Graphics;
using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Generators.Extensions;
using OpenRCT2.Generators.MeshBuilding;
using OpenRCT2.Generators.Sprites;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map
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
        }

        /// <inheritdoc/>
        public IEnumerator<LoadStatus> Run(Map map, Transform transform)
        {
            var images = new List<RequestedImage>();

            int width = map.width;
            int height = map.height;
            int chunkWidth = (width - 1) / _chunkSize + 1;
            int chunkHeight = (height - 1) / _chunkSize + 1;
            int chunkCount = chunkWidth * chunkHeight;
            int counter = 0;

            var builder = new MeshBuilder();
            var chunks = new List<(GameObject, List<int>)>();

            for (int chunkX = 0; chunkX < chunkWidth; chunkX++)
            {
                for (int chunkY = 0; chunkY < chunkHeight; chunkY++)
                {
                    yield return new LoadStatus("Generating chunks...", counter++, chunkCount);

                    CreateChunk(builder, map, transform, chunks, chunkX, chunkY, images);
                }
            }

            Material[] materials = GenerateSurfaceMaterials(images);
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
                renderer.staticShadowCaster = true;
                renderer.sharedMaterials = chunkMaterials;
            }
        }

        /// <summary>
        /// Creates a map chunk for the specified area.
        /// </summary>
        void CreateChunk(MeshBuilder builder, Map map, Transform transform, List<(GameObject, List<int>)> chunks, int chunkX, int chunkY, List<RequestedImage> images)
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
                    CreateSurfaceElement(builder, map, materialIndices, x, y, startX, startY, images);
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
            obj.isStatic = true;
            obj.transform.parent = transform;
            obj.transform.localPosition = new Vector3(chunkX * _chunkSize + 1, 0, chunkY * _chunkSize + 1); // add 1 to account for missing border

            MeshFilter filter = obj.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh;

            chunks.Add((obj, materialIndices));
        }

        /// <summary>
        /// Adds triangles for terrain surface and edges to the specified chunk.
        /// </summary>
        void CreateSurfaceElement(MeshBuilder builder, Map map, List<int> materialIndices, int x, int y, int chunkOffsetX, int chunkOffsetY, List<RequestedImage> images)
        {
            // todo: support multiple surface elements on a single tile
            var tile = map.tiles[x, y];

            if (!tile.TryGetFirst(TileElementType.Surface, out var element)
                || element.baseHeight == 0)
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

            SurfaceInfo surface = tile.surfaces[0];
            SurfaceSlope slope = surface.slope;
            int baseHeight = element.baseHeight;
            Vertex north = GetSurfaceCorner(localX + 1, localY + 1, baseHeight, slope, SurfaceSlope.NorthUp);
            Vertex east = GetSurfaceCorner(localX + 1, localY, baseHeight, slope, SurfaceSlope.EastUp);
            Vertex south = GetSurfaceCorner(localX, localY, baseHeight, slope, SurfaceSlope.SouthUp);
            Vertex west = GetSurfaceCorner(localX, localY + 1, baseHeight, slope, SurfaceSlope.WestUp);
            int waterHeight = surface.waterHeight;

            // If vertical tunnel on flat sloped terrain, skip surface tile.
            if (slope != SurfaceSlope.Flat || !IsVerticalTunnel(tile, baseHeight))
            {
                int surfaceSubmesh = AddMaterialIndex(materialIndices, PushImageIndex(images, surface.surfaceImageIndex, TextureType.Surface));

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
                if (waterHeight != _noWater)
                {
                    float waterVertexHeight = waterHeight * World.CoordsZMultiplier;

                    var waterNorth = new Vertex(north.position.x, waterVertexHeight, north.position.z, Vector3.up, north.uv);
                    var waterEast = new Vertex(east.position.x, waterVertexHeight, east.position.z, Vector3.up, east.uv);
                    var waterSouth = new Vertex(south.position.x, waterVertexHeight, south.position.z, Vector3.up, south.uv);
                    var waterWest = new Vertex(west.position.x, waterVertexHeight, west.position.z, Vector3.up, west.uv);

                    int waterSubmesh = AddMaterialIndex(materialIndices, PushImageIndex(images, _waterImageIndex, TextureType.Water));
                    builder.AddQuad(waterNorth, waterEast, waterSouth, waterWest, waterSubmesh);
                }
            }            

            // Edges
            uint edgeImage = surface.edgeImageIndex;
            // HACK: only add the material stack index when any of the add-edges succeed.
            // Otherwise it will create inconsistency in materials, because some may not be used.
            int materialStackIndex = PushImageIndex(images, edgeImage, TextureType.Edge);
            int edgeSubmesh = materialIndices.IndexOf(materialStackIndex);
            bool addIndexOnSuccess = false;
            if (edgeSubmesh == -1)
            {
                edgeSubmesh = materialIndices.Count;
                addIndexOnSuccess = true;
            }

            bool anyEdge =
                  TryAddSurfaceEdge(builder, map, tile, x, y, 0, 1, north.position, west.position, waterHeight, SurfaceSlope.EastUp, SurfaceSlope.SouthUp, edgeSubmesh)  // Edge northwest
                | TryAddSurfaceEdge(builder, map, tile, x, y, 1, 0, east.position, north.position, waterHeight, SurfaceSlope.SouthUp, SurfaceSlope.WestUp, edgeSubmesh)  // Edge northeast
                | TryAddSurfaceEdge(builder, map, tile, x, y, 0, -1, south.position, east.position, waterHeight, SurfaceSlope.WestUp, SurfaceSlope.NorthUp, edgeSubmesh)  // Edge southeast
                | TryAddSurfaceEdge(builder, map, tile, x, y, -1, 0, west.position, south.position, waterHeight, SurfaceSlope.NorthUp, SurfaceSlope.EastUp, edgeSubmesh); // Edge southwest

            if (anyEdge && addIndexOnSuccess)
            {
                materialIndices.Add(materialStackIndex);
            }
        }

        /// <summary>
        /// Tries to add an surface edge to the specified offset.
        /// </summary>
        bool TryAddSurfaceEdge(MeshBuilder builder, Map map, Tile tile, int x, int y, int offsetX, int offsetY, Vector3 leftTop, Vector3 rightTop, int waterHeight, SurfaceSlope leftOtherCorner, SurfaceSlope rightOtherCorner, int submesh)
        {
            int baseHeight = 0;
            SurfaceInfo otherSurface = default;

            if (TryGetTile(map, x + offsetX, y + offsetY, out var otherTile)
                && otherTile.TryGetFirst(TileElementType.Surface, out var element))
            {
                baseHeight = element.baseHeight;
                otherSurface = otherTile.surfaces[0];
            }

            SurfaceSlope otherSlope = otherSurface.slope;

            if (waterHeight != _noWater && otherSurface.waterHeight != waterHeight)
            {
                // Render water edge at different height
                float waterY = waterHeight * World.CoordsZMultiplier;
                leftTop.y = waterY;
                rightTop.y = waterY;
            }

            int leftBottomHeight = GetSurfaceCornerHeight(baseHeight, otherSlope, leftOtherCorner);
            int rightBottomHeight = GetSurfaceCornerHeight(baseHeight, otherSlope, rightOtherCorner);
            float leftBottomY = leftBottomHeight * World.TileCoordsZMultiplier;
            float rightBottomY = rightBottomHeight * World.TileCoordsZMultiplier;
            float leftTopY = leftTop.y;
            float rightTopY = rightTop.y;

            if (leftTopY > leftBottomY || rightTopY > rightBottomY)
            {
                float bottom = leftBottomY < rightBottomY ? leftBottomY : rightBottomY;
                float top = leftTopY > rightTopY ? leftTopY : rightTopY;
                int direction = (offsetX + 1 + offsetY + Mathf.Abs(offsetY)+6)%4;
                var tunnels = GetTunnelPoints(tile, direction, bottom, top);

                float leftY = leftBottomY;
                float rightY = rightBottomY;
                int u = offsetX == 0 ? x : y; // pick u based on direction

                foreach (var (tunnelBottom, tunnelTop) in tunnels)
                {
                    if (leftY < tunnelBottom || rightY < tunnelBottom)
                    {
                        CreateEdgeQuad(builder, leftTop.x, rightTop.x, leftY, rightY, tunnelBottom, tunnelBottom, leftTop.z, rightTop.z, offsetX, offsetY, u, submesh);
                    }
                    leftY = tunnelTop;
                    rightY = tunnelTop;
                }

                if (leftY < leftTopY || rightY < rightTopY)
                {
                    CreateEdgeQuad(builder, leftTop.x, rightTop.x, leftY, rightY, leftTopY, rightTopY, leftTop.z, rightTop.z, offsetX, offsetY, u, submesh);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Creates a quad for a terrain edge with the specified positions.
        /// </summary>
        void CreateEdgeQuad(MeshBuilder builder, float leftX, float rightX, float leftBottomY, float rightBottomY, float leftTopY, float rightTopY, float leftZ, float rightZ, int offsetX, int offsetY, float u, int submesh)
        {
            var normal = new Vector3(offsetX, 0, offsetY);
            var leftTop = new Vertex(leftX, leftTopY, leftZ, normal, u + offsetY, leftTopY);
            var rightTop = new Vertex(rightX, rightTopY, rightZ, normal, u + offsetX, rightTopY);
            var leftBottom = new Vertex(leftX, leftBottomY, leftZ, normal, u + offsetY, leftBottomY);
            var rightBottom = new Vertex(rightX, rightBottomY, rightZ, normal, u + offsetX, rightBottomY);

            builder.AddQuad(leftTop, rightTop, rightBottom, leftBottom, submesh);
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
        Vertex GetSurfaceCorner(int localX, int localY, int startHeight, SurfaceSlope surfaceSlope, SurfaceSlope surfaceCorner)
        {
            int height = GetSurfaceCornerHeight(startHeight, surfaceSlope, surfaceCorner);

            var position = new Vector3(
                localX * World.TileCoordsXYMultiplier,
                height * World.TileCoordsZMultiplier,
                localY * World.TileCoordsXYMultiplier
            );

            return new Vertex(position, Vector3.zero, new Vector2(localX, localY));
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
        int PushImageIndex(List<RequestedImage> images, uint imageIndex, TextureType type)
        {
            int position = images.FindIndex(i => i.ImageIndex == imageIndex);
            if (position == -1)
            {
                position = images.Count;
                images.Add(new RequestedImage(imageIndex, type));
            }
            return position;
        }

        /// <summary>
        /// Generates the required materials for the surface mesh.
        /// </summary>
        Material[] GenerateSurfaceMaterials(List<RequestedImage> images)
        {
            int count = images.Count;
            Material[] materials = new Material[count];

            for (int i = 0; i < count; i++)
            {
                RequestedImage image = images[i];

                SpriteTexture sprite = SpriteFactory.GetOrCreate(image.ImageIndex);
                Texture2D texture = TextureFactory.CreateFullColour(sprite, TextureWrapMode.Repeat);
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
                        SpriteTexture refraction = SpriteFactory.GetOrCreate(_waterRefractionImageIndex);
                        material.SetTexture(_waterRefractionField, TextureFactory.CreateFullColour(refraction, TextureWrapMode.Repeat));
                        break;

                    default:
                        Debug.LogWarning("Could not parse this image type: " + image.Type);
                        continue;
                }

                materials[i] = material;
            }

            return materials;
        }

        bool TryGetTile(Map map, int x, int y, [MaybeNullWhen(false)] out Tile tile)
        {
            if (x < 0 || y < 0 || x >= map.width || y >= map.height)
            {
                tile = null;
                return false;
            }

            tile = map.tiles[x, y];
            return true;
        }

        List<(float bottom, float top)> GetTunnelPoints(Tile tile, int direction, float start, float end)
        {
            TileElementInfo[] elements = tile.elements;
            int length = elements.Length;
            int pathIdx = 0; //, trackIdx = 0, entranceIdx = 0;
            var tunnels = new List<(float height, float top)>();

            void AddTunnelPoint(int bottom, int top)
            {
                float bottomZ = bottom * World.TileCoordsZMultiplier;
                float topZ = top * World.TileCoordsZMultiplier;
                if (start < topZ && bottomZ < end)
                {
                    tunnels.Add((bottomZ, topZ));
                }
            }

            for (var idx = 0; idx < length; idx++)
            {
                ref var element = ref elements[idx];

                switch (element.type)
                {
                    case TileElementType.Path:
                    {
                        ref var path = ref tile.paths[pathIdx];
                        int side = (1 << direction);
                        if ((path.edges & side) != 0)
                        {
                            byte height = element.baseHeight;
                            if (path.sloped && path.slopeDirection == direction)
                            {
                                height += 2;
                            }
                            AddTunnelPoint(height, height + 4);
                        }
                        pathIdx++;
                        break;
                    }
                    case TileElementType.Track:
                    //{ 
                    //    ref var track = ref tile.tracks[pathIdx];
                    //    trackIdx++;
                    //    break;
                    //}
                    case TileElementType.Entrance:
                    { 
                        //ref var entrance = ref tile.entrances[entranceIdx];
                        //entranceIdx++;
                        byte height = element.baseHeight;
                        byte rotation = element.rotation;
                        if (rotation == direction || rotation == (direction + 2) % 4)
                        {
                            AddTunnelPoint(height, height + 4);
                        }
                        break;
                    }
                }
            }

            tunnels.Sort((a, b) => a.height.CompareTo(b.height));
            return tunnels;
        }

        bool IsVerticalTunnel(Tile tile, int height)
        {
            if (tile.tracks.Length == 0)
            {
                return false;
            }

            var elements = tile.elements;
            var length = elements.Length;
            var trackIdx = 0;

            for (var idx = 0; idx < length; idx++)
            {
                ref var element = ref elements[idx];
                if (element.type != TileElementType.Track)
                {
                    continue;
                }
                if (element.baseHeight == height)
                {
                    ref var track = ref tile.tracks[trackIdx];
                    switch (track.trackType)
                    {
                        case 67 when track.sequenceIndex is 0: // tower piece
                        case 125 when track.sequenceIndex is 1: // reverse freefall upwards track
                        case 126: // upward vertical track
                        case 127: // downward vertical track
                        case 130: // upward steep to vertical track
                        case 131: // downward steep to vertical track
                        case 206 when track.sequenceIndex is 2: // multi-dim inverted to downwards vertical quarter loop
                        case 207 when track.sequenceIndex is 0: // upward quarter loop
                        case 208 when track.sequenceIndex is 2: // downwards quarter loop
                        case 213 when track.sequenceIndex is 1 or 2: // air-powered tophat
                        case 214 when track.sequenceIndex is 0: // air-powered downwards track
                        case 249: // vertical upward left turn
                        case 250: // vertical upward right turn
                        case 251: // vertical downward left turn
                        case 252: // vertical downward right turn
                        case 253 when track.sequenceIndex is 0: // multi-dim upwards vertical to inverted quarter loop
                        case 254 when track.sequenceIndex is 2: // multi-dim flat to downwards vertical quarter loop
                        case 255 when track.sequenceIndex is 0: // multi-dim upwards vertical to flat quarter loop
                            return true;
                    }
                }
                trackIdx++;
            }
            return false;
        }

        enum TextureType : byte
        {
            Surface = 1,
            Edge = 2,
            Water = 3
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
