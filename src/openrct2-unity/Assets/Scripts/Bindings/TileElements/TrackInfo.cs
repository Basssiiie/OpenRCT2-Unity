using System.Runtime.InteropServices;

#nullable enable

namespace OpenRCT2.Bindings.TileElements
{
    public readonly struct TrackInfo
    {
        public readonly ushort trackType;
        public readonly sbyte trackHeight;
        public readonly byte sequenceIndex;
        public readonly byte mainColour;
        public readonly byte additionalColour;
        public readonly byte supportsColour;

        [MarshalAs(UnmanagedType.I1)]
        public readonly bool chainlift;
        [MarshalAs(UnmanagedType.I1)]
        public readonly bool cablelift;
        [MarshalAs(UnmanagedType.I1)]
        public readonly bool inverted;
        [MarshalAs(UnmanagedType.I1)]
        public readonly bool normalToInverted;
        [MarshalAs(UnmanagedType.I1)]
        public readonly bool invertedToNormal;
    };
}
