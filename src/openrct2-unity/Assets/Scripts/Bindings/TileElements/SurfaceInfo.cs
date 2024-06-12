#nullable enable

namespace OpenRCT2.Bindings.TileElements
{
    public readonly struct SurfaceInfo
    {
        public readonly uint surfaceImageIndex;
        public readonly uint edgeImageIndex;

        public readonly int waterHeight;
        public readonly SurfaceSlope slope;
    };
}
