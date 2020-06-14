using System;

namespace Lib
{
    /// <summary>
    /// Flags for a specific track tile element.
    /// </summary>
    [Flags]
    public enum TrackElementFlags : byte
    {
        /// <summary>
        /// Whether the track element has a regular chain lift.
        /// </summary>
        HasChainlift = (1 << 0),

        /// <summary>
        /// Whether the track element is inverted.
        /// </summary>
        IsInverted = (1 << 1),

        /// <summary>
        /// Whether the track element has a cable lift (Giga Coaster only).
        /// </summary>
        HasCableLift = (1 << 2),

        // ?
        Highlight = (1 << 3),

        /// <summary>
        /// Whether to draw this tile piece with a green light or not (otherwise
        /// often red). Used in stations.
        /// </summary>
        HasGreenLight = (1 << 4),

        // ?
        BlockBrakeClosed = (1 << 5),

        /// <summary>
        /// The player cannot remove this track element.
        /// </summary>
        Indestructable = (1 << 5),
    }
}
