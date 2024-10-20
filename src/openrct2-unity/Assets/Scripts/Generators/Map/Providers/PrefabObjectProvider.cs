using OpenRCT2.Bindings.TileElements;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map.Providers
{
    /// <summary>
    /// A provider that creates objects based on prefabs without further modification
    /// </summary>
    public class PrefabObjectProvider<T> : IObjectProvider<T> where T : struct
    {
        readonly GameObject _prefab;

        public PrefabObjectProvider(GameObject prefab)
        {
            _prefab = prefab;
        }

        public GameObject CreateObject(int x, int y, int index, in TileElementInfo element, in T data)
        {
            var obj = Object.Instantiate(_prefab);
            obj.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            return obj;
        }
    }
}
