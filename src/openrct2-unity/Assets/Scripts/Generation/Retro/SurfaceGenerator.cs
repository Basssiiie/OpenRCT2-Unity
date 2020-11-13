using System.Collections.Generic;
using Lib;
using MeshBuilding;
using UnityEngine;

namespace Generation.Retro
{
    /// <summary>
    /// A generator that generates the surface of a map.
    /// </summary>
    [CreateAssetMenu(menuName = (MenuPath + "Retro/" + nameof(SurfaceGenerator)))]
    public partial class SurfaceGenerator : TileElementGenerator
    {
        // For now we only use these two water sprites.
        static readonly uint _waterImageIndex = OpenRCT2.GetWaterImageIndex();
        const uint WaterRefractionImageIndex = 5053;

        [SerializeField] int _chunkSize = 64;

        const byte NoWater = 0;

        List<Chunk> _chunks;


        class Chunk
        {
            public int x;
            public int y;

            public MeshBuilder builder = new MeshBuilder();
            public List<int> materialIndices = new List<int>();


            /// <summary>
            /// Adds a material from the generator-wide material stack, and returns its
            /// local index for use in submeshes.
            /// </summary>
            public int AddMaterialIndex(int materialIndex)
            {
                int position = materialIndices.IndexOf(materialIndex);
                if (position == -1)
                {
                    position = materialIndices.Count;
                    materialIndices.Add(materialIndex);
                }
                return position; // == submesh index
            }
        }


        /// <inheritdoc/>
        protected override void Start()
        {
            _chunks = new List<Chunk>();
            _images = new List<RequestedImage>();
        }


        /// <inheritdoc/>
        protected override void Finish()
        {
            Material[] materials = GenerateSurfaceMaterials();

            foreach (Chunk chunk in _chunks)
            {
                Mesh mesh = chunk.builder.ToMesh();

                if (mesh.vertexCount == 0)
                    continue;

                mesh.name = $"Chunk ({chunk.x}, {chunk.y})";
                mesh.RecalculateNormals();

                GameObject chunkObj = new GameObject($"Mapchunk ({chunk.x}, {chunk.y})");
                chunkObj.transform.parent = _map.transform;
                chunkObj.transform.localPosition = new Vector3(chunk.x * _chunkSize, 0, chunk.y * _chunkSize);

                MeshFilter filter = chunkObj.AddComponent<MeshFilter>();
                MeshRenderer renderer = chunkObj.AddComponent<MeshRenderer>();

                filter.sharedMesh = mesh;

                // Apply all used materials in this chunk.
                int materialCount = chunk.materialIndices.Count;
                Material[] chunkMaterials = new Material[materialCount];

                for (int i = 0; i < materialCount; i++)
                {
                    int matIdx = chunk.materialIndices[i];
                    chunkMaterials[i] = materials[matIdx];
                }

                renderer.sharedMaterials = chunkMaterials;
            }

            _chunks = null;
            _images = null;
        }


        /// <inheritdoc/>
        public override void CreateElement(int x, int y, in TileElement tile)
        {
            int chunkX = (x / _chunkSize);
            int chunkY = (y / _chunkSize);

            Chunk chunk = _chunks.Find(c => c.x == chunkX && c.y == chunkY);
            if (chunk == null)
            {
                chunk = new Chunk
                {
                    x = chunkX,
                    y = chunkY,
                };
                _chunks.Add(chunk);
            }

            CreateSurfaceElement(chunk, x, y, tile);
        }


