namespace OpenRCT2.Unity
{
    /// <summary>
    /// An element representing a trackpiece of a ride, or a stall.
    /// </summary>
    public readonly ref struct TrackElement
    {
        public TileElementType Type => element.Type;


        /* 0x1 = TrackType
         * 0x2 = TrackType
         * 0x3 = Sequence / Maze
         * 0x4 = Sequence / Maze
         * 0x5 = OnRidePhoto / BrakeBoosterSpeed
         * 0x6 = StationIndex
         * 0x7 = Flags2
         * 0x8 = RideIndex
         */
        readonly TileElement element;


        public TrackElement(ref TileElement element)
        {
            this.element = element;
        }
    }
}
