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

        [SerializeField] TileElementFlags _generationFlags = TileElementFlags.All;

        [SerializeField] TileElementGenerator _surfaceGenerator;
        [SerializeField] TileElementGenerator _pathGenerator;
        [SerializeField] TileElementGenerator _trackGenerator;
        [SerializeField] TileElementGenerator _smallSceneryGenerator;
        [SerializeField] TileElementGenerator _entranceGenerator;
        [SerializeField] TileElementGenerator _wallGenerator;
        [SerializeField] TileElementGenerator _largeSceneryGenerator;
        [SerializeField] TileElementGenerator _bannerGenerator;


        /// <summary>
        /// Returns all generators currently selected.
        /// </summary>
        TileElementGenerator[] GetGenerators()
        {
            return new TileElementGenerator[]
            {
                 _surfaceGenerator,
                 _pathGenerator,
                 _trackGenerator,
                 _smallSceneryGenerator,
                 _entranceGenerator,
                 _wallGenerator,
                 _largeSceneryGenerator,
                 _bannerGenerator
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
                    if ((_generationFlags & TileElementFlags.Surface) != 0)
                        _surfaceGenerator.CreateElement(x, y, in tile);
                    break;

                case TileElementType.Path:
                    if ((_generationFlags & TileElementFlags.Path) != 0)
                        _pathGenerator.CreateElement(x, y, in tile);
                    break;

                case TileElementType.Track:
                    if ((_generationFlags & TileElementFlags.Track) != 0)
                        _trackGenerator.CreateElement(x, y, in tile);
                    break;

                case TileElementType.SmallScenery:
                    if ((_generationFlags & TileElementFlags.SmallScenery) != 0)
                        _smallSceneryGenerator.CreateElement(x, y, in tile);
                    break;

                case TileElementType.Entrance:
                    if ((_generationFlags & TileElementFlags.Entrance) != 0)
                        _entranceGenerator.CreateElement(x, y, in tile);
                    break;

                case TileElementType.Wall:
                    if ((_generationFlags & TileElementFlags.Wall) != 0)
                        _wallGenerator.CreateElement(x, y, in tile);
                    break;

                case TileElementType.LargeScenery:
                    if ((_generationFlags & TileElementFlags.LargeScenery) != 0)
                        _largeSceneryGenerator.CreateElement(x, y, in tile);
                    break;

                case TileElementType.Banner:
                    if ((_generationFlags & TileElementFlags.Banner) != 0)
                        _bannerGenerator.CreateElement(x, y, in tile);
                    break;
            }
        }
    }
}
