namespace OpenRCT2.Unity
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
