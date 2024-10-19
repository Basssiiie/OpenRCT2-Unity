using System.Runtime.InteropServices;

#nullable enable

namespace OpenRCT2.Bindings.Entities
{
    /// <summary>
    /// The struct of a peep, which can be either a guest or a staff member.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public readonly struct PeepEntity
    {
        public readonly int x;
        public readonly int y;
        public readonly int z;
        public readonly byte direction;

        public readonly byte tshirtColour;
        public readonly byte trousersColour;
        public readonly byte accessoryColour;

        public readonly byte animationGroup;
        public readonly byte animationType;
        public readonly byte animationOffset;
    }
}
