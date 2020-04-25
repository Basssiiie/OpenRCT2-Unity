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

        [SerializeField] bool generatePath = true;
        [SerializeField] bool generateTrack = true;
        [SerializeField] bool generateSmallScenery = true;
        [SerializeField] bool generateEntrance = true;
        [SerializeField] bool generateWall = true;
        [SerializeField] bool generateLargeScenery = true;
        [SerializeField] bool generateBanner = true;


        const int TileCoordsToCoords = 32;
        const float TileCoordsToVector3Multiplier = 1f;
        const float CoordsToVector3Multiplier = TileCoordsToVector3Multiplier / TileCoordsToCoords;

        const float TileHeightMultiplier = 0.25f;
        const int TileHeightStep = 2;

        const float PixelPerUnitMultiplier = 0.022f;


        MeshBuilder cachedBuilder;


        /// <summary>
        /// Generates the surface of the map.
        /// </summary>
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

            return cachedBuilder.ToMesh();
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
                        InstantiateSmallScenery(ref tile, smallSceneryPrefab, x, tile.baseHeight, y);
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
        GameObject InstantiateElement(GameObject prefab, float x, float y, float z)
        {
            Vector3 position = TileCoordsToUnity(x, y, z);
            return Instantiate(prefab, position, Quaternion.identity, transform);
        }


        /// <summary>
        /// Instantiates a small scenery prefab in the place of a tile element.
        /// </summary>
        GameObject InstantiateSmallScenery(ref TileElement tile, GameObject prefab, float x, float y, float z)
        {
            SmallSceneryElement smallScenery = tile.AsSmallScenery();

            SmallSceneryEntry entry = OpenRCT2.GetSmallSceneryEntry(smallScenery.EntryIndex);
            SmallSceneryFlags flags = entry.Flags; 

            // If not a full tile, move small scenery to the correct quadrant.
            if ((flags & SmallSceneryFlags.FullTile) == 0)
            {
                const float distanceToQuadrant = (TileCoordsToVector3Multiplier / 4);
                byte quadrant = smallScenery.Quadrant;

                switch (quadrant)
                {
                    case 0: x -= distanceToQuadrant; z -= distanceToQuadrant; break;
                    case 1: x -= distanceToQuadrant; z += distanceToQuadrant; break;
                    case 2: x += distanceToQuadrant; z += distanceToQuadrant; break;
                    case 3: x += distanceToQuadrant; z -= distanceToQuadrant; break;
                }
            }

            // Instantiate the element.
            GameObject obj = InstantiateElement(prefab, x, y, z);
            Texture2D texture = GraphicsFactory.ForTileElement(ref tile).ToTexture2D();

            foreach (MeshRenderer renderer in obj.GetComponentsInChildren<MeshRenderer>())
                renderer.material.SetTexture("_BaseMap", texture);

            // Set the visual scale of the model.
            float width = (texture.width * PixelPerUnitMultiplier);
            obj.transform.localScale = new Vector3(width, texture.height * PixelPerUnitMultiplier, width);
            return obj;
        }
    }
}
