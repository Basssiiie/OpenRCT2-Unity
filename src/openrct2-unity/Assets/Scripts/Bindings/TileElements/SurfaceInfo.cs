#nullable enable

namespace OpenRCT2.Bindings.TileElements
{
    public readonly struct SurfaceInfo
    {
        public readonly uint surfaceIndex;
        public readonly uint edgeIndex;

        public readonly int waterHeight;
        public readonly SurfaceSlope slope;
    };
}
