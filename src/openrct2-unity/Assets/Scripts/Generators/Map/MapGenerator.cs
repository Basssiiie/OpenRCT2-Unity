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

        const int _maxElementsPerTile = 128;


        public MapGenerator(IReadOnlyDictionary<TileElementType, ITileElementGenerator> generators)
        {
            _generators = generators;
        }


        /// <summary>
        /// Generates the map. 
        /// </summary>
        public IEnumerable<MapLoadStatus> GenerateMap(Transform parent)
        {
            var size = Park.GetMapSize();
            var map = new MapData(size.width, size.height, parent);

            // Start the generators
            int generatorCount = 1;
            foreach (ITileElementGenerator generator in _generators.Values)
            {
                yield return new MapLoadStatus($"Starting {generator.name}...", generatorCount, _generators.Count);
                generator.Start(map);
                generatorCount++;
            }

            // Create the tile objects
            int width = size.width - 1;
            int height = size.height - 1;
            int totalTiles = width * height;
            TileElementInfo[] buffer = new TileElementInfo[_maxElementsPerTile];

            for (int x = 1; x < width; x++)
            {
                for (int y = 1; y < width; y++)
                {
                    yield return new MapLoadStatus("Creating tiles...", x * width + y, totalTiles);

                    int count = Park.GetMapElementsAt(x, y, buffer);
                    for (int idx = 0; idx < count; idx++)
                    {
                        TileElementInfo element = buffer[idx];
                        if (!element.invisible && _generators.TryGetValue(element.type, out ITileElementGenerator generator))
                        {
                            generator.CreateElement(map, x, y, idx, in element);
                        }
                    }
                }
            }

            // Finish up the generators
            generatorCount = 0;
            foreach (ITileElementGenerator generator in _generators.Values)
            {
                yield return new MapLoadStatus($"Finalizing {generator.name}...", generatorCount, _generators.Count);
                generator.Finish(map);
                generatorCount++;
            }

            Debug.Log($"Map load complete!");
        }
    }
}
