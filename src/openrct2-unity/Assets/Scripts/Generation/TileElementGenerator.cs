using Lib;
using UnityEngine;

namespace Generation
{
    /// <summary>
    /// A base generator that creates the tile elements.
    /// </summary>
    public abstract class TileElementGenerator : ScriptableObject
    {
        protected const string MenuPath = "OpenRCT2/Generators/";

        protected Map _map;


        /// <summary>
        /// Initialises the generator for the specified map, before new elements
        /// are created.
        /// </summary>
        public void StartGenerator(Map map)
        {
            _map = map;
            Start();
        }


        /// <summary>
        /// Finalizes the created elements and finishes the generator.
        /// </summary>
        public void FinishGenerator()
        {
            Finish();
            _map = null;
        }


        /// <summary>
        /// Creates a tile element at the specified tile position.
        /// </summary>
        public abstract void CreateElement(int x, int y, in TileElement tile);


        /// <summary>
        /// Override to add startup code.
        /// </summary>
        protected virtual void Start() {}


        /// <summary>
        /// Override to add finish code.
        /// </summary>
        protected virtual void Finish() {}
    }
}
