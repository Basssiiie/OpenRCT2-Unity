
#nullable enable

namespace Lib
{
    /// <summary>
    /// An element representing the surface of the map.
    /// </summary>
    public readonly ref struct SurfaceElement
    {
        /// <summary>
        /// The type of the element. In this case a surface element.
        /// </summary>
        public TileElementType Type => element.Type;


        /// <summary>
        /// The base height of the element.
        /// </summary>
        public int BaseHeight => element.baseHeight;


        /// <summary>
        /// A flaggable describing how the surface element is sloped.
        /// </summary>
        public SurfaceSlope Slope => (SurfaceSlope)element.slot0x1;


        /// <summary>
        /// The height at which the water is, if there is any.
        /// </summary>
        public int WaterHeight => element.slot0x2;


        /// <summary>
        /// A flaggable describing the possible ownerships of this surface element.
        /// </summary>
        public Ownership Ownership => (Ownership)element.slot0x4;


        /// <summary>
        /// The graphical style of this surface element.
        /// </summary>
        public TerrainSurfaceStyle SurfaceStyle => (TerrainSurfaceStyle)element.slot0x5;


        /// <summary>
        /// The graphical style of the edges of this surface element.
        /// </summary>
        public TerrainEdgeStyle EdgeStyle => (TerrainEdgeStyle)element.slot0x6;


        /* 0x1 = Slope
         * 0x2 = WaterHeight
         * 0x3 = GrassLength
         * 0x4 = Ownership
         * 0x5 = SurfaceStyle
         * 0x6 = EdgeStyle
         */
        readonly TileElement element;


        /// <summary>
        /// Wraps the tile element to access the surface level information.
        /// </summary>
        public SurfaceElement(in TileElement element)
        {
            this.element = element;
        }
    }
}
