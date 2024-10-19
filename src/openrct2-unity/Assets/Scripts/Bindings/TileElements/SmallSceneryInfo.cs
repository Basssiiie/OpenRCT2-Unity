using System.Runtime.InteropServices;

#nullable enable

namespace OpenRCT2.Bindings.TileElements
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct SmallSceneryInfo
    {
        public readonly uint imageIndex;
        public readonly ushort objectIndex;
        public readonly byte quadrant;
        [MarshalAs(UnmanagedType.I1)]
        public readonly bool fullTile;

        public readonly byte colour1;
        public readonly byte colour2;
        public readonly byte colour3;

        [MarshalAs(UnmanagedType.I1)]
        public readonly bool animated;
        public readonly ushort animationFrameCount;
        public readonly ushort animationFrameDelay;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public readonly string identifier;
    };
}