        /// <summary>
        /// Adds triangles for terrain surface and edges to the specified chunk.
        /// </summary>
        void CreateSurfaceElement(Chunk chunk, int x, int y, in TileElement tile)
        {
            // Convert to local chunk space.
            int localX = x - (chunk.x * _chunkSize);
            int localY = y - (chunk.y * _chunkSize);

            /* Surface coords to Unity:
                 * 
                 *  ^   W N
                 *  |   S E
                 *  y 
                 *    x -->
                 */

            uint surfaceImage = OpenRCT2.GetSurfaceImageIndex(tile, x, y, 0);
            if (surfaceImage == uint.MaxValue)
            {
                Debug.LogWarning($"Invalid surface image at ({x}, {y}).");
                return; // empty tile can happen?
            }

            int surfaceSubmesh = chunk.AddMaterialIndex(PushImageIndex(surfaceImage, TextureType.Surface));

            SurfaceElement surface = tile.AsSurface();
            SurfaceSlope slope = surface.Slope;
            int baseHeight = tile.baseHeight;

            int northHeight = GetSurfaceCorner(localX + 1, localY + 1, baseHeight, slope, SurfaceSlope.NorthUp, out Vertex north);
            int eastHeight =  GetSurfaceCorner(localX + 1, localY,     baseHeight, slope, SurfaceSlope.EastUp,  out Vertex east);
            int southHeight = GetSurfaceCorner(localX,     localY,     baseHeight, slope, SurfaceSlope.SouthUp, out Vertex south);
            int westHeight =  GetSurfaceCorner(localX,     localY + 1, baseHeight, slope, SurfaceSlope.WestUp,  out Vertex west);

            SurfaceSlope rotatedSlope = (slope & SurfaceSlope.WestEastValley);
            if (rotatedSlope == 0 || rotatedSlope == SurfaceSlope.WestEastValley)
            {
                // In these cases the quad has to be rotated to show the correct kind of slope
                chunk.builder.AddTriangle(west, north, east, surfaceSubmesh);
                chunk.builder.AddTriangle(east, south, west, surfaceSubmesh);
            }
            else
            {
                chunk.builder.AddQuad(north, east, south, west, surfaceSubmesh);
            }

            // Water
            int waterHeight = surface.WaterHeight;
            if (waterHeight != NoWater)
            {
                float waterVertexHeight = (waterHeight * Map.TileCoordsZMultiplier * Map.TileHeightStep);

                Vertex waterNorth = new Vertex(north.position.x, waterVertexHeight, north.position.z, Vector3.up, north.uv);
                Vertex waterEast =  new Vertex(east.position.x,  waterVertexHeight, east.position.z,  Vector3.up, east.uv);
                Vertex waterSouth = new Vertex(south.position.x, waterVertexHeight, south.position.z, Vector3.up, south.uv);
                Vertex waterWest =  new Vertex(west.position.x,  waterVertexHeight, west.position.z,  Vector3.up, west.uv);

                int waterSubmesh = chunk.AddMaterialIndex(PushImageIndex(_waterImageIndex, TextureType.Water));
                chunk.builder.AddQuad(waterNorth, waterEast, waterSouth, waterWest, waterSubmesh);
            }

            // Edges
            uint edgeImage = OpenRCT2.GetSurfaceEdgeImageIndex(tile);
            if (edgeImage == uint.MaxValue)
            {
                Debug.LogWarning($"Invalid surface edge image at ({x}, {y}).");
                return; // empty tile can happen?
            }

            // HACK: only add the material stack index when any of the add-edges succeed.
            // Otherwise it will create inconsistency in materials, because some may not be used.
            int materialStackIndex = PushImageIndex(edgeImage, TextureType.Edge);
            int edgeSubmesh = chunk.materialIndices.IndexOf(materialStackIndex);
            bool addIndexOnSuccess = false;
            if (edgeSubmesh == -1)
            {
                edgeSubmesh = chunk.materialIndices.Count;
                addIndexOnSuccess = true;
            }

            bool anyEdge = 
                  TryAddSurfaceEdge(chunk, x, y,  0,  1, north, west, northHeight, westHeight, waterHeight, SurfaceSlope.EastUp, SurfaceSlope.SouthUp, edgeSubmesh)  // Edge northwest
                | TryAddSurfaceEdge(chunk, x, y,  1,  0, east, north, eastHeight, northHeight, waterHeight, SurfaceSlope.SouthUp, SurfaceSlope.WestUp, edgeSubmesh)  // Edge northeast
                | TryAddSurfaceEdge(chunk, x, y,  0, -1, south, east, southHeight, eastHeight, waterHeight, SurfaceSlope.WestUp, SurfaceSlope.NorthUp, edgeSubmesh)  // Edge southeast
                | TryAddSurfaceEdge(chunk, x, y, -1,  0, west, south, westHeight, southHeight, waterHeight, SurfaceSlope.NorthUp, SurfaceSlope.EastUp, edgeSubmesh); // Edge southwest

            if (anyEdge && addIndexOnSuccess)
            {
                chunk.materialIndices.Add(materialStackIndex);
            }
        }


