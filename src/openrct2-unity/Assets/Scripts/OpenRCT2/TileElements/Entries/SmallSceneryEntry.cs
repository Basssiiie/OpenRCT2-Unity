using System.Runtime.InteropServices;

namespace Lib
{
    /// <summary>
    /// A small scenery entry struct.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public readonly struct SmallSceneryEntry
    {
        /// <summary>
        /// The *.DAT object identifier for the entry.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public readonly string Identifier;


        /// <summary>
        /// The flags of this small scenery entry.
        /// </summary>
        public readonly SmallSceneryFlags Flags;


        public readonly ushort AnimationDelay;
        public readonly ushort AnimationFrameCount;
    }
}
