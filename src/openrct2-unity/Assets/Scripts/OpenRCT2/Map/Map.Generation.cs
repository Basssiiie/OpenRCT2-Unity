using System;
using Generation;
using UnityEngine;

namespace Lib
{
    public partial class Map
    {
        /// <summary>
        /// Flags to select which elements to generate and which to ignore.
        /// </summary>
        [Flags]
        enum TileElementFlags
        {
            None = 0,
            Surface = (1 << 0),
            Path = (1 << 1),
            Track = (1 << 2),
            SmallScenery = (1 << 3),
            Entrance = (1 << 4),
            Wall = (1 << 5),
            LargeScenery = (1 << 6),
            Banner = (1 << 7),
            All = ~0
        };

        [SerializeField] TileElementFlags generationFlags = TileElementFlags.All;

        [SerializeField] TileElementGenerator surfaceGenerator;
        [SerializeField] TileElementGenerator pathGenerator;
        [SerializeField] TileElementGenerator trackGenerator;
        [SerializeField] TileElementGenerator smallSceneryGenerator;
        [SerializeField] TileElementGenerator entranceGenerator;
        [SerializeField] TileElementGenerator wallGenerator;
        [SerializeField] TileElementGenerator largeSceneryGenerator;
        [SerializeField] TileElementGenerator bannerGenerator;


        /// <summary>
        /// Returns all generators currently selected.
        /// </summary>
        TileElementGenerator[] GetGenerators()
        {
            return new TileElementGenerator[]
            {
                 surfaceGenerator,
                 pathGenerator,
                 trackGenerator,
                 smallSceneryGenerator,
                 entranceGenerator,
                 wallGenerator,
                 largeSceneryGenerator,
                 bannerGenerator
            };
        }


        /// <summary>
        /// Generates the surface of the map.public 
        /// </summary>
        void GenerateMap()
        {
            TileElementGenerator[] generators = GetGenerators();

            foreach (var generator in generators)
                generator.StartGenerator(this);

            int end = (Size - 1);
            for (int x = 1; x < end; x++)
            {
                for (int y = 1; y < end; y++)
                {
                    Tile tile = Tiles[x, y];
                    for (int e = 0; e < tile.Count; e++)
                    {
                        GenerateTileElement(x, y, in tile.Elements[e]);
                    }
                }
            }

            foreach (var generator in generators)
                generator.FinishGenerator();
        }


        /// <summary>
        /// Generates a tile element based on the type of the given tile.
        /// </summary>
        void GenerateTileElement(int x, int y, in TileElement tile)
        {
            switch (tile.Type)
            {
                case TileElementType.Surface:
                    if ((generationFlags & TileElementFlags.Surface) != 0)
                        surfaceGenerator.CreateElement(x, y, in tile);
                    break;

                case TileElementType.Path:
                    if ((generationFlags & TileElementFlags.Path) != 0)
                        pathGenerator.CreateElement(x, y, in tile);
                    break;

                case TileElementType.Track:
                    if ((generationFlags & TileElementFlags.Track) != 0)
                        trackGenerator.CreateElement(x, y, in tile);
                    break;

                case TileElementType.SmallScenery:
                    if ((generationFlags & TileElementFlags.SmallScenery) != 0)
                        smallSceneryGenerator.CreateElement(x, y, in tile);
                    break;

                case TileElementType.Entrance:
                    if ((generationFlags & TileElementFlags.Entrance) != 0)
                        entranceGenerator.CreateElement(x, y, in tile);
                    break;

                case TileElementType.Wall:
                    if ((generationFlags & TileElementFlags.Wall) != 0)
                        wallGenerator.CreateElement(x, y, in tile);
                    break;

                case TileElementType.LargeScenery:
                    if ((generationFlags & TileElementFlags.LargeScenery) != 0)
                        largeSceneryGenerator.CreateElement(x, y, in tile);
                    break;

                case TileElementType.Banner:
                    if ((generationFlags & TileElementFlags.Banner) != 0)
                        bannerGenerator.CreateElement(x, y, in tile);
                    break;
            }
        }
    }
}
