#nullable enable

namespace OpenRCT2.Bindings.TileElements
{
    public readonly struct TunnelInfo
    {
        public readonly ushort tunnelType;
        public readonly ushort trackType;
        public readonly uint edgeImageIndex;
        public readonly bool enters;
        public readonly bool exits;

        public TunnelInfo(ushort tunnelType, ushort trackType, uint edgeImageIndex, bool enters, bool exits)
        {
            this.tunnelType = tunnelType;
            this.trackType = trackType;
            this.edgeImageIndex = edgeImageIndex;
            this.enters = enters;
            this.exits = exits;
        }
    }
}
