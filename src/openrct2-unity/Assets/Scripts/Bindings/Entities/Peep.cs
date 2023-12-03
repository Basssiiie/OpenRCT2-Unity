#nullable enable

using System.Runtime.InteropServices;

namespace OpenRCT2.Bindings.Entities
{
    /// <summary>
    /// The struct of a peep, which can be either a guest or a staff member.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public readonly struct Peep
    {
        public readonly ushort id;
        public readonly int x;
        public readonly int y;
        public readonly int z;
        public readonly byte direction;
        public readonly uint imageId;
    }
}
