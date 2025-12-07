using OpenRCT2.Bindings.TileElements;

#nullable enable

namespace OpenRCT2.Bindings
{
    public readonly ref struct Element<T>
    {
        public readonly Tile tile;
        public readonly TileElementInfo info;
        public readonly short index;
        public readonly short offset;

        readonly T[] _array;

        public Element(Tile tile, in TileElementInfo info, short index, short offset, T[] array)
        {
            this.tile = tile;
            this.info = info;
            this.index = index;
            this.offset = offset;
            _array = array;
        }

        public ref T GetData() => ref _array[offset];
    }
}
