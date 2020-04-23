using MeshBuilding;
using UnityEngine;

namespace OpenRCT2.Unity
{
    public partial class Map
    {
        [SerializeField] GameObject pathPrefab;
        [SerializeField] GameObject trackPrefab;
        [SerializeField] GameObject smallSceneryPrefab;
        [SerializeField] GameObject entrancePrefab;
        [SerializeField] GameObject wallPrefab;
        [SerializeField] GameObject largeSceneryPrefab;
        [SerializeField] GameObject bannerPrefab;

        [SerializeField] private bool generatePath = true;
        [SerializeField] private bool generateTrack = true;
        [SerializeField] private bool generateSmallScenery = true;
        [SerializeField] private bool generateEntrance = true;
        [SerializeField] private bool generateWall = true;
        [SerializeField] private bool generateLargeScenery = true;
        [SerializeField] private bool generateBanner = true;


        const int TileCoordsToCoords = 32;
        const float CoordsToVector3Multiplier = 1f / TileCoordsToCoords;
        const float TileCoordsToVector3Multiplier = CoordsToVector3Multiplier * TileCoordsToCoords;

        const float TileHeightMultiplier = 0.25f;

        const int TileHeightStep = 2;



        MeshBuilder cachedBuilder;


        Mesh GenerateSurfaceMesh()
        {
            ResetSurfaceMaterials();

            if (cachedBuilder == null)
                cachedBuilder = new MeshBuilder();
            else
                cachedBuilder.Clear();

            int end = mapSize - 1;
            for (int x = 1; x < end; x++)
            {
                for (int y = 1; y < end; y++)
                {
                    Tile tile = tiles[x, y];
                    for (int e = 0; e < tile.Count; e++)
                    {
                        GenerateTileElement(cachedBuilder, ref tile.Elements[e], x, y);
                    }
                }
            }

            return cachedBuilder.Build();
        }


        /// <summary>
        /// Generates a tile element based on the type of the given tile.
        /// </summary>
        void GenerateTileElement(MeshBuilder builder, ref TileElement tile, int x, int y)
        {
            switch (tile.Type)
            {
                case TileElementType.Surface:
                    GenerateSurface(builder, ref tile, x, y);
                    break;

                case TileElementType.Path:
                    if (generatePath)
                    {
                        InstantiateElement(pathPrefab, x, tile.baseHeight, y);
                    }
                    break;

                case TileElementType.Track:
                    if (generateTrack)
                    {
                        InstantiateElement(trackPrefab, x, tile.baseHeight, y);
                    }
                    break;

                case TileElementType.SmallScenery:
                    if (generateSmallScenery)
                    {
                        GameObject scenery = InstantiateElement(smallSceneryPrefab, x, tile.baseHeight, y);

                        Vector3 scale = scenery.transform.localScale;
                        scale.y = Mathf.Max((tile.clearanceHeight - tile.baseHeight) * TileHeightMultiplier, 1);
                        scenery.transform.localScale = scale;
                    }
                    break;

                case TileElementType.Entrance:
                    if (generateEntrance)
                    {
                        InstantiateElement(entrancePrefab, x, tile.baseHeight, y);
                    }
                    break;

                case TileElementType.Wall:
                    if (generateWall)
                    {
                        InstantiateElement(wallPrefab, x, tile.baseHeight, y);
                    }
                    break;

                case TileElementType.LargeScenery:
                    if (generateLargeScenery)
                    {
                        InstantiateElement(largeSceneryPrefab, x, tile.baseHeight, y);
                    }
                    break;

                case TileElementType.Banner:
                    if (generateBanner)
                    {
                        InstantiateElement(bannerPrefab, x, tile.baseHeight, y);
                    }
                    break;
            }
        }


        /// <summary>
        /// Instantiates a prefab in the place of a tile element.
        /// </summary>
        GameObject InstantiateElement(GameObject obj, float x, float y, float z)
        {
            Vector3 position = TileCoordsToUnity(x, y, z);
            return Instantiate(obj, position, Quaternion.identity, transform);
        }
    }
}
