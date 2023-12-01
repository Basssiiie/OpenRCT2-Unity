using Lib;
using UnityEngine;
using Utilities;

#nullable enable

namespace Generation
{
    /// <summary>
    /// A simple generator that spawns the specified prefab for the given tile element.
    /// </summary>
    [CreateAssetMenu(menuName = (MenuPath + "Utilities/" + nameof(PrefabGenerator)))]
    public class PrefabGenerator : TileElementGenerator
    {
        [SerializeField, Required] GameObject? _prefab;


        /// <inheritdoc/>
        public override void CreateElement(Map map, int x, int y, int index, in TileElementInfo tile)
        {
            Assert.IsNotNull(_prefab, nameof(_prefab));

            Vector3 position = Map.TileCoordsToUnity(x, y, tile.baseHeight);
            Quaternion rotation = Quaternion.Euler(0, 90 * tile.rotation + 90, 0);

            GameObject.Instantiate(_prefab, position, rotation, map.transform);
        }
    }
}
