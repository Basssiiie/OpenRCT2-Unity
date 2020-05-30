namespace Lib
{
    /// <summary>
    /// An element representing a trackpiece of a ride, or a stall.
    /// </summary>
    public readonly ref struct TrackElement
    {
        // Mask to get the bits for whether the track has a chainlift.
        const byte ChainliftMask = 0b_1000_0000;

        // Mask to get the bits for the colour scheme.
        const byte ColourSchemeMask = 0b_0000_0011;


        /// <summary>
        /// The type of the element. In this case a track element.
        /// </summary>
        public TileElementType Type => element.Type;


        /// <summary>
        /// Returns true of the track element has a chainlift.
        /// </summary>
        public bool HasChainlift => (element.type & ChainliftMask) != 0;


        /// <summary>
        /// Returns the track type index of this track element.
        /// </summary>
        public short TrackType => (short)((element.slot0x2 << 8) + element.slot0x1);


        /// <summary>
        /// Returns the index of the tile part of this track element. A multi-tile
        /// track-piece will have multiple parts, one for each tile. Also called
        /// 'sequence' in OpenRCT2 source code.
        /// </summary>
        public byte PartIndex => element.slot0x3;


        /// <summary>
        /// Returns the index of which colour scheme this track element uses. 
        /// </summary>
        public byte ColourScheme => (byte)(element.slot0x4 & ColourSchemeMask);


        /// <summary>
        /// Returns the track type index of this track element.
        /// </summary>
        public short RideIndex => (short)((element.slot0x9 << 8) + element.slot0x8);


        /* 0x1 = TrackType
         * 0x2 = TrackType
         * 0x3 = Sequence / Maze
         * 0x4 = ColourScheme / Maze
         * 0x5 = OnRidePhoto / BrakeBoosterSpeed
         * 0x6 = StationIndex
         * 0x7 = Flags2
         * 0x8 = RideIndex
         * 0x9 = RideIndex
         */
        readonly TileElement element;


        /// <summary>
        /// Wraps the tile element to access the track element information.
        /// </summary>
        public TrackElement(ref TileElement element)
        {
            this.element = element;
        }
    }
}
