using OpenRCT2.Bindings;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map.Providers
{
    public interface IObjectProvider<T> where T : struct
    {
        /// <summary>
        /// Creates an object according to the provider's implementation.
        /// </summary>
        GameObject? CreateObject(Map map, in Element<T> element);
    }
}
