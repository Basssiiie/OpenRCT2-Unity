using OpenRCT2.Bindings.TileElements;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map.Providers
{
    [CreateAssetMenu(menuName = ("OpenRCT2/Objects/" + nameof(SmallSceneryPrefabProviderObject)))]
    public class SmallSceneryPrefabProviderObject : PrefabProviderObject<SmallSceneryInfo>
    {
    }
}
