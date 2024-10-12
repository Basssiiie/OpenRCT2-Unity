using OpenRCT2.Bindings;
using OpenRCT2.Bindings.TileElements;

#nullable enable

namespace OpenRCT2.Generators.Extensions
{
    public static class TileExtensions
    {
        public static bool TryGetFirst(this Tile tile, TileElementType type, out TileElementInfo element)
        {
            var elements = tile.elements;

            for (var i = 0; i < elements.Length; i++)
            {
                var elem = elements[i];
                if (elem.type == type)
                {
                    element = elem;
                    return true;
                }
            }

            element = default;
            return false;
        }

        public static int FindIndex(this Tile tile, TileElementType type)
        {
            var elements = tile.elements;

            for (var i = 0; i < elements.Length; i++)
            {
                if (elements[i].type == type)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
