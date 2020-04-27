using System;

namespace OpenRCT2.Unity
{
    /// <summary>
    /// Flags that can be set on a small scenery.
    /// </summary>
    [Flags]
    public enum SmallSceneryFlags : uint
    {
        None = 0,

        FullTile = (1 << 0),            // 0x1
        VOffsetCentre = (1 << 1),       // 0x2
        RequireFlatSurface = (1 << 2),  // 0x4
        Rotatable = (1 << 3),           // 0x8; when set, user can set rotation, otherwise rotation is automatic
        Animated = (1 << 4),            // 0x10
        CanWither = (1 << 5),           // 0x20
        CanBeWatered = (1 << 6),        // 0x40
        AnimatedForeground = (1 << 7),  // 0x80

        Diagonal = (1 << 8),            // 0x100
        HasGlass = (1 << 9),            // 0x200
        HasPrimaryColour = (1 << 10),   // 0x400
        FountainSpray1 = (1 << 11),     // 0x800
        FountainSpray4 = (1 << 12),     // 0x1000
        IsClock = (1 << 13),            // 0x2000
        SwampGoo = (1 << 14),           // 0x4000
        HasFrameOffsets = (1 << 15),    // 0x8000

        Flag17 = (1 << 16),             // 0x10000
        Stackable = (1 << 17),          // 0x20000; means scenery item can be placed in the air and over water
        NoWalls = (1 << 18),            // 0x40000
        HasSecondaryColour = (1 << 19), // 0x80000
        NoSupports = (1 << 20),         // 0x100000
        VisibleWhenZoomed = (1 << 21),  // 0x200000
        Cog = (1 << 22),                // 0x400000
        BuildDirectlyOnTop = (1 << 23), // 0x800000; means supports can be built on this object. Used for base blocks.

        HalfSpace = (1 << 24),          // 0x1000000
        ThreeQuarters = (1 << 25),      // 0x2000000
        PaintSupports = (1 << 26),      // 0x4000000; used for scenery items which are support structures
        Flag27 = (1 << 27),             // 0x8000000

        IsTree = (1 << 28), // Added by OpenRCT2
    }
}
