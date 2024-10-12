using OpenRCT2.Bindings;

#nullable enable

namespace OpenRCT2.Generators.Map
{
    public class Map
    {
        public readonly int width;
        public readonly int height;
        public readonly Tile[,] tiles;


        public Map(Tile[,] tiles)
        {
            width = tiles.GetLength(0);
            height = tiles.GetLength(1);

            this.tiles = tiles;
        }
    }
}
