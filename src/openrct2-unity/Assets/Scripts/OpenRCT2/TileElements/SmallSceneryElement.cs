
#nullable enable

namespace Lib
{
    /// <summary>
    /// An element representing a small scenery piece.
    /// </summary>
    public readonly ref struct SmallSceneryElement
    {
        // Mask to get the bits for the scenery quadrant.
        const byte QuadrantMask = 0b_1100_0000;


        /// <summary>
        /// The type of the element. In this case a small scenery element.
        /// </summary>
        public TileElementType Type => element.Type;


        /// <summary>
        /// Gets the quadrant(s) this scenery item occupies.
        /// </summary>
        public byte Quadrant => (byte)((element.type & QuadrantMask) >> 6);


        /// <summary>
        /// The entry index of the graphic of the small scenery element.
        /// </summary>
        public uint EntryIndex => (element.slot0x1 + ((uint)element.slot0x2 << 8));


        /* 0x1 = EntryIndex
         * 0x2 = EntryIndex
         * 0x3 = Age
         * 0x4 = Colour 1
         * 0x5 = Colour 2
         */
        readonly TileElement element;


        /// <summary>
        /// Wraps the tile element to access the small scenery information.
        /// </summary>
        public SmallSceneryElement(in TileElement element)
        {
            this.element = element;
        }
    }
}
