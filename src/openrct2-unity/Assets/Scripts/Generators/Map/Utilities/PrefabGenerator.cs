using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map.Utilities
{
    /// <summary>
    /// A simple generator that spawns the specified prefab for the given tile element.
    /// </summary>
    public class PrefabGenerator : ITileElementGenerator
    {
        readonly GameObject? _prefab;


        public PrefabGenerator(GameObject? prefab)
        {
            _prefab = prefab;
        }


        /// <inheritdoc/>
        public void CreateElement(in MapData map, int x, int y, int index, in TileElementInfo element)
        {
            Assert.IsNotNull(_prefab, nameof(_prefab));

            Vector3 position = World.TileCoordsToUnity(x, y, element.baseHeight);
            Quaternion rotation = Quaternion.Euler(0, 90 * element.rotation + 90, 0);

            Object.Instantiate(_prefab, position, rotation, map.transform);
        }
    }
}
