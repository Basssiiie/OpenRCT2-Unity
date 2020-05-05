namespace OpenRCT2.Unity
{
    /// <summary>
    /// An element representing a piece of wall.
    /// </summary>
    public readonly ref struct WallElement
    {
        // Mask to get the bits for the scenery quadrant/wall side.
        const byte SlopeMask = 0b11000000;


        /// <summary>
        /// The type of the element. In this case a wall element.
        /// </summary>
        public TileElementType Type => element.Type;


        /// <summary>
        /// Gets the slope for this wall element occupies.
        /// </summary>
        public byte Slope => (byte)((element.type & SlopeMask) >> 6);



        /// <summary>
        /// The entry index of the graphic of the small scenery element.
        /// </summary>
        public uint EntryIndex => (element.slot0x1 + ((uint)element.slot0x2 << 8));


        /* 0x1 = EntryIndex
         * 0x2 = EntryIndex
         * 0x3 = Color 1
         * 0x4 = Color 2
         * 0x5 = Color 3
         * 0x6 = BannerIndex
         * 0x7 = BannerIndex
         * 0x8 = Animation (1st 2 bits: zero, 3rd bit: unused, 4-7th bit: frame number, 8th: direction)
         */
        readonly TileElement element;


        /// <summary>
        /// Wraps the tile element to access the wall element information.
        /// </summary>
        public WallElement(ref TileElement element)
        {
            this.element = element;
        }
    }
}
