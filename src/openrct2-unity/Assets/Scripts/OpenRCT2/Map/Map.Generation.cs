using MeshBuilding;
using UnityEngine;

namespace OpenRCT2.Unity
{
    public partial class Map
    {
        [SerializeField] GameObject pathPrefab;
        [SerializeField] GameObject trackPrefab;
        [SerializeField] GameObject smallSceneryPrefab;


        const int TileCoordsToCoords = 32;
        const float CoordsToVector3Multiplier = 1f / TileCoordsToCoords;
        const float TileCoordsToVector3Multiplier = CoordsToVector3Multiplier * TileCoordsToCoords;

        const float TileHeightMultiplier = 0.25f;
        const int TileHeightStep = 2;

        const float PixelPerUnitMultiplier = 0.025f;


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
                    InstantiateElement(pathPrefab, x, tile.baseHeight, y);
                    break;

                case TileElementType.Track:
                    InstantiateElement(trackPrefab, x, tile.baseHeight, y);
                    break;

                case TileElementType.SmallScenery:
                    InstantiateSmallScenery(ref tile, smallSceneryPrefab, x, tile.baseHeight, y);
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
            GameObject obj = InstantiateElement(prefab, x, y, z);
            Texture2D texture = TextureFactory.ForTileElement(ref tile);

            float width = (texture.width * PixelPerUnitMultiplier);
            obj.transform.localScale = new Vector3(width, texture.height * PixelPerUnitMultiplier, width);

            foreach (var renderer in obj.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.material.SetTexture("_BaseMap", texture);
            }
            return obj;
        }
    }
}
