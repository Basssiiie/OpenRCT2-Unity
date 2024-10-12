using System;
using System.Collections.Generic;
using OpenRCT2.Bindings;
using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Generators.Map;

#nullable enable

namespace OpenRCT2.Generators.Extensions
{
    public static class MapExtensions
    {
        public delegate void ElementCallback(Tile tile, int index, in TileElementInfo element);
        public delegate void ElementCallback<T>(Tile tile, int index, in TileElementInfo element, in T data) where T : struct;


        public static IEnumerator<LoadStatus> ForEach(this Map.Map map, string loader, ElementCallback callback)
        {
            var width = map.width;
            var height = map.height;
            var total = width * height;
            var current = 0;

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var tile = map.tiles[x, y];
                    var elements = tile.elements;

                    for (var i = 0; i < elements.Length; i++)
                    {
                        callback(tile, i, in elements[i]);
                    }

                    yield return new LoadStatus(loader, current++, total);
                }
            }
        }

        public static IEnumerator<LoadStatus> ForEach<T>(this Map.Map map, string loader, ElementCallback<T> callback) where T : struct
        {
            return default(T) switch
            {
                SurfaceInfo => ForEachType(map, loader, TileElementType.Surface, static tile => tile.surfaces as T[], callback),
                PathInfo => ForEachType(map, loader, TileElementType.Path, static tile => tile.paths as T[], callback),
                TrackInfo => ForEachType(map, loader, TileElementType.Track, static tile => tile.tracks as T[], callback),
                SmallSceneryInfo => ForEachType(map, loader, TileElementType.SmallScenery, static tile => tile.smallScenery as T[], callback),
                WallInfo => ForEachType(map, loader, TileElementType.Wall, static tile => tile.walls as T[], callback),

                _ => throw new NotImplementedException($"Iterating type '{typeof(T).Name}' not yet implemented")
            };
        }

        static IEnumerator<LoadStatus> ForEachType<T>(Map.Map map, string loader, TileElementType type, Func<Tile, T[]?> getter, ElementCallback<T> callback) where T : struct
        {
            Tile? previous = null;
            T[]? data = null;
            var typeIdx = 0;

            return ForEach(map, loader, (Tile tile, int index, in TileElementInfo element) =>
            {
                if (element.type == type)
                {
                    if (tile != previous)
                    {
                        data = getter(tile) ?? throw new InvalidOperationException($"Failed to get array for type '{type}'");
                        previous = tile;
                        typeIdx = 0;
                    }

                    callback(tile, index, in element, in data![typeIdx++]);
                }
            });
        }
    }
}
