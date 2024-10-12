using System.Collections.Generic;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map
{
    /// <summary>
    /// A base generator that creates the tile elements.
    /// </summary>
    public interface ITileElementGenerator
    {
        /// <summary>
        /// Returns the name of the generator.
        /// </summary>
        string name => GetType().Name;

        /// <summary>
        /// Runs the map generator as a coroutine for the specified map, to generate tile elements where needed.
        /// </summary>
        IEnumerator<LoadStatus> Run(Map map, Transform transform);
    }
}
