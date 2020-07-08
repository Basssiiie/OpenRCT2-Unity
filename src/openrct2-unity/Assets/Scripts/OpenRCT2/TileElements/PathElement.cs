namespace Lib
{
    /// <summary>
    /// An element representing a piece of path.
    /// </summary>
    public readonly ref struct PathElement
    {
        /// <summary>
        /// The type of the element. In this case a path element.
        /// </summary>
        public TileElementType Type => element.Type;


        /// <summary>
        /// Bits of which edges and corners are connected, where the first 4
        /// bits are the edges and the last 4 the corners.
        /// </summary>
        public byte EdgesAndCorners => element.slot0x6;


        /// <summary>
        /// A second set of flags for this path element.
        /// </summary>
        public PathElementFlags Flags2 => (PathElementFlags)element.slot0x7;


        /// <summary>
        /// Returns whether this path element is sloped or flat.
        /// </summary>
        public bool IsSloped => ((element.slot0x7 & (int)PathElementFlags.IsSloped) != 0);


        /// <summary>
        /// Returns the direction in which the slope is, if the IsSloped bit is set.
        /// </summary>
        public byte SlopeDirection => element.slot0x8;



        /* 0x1 = SurfaceIndex
         * 0x2 = SurfaceIndex
         * 0x3 = RailingsIndex
         * 0x4 = RailingsIndex
         * 0x5 = Additions
         * 0x6 = Edges
         * 0x7 = Flags2
         * 0x8 = SlopeDirection
         * 0x9 = RideIndex / AdditionStatus
         * 0xA = RideIndex 
         * 0xB = StationIndex
         */
        readonly TileElement element;


        /// <summary>
        /// Wraps the tile element to access the path element information.
        /// </summary>
        public PathElement(ref TileElement element)
        {
            this.element = element;
        }
    }
}
