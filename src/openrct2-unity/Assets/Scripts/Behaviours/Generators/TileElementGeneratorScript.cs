using OpenRCT2.Generators.Map;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours.Generators
{
    /// <summary>
    /// A base generator that creates the tile elements.
    /// </summary>
    public abstract class TileElementGeneratorScript : ScriptableObject
    {
        protected const string menuPath = "OpenRCT2/Generators/";

        public abstract ITileElementGenerator CreateGenerator();
    }
}