        /// <summary>
        /// Tries to add an surface edge to the specified offset.
        /// </summary>
        bool TryAddSurfaceEdge(Chunk chunk, int x, int y, int offsetX, int offsetY, Vertex leftTop, Vertex rightTop, int leftTopHeight, int rightTopHeight, int waterHeight, SurfaceSlope leftOtherCorner, SurfaceSlope rightOtherCorner, int submesh)
        {
            SurfaceElement other = _map.Tiles[x + offsetX, y + offsetY].Surface;

            int baseHeight = other.BaseHeight;
            SurfaceSlope otherSlope = other.Slope;

            if (waterHeight != NoWater && other.WaterHeight != waterHeight)
            {
                // Render water edge at different height
                float waterY = (waterHeight * Map.TileCoordsZMultiplier * Map.TileHeightStep);
                leftTop.position.y = waterY;
                rightTop.position.y = waterY;
            }

            int leftBottomHeight = GetSurfaceCornerHeight(baseHeight, otherSlope, leftOtherCorner);
            int rightBottomHeight = GetSurfaceCornerHeight(baseHeight, otherSlope, rightOtherCorner);
            float leftBottomY = (leftBottomHeight * Map.TileCoordsZMultiplier);
            float rightBottomY = (rightBottomHeight * Map.TileCoordsZMultiplier);

            if (leftTop.position.y > leftBottomY || rightTop.position.y > rightBottomY)
            {
                const float VsPerHeightUnit = 4f;
                int u = (offsetX == 0) ? x : y; // pick u based on direction

                Vector3 normal = new Vector3(offsetX, 0, offsetY);
                leftTop.normal = normal;
                leftTop.uv = new Vector2(u + offsetY, leftTopHeight / VsPerHeightUnit);
                rightTop.normal = normal;
                rightTop.uv = new Vector2(u + offsetX, rightTopHeight / VsPerHeightUnit);

                Vertex leftBottom = new Vertex(leftTop.position.x, leftBottomY, leftTop.position.z, normal, u + offsetY, leftBottomHeight / VsPerHeightUnit);
                Vertex rightBottom = new Vertex(rightTop.position.x, rightBottomY, rightTop.position.z, normal, u + offsetX, rightBottomHeight / VsPerHeightUnit);

                chunk.builder.AddQuad(leftTop, rightTop, rightBottom, leftBottom, submesh);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Gets the specified surface corner as a 3D vertex.
        /// </summary>
        int GetSurfaceCorner(int localX, int localY, int startHeight, SurfaceSlope surfaceSlope, SurfaceSlope surfaceCorner, out Vertex vertex)
        {
            int height = GetSurfaceCornerHeight(startHeight, surfaceSlope, surfaceCorner);

            Vector3 position = new Vector3(
                localX * Map.TileCoordsXYMultiplier,
                height * Map.TileCoordsZMultiplier,
                localY * Map.TileCoordsXYMultiplier
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
                height += Map.TileHeightStep;

            if ((slope & (int)SurfaceSlope.DoubleHeight) != 0)
            {
                const int allcornersup = (int)SurfaceSlope.AllCornersUp;

                int corners = (slope & allcornersup); // get just the corners
                int opposite = (corner << 2) % allcornersup;

                // Check if all corners except opposite are raised.
                if (corners == (allcornersup & ~opposite))
                    height += Map.TileHeightStep;
            }
            return height;
        }


        /// <summary>
        /// Gets a hash for the specified chunk position.
        /// </summary>
        int GetChunkPositionHash(short x, short y)
        {
            return (x << 16) | (int)y;
        }

    }
}
