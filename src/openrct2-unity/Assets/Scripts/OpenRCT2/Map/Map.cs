using System;
using UnityEngine;
using Utilities;

#nullable enable

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
        public Tile[,] Tiles { get; private set; } = new Tile[0, 0];


        const int MaxElementsPerTile = 128;


		void Start()
		{
            if (_loader != null)
            {
                _loader.RunCoroutine(GenerateMap());
            }
            else
            {
                StartCoroutine(GenerateMap());
            }
        }
    }
}
