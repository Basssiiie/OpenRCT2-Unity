using System.Collections.Generic;
using UnityEditor;

namespace EditorExtensions
{
    /// <summary>
    /// Cache for storing data related to <see cref="SerializedProperty"/> instances.
    /// </summary>
    class DrawerCache<TData>
    {
        readonly Dictionary<string, TData> cache = new Dictionary<string, TData>();


        /// <summary>
        /// Gets the data for the specified property. Returns the default of <typeparamref name="TData"/> if none saved.
        /// </summary>
        public string Get(SerializedProperty property, out TData data)
        {
            string key = GetCacheKey(property);
            cache.TryGetValue(key, out data);
            return key;
        }


        /// <summary>
        /// Saves the data for the specified property.
        /// </summary>
        public void Set(string key, TData data)
            => cache[key] = data;


        /// <summary>
        /// Gets the key (path) of the cache.
        /// </summary>
        static string GetCacheKey(SerializedProperty property)
            => $"{property.propertyPath}<{property.serializedObject.targetObject}>";
    }
}
