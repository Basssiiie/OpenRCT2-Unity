using OpenRCT;
using UnityEngine;

namespace Generation
{
    /// <summary>
    /// A simple generator that spawns the specified prefab for the given tile element.
    /// </summary>
    public class PrefabGenerator : IElementGenerator
    {
        [SerializeField] GameObject prefab;


        Map map;


        /// <inheritdoc/>
        public void StartGenerator(Map map)
        {
            this.map = map;
        }


        /// <inheritdoc/>
        public void FinishGenerator()
        {
            map = null;
        }


        /// <inheritdoc/>
        public void CreateElement(int x, int y, ref TileElement tile)
        {
            Vector3 position = Map.TileCoordsToUnity(x, tile.baseHeight, y);
            Quaternion rotation = Quaternion.Euler(0, 90 * tile.Rotation + 90, 0);

            GameObject.Instantiate(prefab, position, rotation, map.transform);
        }
    }
}
