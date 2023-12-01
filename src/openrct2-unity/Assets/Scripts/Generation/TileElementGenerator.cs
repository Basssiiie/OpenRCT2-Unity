using Lib;
using UnityEngine;

#nullable enable

namespace Generation
{
    /// <summary>
    /// A base generator that creates the tile elements.
    /// </summary>
    public abstract class TileElementGenerator : ScriptableObject
    {
        protected const string MenuPath = "OpenRCT2/Generators/";

        Map? _map;


        /// <summary>
        /// Initialises the generator for the specified map, before new elements
        /// are created.
        /// </summary>
        public void StartGenerator(Map map)
        {
            _map = map;
            Startup(map);
        }


        /// <summary>
        /// Finalizes the created elements and finishes the generator.
        /// </summary>
        public void FinishGenerator()
        {
            if (_map != null)
            {
                Finish(_map);
                _map = null;
            }
        }


        /// <summary>
        /// Creates a tile element at the specified tile position.
        /// </summary>
        public abstract void CreateElement(Map map, int x, int y, int index, in TileElementInfo tile);


        /// <summary>
        /// Override to add startup code.
        /// </summary>
        protected virtual void Startup(Map map) {}


        /// <summary>
        /// Override to add finish code.
        /// </summary>
        protected virtual void Finish(Map map) {}
    }
}
