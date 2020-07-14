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
        public TData Get(SerializedProperty property)
        {
            cache.TryGetValue(GetCacheKey(property), out TData data);
            return data;
        }


        /// <summary>
        /// Saves the data for the specified property.
        /// </summary>
        public void Set(SerializedProperty property, TData data)
            => cache[GetCacheKey(property)] = data;


        /// <summary>
        /// Gets the key (path) of the cache.
        /// </summary>
        static string GetCacheKey(SerializedProperty property)
            => $"{property.propertyPath}<{property.serializedObject.targetObject}>";
    }
}
