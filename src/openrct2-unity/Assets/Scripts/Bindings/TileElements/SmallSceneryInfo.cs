using System.Runtime.InteropServices;

#nullable enable

namespace OpenRCT2.Bindings.TileElements
{
    public readonly struct SmallSceneryInfo
    {
        public readonly uint imageIndex;
        public readonly ushort objectIndex;
        public readonly byte quadrant;
        [MarshalAs(UnmanagedType.I1)]
        public readonly bool fullTile;

        public readonly ushort animationFrameCount;
        public readonly ushort animationFrameDelay;
        [MarshalAs(UnmanagedType.I1)]
        public readonly bool animated;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public readonly string identifier;
    };
}
