using UnityEngine;

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
			LoadMap();
            GenerateMap();

            Debug.Log($"Map load complete!");
        }


        /// <summary>
        /// Loads the map of the currently loaded park into Unity.
        /// </summary>
		void LoadMap()
		{
            // Remove all children
            foreach (Transform child in transform)
                Destroy(child.gameObject);

            // Load the map
            Size = OpenRCT2.GetMapSize();
			Tiles = new Tile[Size, Size];

			TileElement[] buffer = new TileElement[MaxElementsPerTile];

			for (int x = 0; x < Size; x++)
			{
				for (int y = 0; y < Size; y++)
				{
					int amount = OpenRCT2.GetMapElementsAt(x, y, buffer);

					Tiles[x, y] = new Tile(buffer, amount);
				}
			}
        }        
    }
}
