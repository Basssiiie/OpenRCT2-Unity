using System.Runtime.InteropServices;

public readonly struct SmallSceneryInfo
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
    public readonly string identifier;
    public readonly ushort objectIndex;
    public readonly uint imageIndex;
    public readonly byte quadrant;
    public readonly bool fullTile;
    public readonly bool animated;
    public readonly ushort animationFrameCount;
    public readonly ushort animationFrameDelay;
};
