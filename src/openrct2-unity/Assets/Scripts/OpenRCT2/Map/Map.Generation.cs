using System;
using System.Collections;
using Generation;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

#nullable enable

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

        [SerializeField] TileElementGenerator? _surfaceGenerator;
        [SerializeField] TileElementGenerator? _pathGenerator;
        [SerializeField] TileElementGenerator? _trackGenerator;
        [SerializeField] TileElementGenerator? _smallSceneryGenerator;
        [SerializeField] TileElementGenerator? _entranceGenerator;
        [SerializeField] TileElementGenerator? _wallGenerator;
        [SerializeField] TileElementGenerator? _largeSceneryGenerator;
        [SerializeField] TileElementGenerator? _bannerGenerator;

        [SerializeField] LoaderView? _loader;
        [SerializeField] UnityEvent? _onGenerationComplete;


        /// <summary>
        /// Returns all generators currently selected.
        /// </summary>
        TileElementGenerator?[] GetGenerators()
        {
            return new TileElementGenerator?[]
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
        IEnumerator GenerateMap()
        {
            // Remove all children
            foreach (Transform child in transform)
                Destroy(child.gameObject);

            // Load the map
            Size = OpenRCT2.GetMapSize();
            Tiles = new Tile[Size, Size];

            if (_loader != null)
            {
                _loader.SetText($"Loading tiles...");
                _loader.SetMaximumProgress(Size * Size);
            }
            yield return null;

            TileElement[] buffer = new TileElement[MaxElementsPerTile];

            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    int amount = OpenRCT2.GetMapElementsAt(x, y, buffer);

                    Tiles[x, y] = new Tile(buffer, amount);
                    yield return null;
                }
            }

            // Start the generators
            TileElementGenerator?[] generators = GetGenerators();

            if (_loader != null)
                _loader.SetMaximumProgress(generators.Length);

            foreach (TileElementGenerator? generator in generators)
            {
                if (generator == null)
                    continue;

                if (_loader != null)
                    _loader.SetText($"Starting generator '{generator.name}'...");

                yield return null;

                generator.StartGenerator(this);
            }

            // Create the tile objects
            int end = (Size - 1);

            if (_loader != null)
            {
                _loader.SetText("Creating tiles...");
                _loader.SetMaximumProgress(end * end);
            }
            yield return null;

            for (int x = 1; x < end; x++)
            {
                for (int y = 1; y < end; y++)
                {
                    Tile tile = Tiles[x, y];
                    for (int e = 0; e < tile.Count; e++)
                    {
                        GenerateTileElement(x, y, in tile.Elements[e]);
                    }
                    yield return null;
                }
            }

            // Finish up the generators
            if (_loader != null)
                _loader.SetMaximumProgress(generators.Length);

            foreach (TileElementGenerator? generator in generators)
            {
                if (generator == null)
                    continue;

                if (_loader != null)
                    _loader.SetText($"Finalizing generator '{generator.name}'...");

                yield return null;

                generator.FinishGenerator();
            }

            Debug.Log($"Map load complete!");
            _onGenerationComplete?.Invoke();
        }


        /// <summary>
        /// Generates a tile element based on the type of the given tile.
        /// </summary>
        void GenerateTileElement(int x, int y, in TileElement tile)
        {
            switch (tile.Type)
            {
                case TileElementType.Surface:
                    if ((_generationFlags & TileElementFlags.Surface) != 0 && _surfaceGenerator != null)
                        _surfaceGenerator.CreateElement(this, x, y, in tile);
                    break;

                case TileElementType.Path:
                    if ((_generationFlags & TileElementFlags.Path) != 0 && _pathGenerator != null)
                        _pathGenerator.CreateElement(this, x, y, in tile);
                    break;

                case TileElementType.Track:
                    if ((_generationFlags & TileElementFlags.Track) != 0 && _trackGenerator != null)
                        _trackGenerator.CreateElement(this, x, y, in tile);
                    break;

                case TileElementType.SmallScenery:
                    if ((_generationFlags & TileElementFlags.SmallScenery) != 0 && _smallSceneryGenerator != null)
                        _smallSceneryGenerator.CreateElement(this, x, y, in tile);
                    break;

                case TileElementType.Entrance:
                    if ((_generationFlags & TileElementFlags.Entrance) != 0 && _entranceGenerator != null)
                        _entranceGenerator.CreateElement(this, x, y, in tile);
                    break;

                case TileElementType.Wall:
                    if ((_generationFlags & TileElementFlags.Wall) != 0 && _wallGenerator != null)
                        _wallGenerator.CreateElement(this, x, y, in tile);
                    break;

                case TileElementType.LargeScenery:
                    if ((_generationFlags & TileElementFlags.LargeScenery) != 0 && _largeSceneryGenerator != null)
                        _largeSceneryGenerator.CreateElement(this, x, y, in tile);
                    break;

                case TileElementType.Banner:
                    if ((_generationFlags & TileElementFlags.Banner) != 0 && _bannerGenerator != null)
                        _bannerGenerator.CreateElement(this, x, y, in tile);
                    break;
            }
        }
    }
}
