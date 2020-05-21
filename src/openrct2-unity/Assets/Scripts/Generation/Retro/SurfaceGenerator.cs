using System;
using System.Collections.Generic;
using MeshBuilding;
using Lib;
using UnityEngine;

namespace Generation.Retro
{
    /// <summary>
    /// A generator that generates the surface of a map.
    /// </summary>
    public partial class SurfaceGenerator : IElementGenerator
    {
        // For now we only use these two water sprites.
        static readonly uint WaterImageIndex = OpenRCT2.GetWaterImageIndex();
        static readonly uint WaterRefractionImageIndex = 5053;

        const byte NoWater = 0;

        Map map;
        MeshBuilder surfaceMeshBuilder;


        /// <inheritdoc/>
        public void StartGenerator(Map map)
        {
            this.map = map;
            surfaceMeshBuilder = new MeshBuilder();
            images = new List<RequestedImage>();
        }


        /// <inheritdoc/>
        public void FinishGenerator()
        {
            Mesh mesh = surfaceMeshBuilder.ToMesh();
            mesh.name = "Map";
            mesh.RecalculateNormals();

            map.Mesh = mesh;
            map.Materials = GenerateSurfaceMaterials();

            surfaceMeshBuilder = null;
            images = null;
        }


        /// <inheritdoc/>
        public void CreateElement(int x, int y, ref TileElement tile)
        {
            /* Surface coords to Unity:
                 * 
                 *  ^   W N
                 *  |   S E
                 *  y 
                 *    x -->
                 */
            SurfaceElement surface = tile.AsSurface();
            SurfaceSlope slope = surface.Slope;
            int baseHeight = tile.baseHeight;

            Vertex north = GetSurfaceCorner(x + 1, y + 1, baseHeight, slope, SurfaceSlope.NorthUp);
            Vertex east = GetSurfaceCorner(x + 1, y, baseHeight, slope, SurfaceSlope.EastUp);
            Vertex south = GetSurfaceCorner(x, y, baseHeight, slope, SurfaceSlope.SouthUp);
            Vertex west = GetSurfaceCorner(x, y + 1, baseHeight, slope, SurfaceSlope.WestUp);

            uint surfaceImage = OpenRCT2.GetSurfaceImageIndex(tile, x, y, 0);
            int surfaceSubmesh = PushImageIndex(surfaceImage, TypeSurface);

            SurfaceSlope rotatedSlope = (slope & SurfaceSlope.WestEastValley);
            if (rotatedSlope == 0 || rotatedSlope == SurfaceSlope.WestEastValley)
            {
                // In these cases the quad has to be rotated to show the correct kind of slope
                surfaceMeshBuilder.AddTriangle(west, north, east, surfaceSubmesh);
                surfaceMeshBuilder.AddTriangle(east, south, west, surfaceSubmesh);
            }
            else
            {
                surfaceMeshBuilder.AddQuad(north, east, south, west, surfaceSubmesh);
            }

            // Water
            int waterHeight = surface.WaterHeight;
            if (waterHeight != NoWater)
            {
                float waterVertexHeight = (waterHeight * Map.TileHeightMultiplier * Map.TileHeightStep);

                Vertex waterNorth = new Vertex(north.position.x, waterVertexHeight, north.position.z, Vector3.up, north.uv);
                Vertex waterEast = new Vertex(east.position.x, waterVertexHeight, east.position.z, Vector3.up, east.uv);
                Vertex waterSouth = new Vertex(south.position.x, waterVertexHeight, south.position.z, Vector3.up, south.uv);
                Vertex waterWest = new Vertex(west.position.x, waterVertexHeight, west.position.z, Vector3.up, west.uv);

                int waterSubmesh = PushImageIndex(WaterImageIndex, TypeWater);
                surfaceMeshBuilder.AddQuad(waterNorth, waterEast, waterSouth, waterWest, waterSubmesh);
            }

            // Edges
            uint edgeImage = OpenRCT2.GetSurfaceEdgeImageIndex(tile);
            int edgeSubmesh = PushImageIndex(edgeImage, TypeEdge);

            TryAddSurfaceEdge(surfaceMeshBuilder, north, west, x, y, 0, 1, waterHeight, SurfaceSlope.EastUp, SurfaceSlope.SouthUp, edgeSubmesh); // Edge northwest
            TryAddSurfaceEdge(surfaceMeshBuilder, east, north, x, y, 1, 0, waterHeight, SurfaceSlope.SouthUp, SurfaceSlope.WestUp, edgeSubmesh); // Edge northeast
            TryAddSurfaceEdge(surfaceMeshBuilder, south, east, x, y, 0, -1, waterHeight, SurfaceSlope.WestUp, SurfaceSlope.NorthUp, edgeSubmesh); // Edge southeast
            TryAddSurfaceEdge(surfaceMeshBuilder, west, south, x, y, -1, 0, waterHeight, SurfaceSlope.NorthUp, SurfaceSlope.EastUp, edgeSubmesh); // Edge southwest
        }


        /// <summary>
        /// Tries to add an surface edge to the specified offset.
        /// </summary>
        void TryAddSurfaceEdge(MeshBuilder builder, Vertex leftTop, Vertex rightTop, int x, int y, int offsetX, int offsetY, int waterHeight, SurfaceSlope leftOtherCorner, SurfaceSlope rightOtherCorner, int submesh)
        {
            SurfaceElement other = map.Tiles[x + offsetX, y + offsetY].Surface;
            int baseHeight = other.BaseHeight;
            SurfaceSlope otherSlope = other.Slope;

            if (waterHeight != NoWater && other.WaterHeight != waterHeight)
            {
                // Render water edge at different height
                float waterY = (waterHeight * Map.TileHeightMultiplier * Map.TileHeightStep);
                leftTop.position.y = waterY;
                rightTop.position.y = waterY;
            }

            float leftBottomY = GetSurfaceCornerHeight(baseHeight, otherSlope, leftOtherCorner) * Map.TileHeightMultiplier;
            float rightBottomY = GetSurfaceCornerHeight(baseHeight, otherSlope, rightOtherCorner) * Map.TileHeightMultiplier;

            if (leftTop.position.y > leftBottomY || rightTop.position.y > rightBottomY)
            {
                int u = (offsetX == 0) ? x : y; // pick u based on direction

                Vector3 normal = new Vector3(offsetX, 0, offsetY);
                leftTop.normal = normal;
                leftTop.uv = new Vector2(u + offsetY, leftTop.position.y);
                rightTop.normal = normal;
                rightTop.uv = new Vector2(u + offsetX, rightTop.position.y);

                Vertex leftBottom = new Vertex(leftTop.position.x, leftBottomY, leftTop.position.z, normal, u + offsetY, leftBottomY);
                Vertex rightBottom = new Vertex(rightTop.position.x, rightBottomY, rightTop.position.z, normal, u + offsetX, rightBottomY);

                builder.AddQuad(leftTop, rightTop, rightBottom, leftBottom, submesh);
            }
        }


        /// <summary>
        /// Gets the specified surface corner as a 3D vertex.
        /// </summary>
        Vertex GetSurfaceCorner(int x, int y, int startHeight, SurfaceSlope surfaceSlope, SurfaceSlope surfaceCorner)
        {
            int height = GetSurfaceCornerHeight(startHeight, surfaceSlope, surfaceCorner);

            Vector3 position = new Vector3(
                x * Map.TileCoordsToVector3Multiplier,
                height * Map.TileHeightMultiplier,
                y * Map.TileCoordsToVector3Multiplier
            );

            return new Vertex(position, Vector3.zero, new Vector2(x, y));
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

    }
}
