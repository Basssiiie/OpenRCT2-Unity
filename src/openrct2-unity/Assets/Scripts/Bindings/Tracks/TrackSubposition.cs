#nullable enable

namespace OpenRCT2.Bindings.Tracks
{
    public readonly struct TrackSubposition
    {
        public readonly short x;
        public readonly short y;
        public readonly short z;
        public readonly byte direction; // 0-31 to indicate direction, 0 = negative x axis direction
        public readonly byte pitch; // or specific vehicle sprite for corkscrew
        public readonly byte banking;


        /// <summary>
        /// Returns true if both track nodes have equal rotation bytes.
        /// </summary>
        public static bool HasEqualRotation(in TrackSubposition left, in TrackSubposition right)
            => (left.direction == right.direction)
            && (left.banking == right.banking)
            && (left.pitch == right.pitch);
    }
}
