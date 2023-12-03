using System.Runtime.InteropServices;

#nullable enable

namespace OpenRCT2.Bindings.TileElements
{
    public readonly struct PathInfo
    {
        public readonly uint surfaceIndex;
        public readonly uint railingIndex;

        [MarshalAs(UnmanagedType.I1)]
        public readonly bool sloped;
        public readonly byte slopeDirection;
    };
}
