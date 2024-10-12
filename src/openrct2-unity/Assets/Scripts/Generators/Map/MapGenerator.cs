using System.Collections.Generic;
using OpenRCT2.Bindings;
using OpenRCT2.Bindings.TileElements;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map
{
    /// <summary>
    /// A generator for the map of the park.
    /// </summary>
    public class MapGenerator
    {
        readonly IReadOnlyDictionary<TileElementType, ITileElementGenerator> _generators;


        public MapGenerator(IReadOnlyDictionary<TileElementType, ITileElementGenerator> generators)
        {
            _generators = generators;
        }


        /// <summary>
        /// Generates the map. 
        /// </summary>
        public IEnumerator<LoadStatus> GenerateMap(Transform parent)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var size = Park.GetMapSize();

            // Remove map border
            var width = size.width - 2;
            var height = size.height - 2;
            var total = width * height;
            var tiles = new Tile[width, height];

            // Load in all tiles for the map
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < width; y++)
                {
                    yield return new LoadStatus("Loading tiles...", x * width + y, total);

                    tiles[x, y] = Tile.GetAt(x + 1, y + 1); // skip border
                }
            }

            // Create the map and run the object generators
            var map = new Map(tiles);

            int generatorCount = 1;
            foreach (ITileElementGenerator generator in _generators.Values)
            {
                yield return new LoadStatus($"Running {generator.name}...", generatorCount, _generators.Count);

                var runner = generator.Run(map, parent);
                while (runner.MoveNext())
                {
                    yield return runner.Current;
                }
                generatorCount++;
            }

            Debug.Log($"Map load complete in {watch.Elapsed}!");
        }
    }
}
