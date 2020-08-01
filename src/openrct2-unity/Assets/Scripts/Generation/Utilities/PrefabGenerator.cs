using Lib;
using UnityEngine;

namespace Generation
{
    /// <summary>
    /// A simple generator that spawns the specified prefab for the given tile element.
    /// </summary>
    [CreateAssetMenu(menuName = (MenuPath + "Utilities/" + nameof(PrefabGenerator)))]
    public class PrefabGenerator : TileElementGenerator
    {
        [SerializeField] GameObject prefab;


        /// <inheritdoc/>
        public override void CreateElement(int x, int y, in TileElement tile)
        {
            Vector3 position = Map.TileCoordsToUnity(x, tile.baseHeight, y);
            Quaternion rotation = Quaternion.Euler(0, 90 * tile.Rotation + 90, 0);

            GameObject.Instantiate(prefab, position, rotation, map.transform);
        }
    }
}
