using System.Runtime.InteropServices;

#nullable enable

namespace OpenRCT2.Bindings.Entities
{
    /// <summary>
    /// The struct of a ride vehicle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public readonly struct VehicleEntity
    {
        public readonly int x;
        public readonly int y;
        public readonly int z;
        public readonly byte direction; // 0-31 to indicate direction, 0 = negative x axis direction
        public readonly byte banking;
        public readonly byte pitch; // this is a index describing what sprite should be used, so not always pitch

        public readonly ushort trackType; // current track type its on.
        public readonly byte trackDirection; // the direction this track type is in.
        public readonly ushort trackProgress; // current track node index.
    }
}
