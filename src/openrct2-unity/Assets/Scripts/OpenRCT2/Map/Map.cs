using UnityEngine;
using Utilities;

namespace Lib
{
    /// <summary>
    /// The map of the park.
    /// </summary>
    public partial class Map : MonoBehaviour
    {
        /// <summary>
        /// Gets the size of one side of the map in tiles.
        /// </summary>
        public int Size { get; private set; }


        /// <summary>
        /// Gets all tiles of the map as a 2-dimensional array.
        /// </summary>
        public Tile[,] Tiles { get; private set; }


        const int MaxElementsPerTile = 128;


		void Start()
		{
            _loader.RunCoroutine(GenerateMap());
        }
    }
}
