using OpenRCT2.Generators.Map;
using OpenRCT2.Generators.Map.Utilities;
using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours.Generators
{
    /// <summary>
    /// A simple generator that spawns the specified prefab for the given tile element.
    /// </summary>
    [CreateAssetMenu(menuName = (menuPath + "Utilities/" + nameof(PrefabGeneratorScript)))]
    public class PrefabGeneratorScript : TileElementGeneratorScript
    {
        [SerializeField, Required] GameObject? _prefab;

        public override ITileElementGenerator CreateGenerator()
        {
            return new PrefabGenerator(_prefab);
        }
    }
}
