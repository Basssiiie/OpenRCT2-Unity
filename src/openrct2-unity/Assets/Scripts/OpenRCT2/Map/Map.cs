using System;
using System.Runtime.InteropServices;
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
        public MapSize Size { get; private set; }


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
