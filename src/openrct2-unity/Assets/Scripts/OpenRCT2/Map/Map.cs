using System;
using UnityEngine;

namespace OpenRCT
{
    /// <summary>
    /// The map of the park.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
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


        /// <summary>
        /// Gets or sets the map's mesh.
        /// </summary>
        public Mesh Mesh
        {
            get => meshFilter.sharedMesh;
            set => meshFilter.sharedMesh = value;
        }


        /// <summary>
        /// Gets or sets the map's materials.
        /// </summary>
        public Material[] Materials
        {
            get => meshRenderer.sharedMaterials;
            set => meshRenderer.sharedMaterials = value;
        }


        const int MaxElementsPerTile = 128;


        MeshFilter meshFilter;
        MeshRenderer meshRenderer;


        void Awake()
		{
			meshFilter = gameObject.GetComponent<MeshFilter>();
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
        }


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
