public readonly struct TrackInfo
{
    public readonly ushort trackType;
    public readonly ushort trackLength;
    public readonly sbyte trackHeight;
    public readonly byte sequenceIndex;
    public readonly byte mainColour;
    public readonly byte additionalColour;
    public readonly byte supportsColour;
    public readonly bool chainlift;
    public readonly bool cablelift;
    public readonly bool inverted;
    public readonly bool normalToInverted;
    public readonly bool invertedToNormal;
};
