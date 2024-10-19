using System.Runtime.InteropServices;

#nullable enable

namespace OpenRCT2.Bindings.TileElements
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct WallInfo
    {
        public readonly uint imageIndex;
        public readonly byte slope;

        public readonly byte colour1;
        public readonly byte colour2;
        public readonly byte colour3;

        public readonly ushort animationFrameCount;
        public readonly ushort animationFrameDelay;
        [MarshalAs(UnmanagedType.I1)]
        public readonly bool animated;
    };
}
