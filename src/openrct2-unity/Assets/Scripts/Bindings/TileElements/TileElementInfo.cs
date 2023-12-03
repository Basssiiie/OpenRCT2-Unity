using System.Runtime.InteropServices;

#nullable enable

namespace OpenRCT2.Bindings.TileElements
{
    public readonly struct TileElementInfo
    {
        public readonly TileElementType type;
        public readonly byte rotation;
        public readonly byte baseHeight;
        [MarshalAs(UnmanagedType.I1)]
        public readonly bool invisible;
    };
}
