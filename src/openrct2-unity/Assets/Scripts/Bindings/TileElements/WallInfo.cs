using System.Runtime.InteropServices;

#nullable enable

namespace OpenRCT2.Bindings.TileElements
{
    public readonly struct WallInfo
    {
        public readonly uint imageIndex;
        public readonly byte slope;
        [MarshalAs(UnmanagedType.I1)]
        public readonly bool animated;
        public readonly ushort animationFrameCount;
        public readonly ushort animationFrameDelay;
    };
}
