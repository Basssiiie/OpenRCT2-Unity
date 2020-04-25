using System.Runtime.InteropServices;

namespace OpenRCT2.Unity
{
    /// <summary>
    /// A small scenery entry struct.
    /// It's explicit because it seems C# likes to swap members around for performance?
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = (26 + Ptr.Size))]
    public struct SmallSceneryEntry
    {
        /// <summary>
        /// The flags of this small scenery entry.
        /// </summary>
        [FieldOffset(0x06)] public SmallSceneryFlags Flags;


        /* Memory map:
         * 0x00: (base) Name
         * 0x01: (base) Name
         * 0x02: (base) Image
         * 0x03: (base) Image
         * 0x04: (base) Image
         * 0x05: (base) Image
         *
         * 0x06: Flags
         * 0x07: Flags
         * 0x08: Flags
         * 0x09: Flags
         * 0x0A: Height
         * 0x0B: Tool id
         * 0x0C: Price
         * 0x0D: Price
         * 0x0E: Removal price
         * 0x0F: Removal price
         * 0x10: Pointer to frame offsets
         * 0x11: Animation delay
         * 0x12: Animation delay
         * 0x13: Animation mask
         * 0x14: Animation mask
         * 0x15: Number of frames
         * 0x16: Number of frames
         * 0x17: Scenery tab id
         */
    }
}
