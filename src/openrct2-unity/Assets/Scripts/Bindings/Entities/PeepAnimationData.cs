#nullable enable

using System.Runtime.InteropServices;

namespace OpenRCT2.Bindings.Entities
{
    /// <summary>
    /// The struct of data about a specific peep animation
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct PeepAnimationData
    {
        public readonly uint baseImageId;
        public readonly byte accessoryImageOffset;
        public readonly byte length;
        public readonly byte rotations;
    }
}
