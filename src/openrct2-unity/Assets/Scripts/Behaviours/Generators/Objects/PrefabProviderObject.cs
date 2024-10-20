using System;
using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map.Providers
{
    [CreateAssetMenu(menuName = ("OpenRCT2/Objects/" + nameof(SmallSceneryPrefabProviderObject)))]
    public class SmallSceneryPrefabProviderObject : PrefabProviderObject<SmallSceneryInfo>
    {
    }

    public abstract class PrefabProviderObject<T> : ProviderObject<T> where T : struct
    {
        [SerializeField]
        [ContextMenuItem("Sort Alphabetically", nameof(SortObjectIds))]
        PrefabEntry[] _prefabs = null!;

        public override (string identifier, IObjectProvider<T> provider)[] GetEntries()
        {
            var length = _prefabs.Length;
            var array = new (string identifiers, IObjectProvider<T> provider)[length];

            for (var i = 0; i < length; i++)
            {
                ref var entry = ref _prefabs[i];
                array[i] = (entry.identifier, new PrefabObjectProvider<T>(entry.prefab));
            }

            return array;
        }

        /// <summary>
        /// Sorts all object ids alphabetically.
        /// </summary>
        void SortObjectIds()
        {
            Array.Sort(_prefabs, static (a, b) => StringComparer.Ordinal.Compare(a.identifier, b.identifier));
        }

        [Serializable]
        struct PrefabEntry
        {
            [Required] public string identifier;
            [Required] public GameObject prefab;
        }
    }
}
