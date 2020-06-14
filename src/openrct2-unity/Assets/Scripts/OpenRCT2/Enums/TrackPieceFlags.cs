using System;

namespace Lib
{
    /// <summary>
    /// Flags that can be set for a specific track piece type.
    /// </summary>
    [Flags]
    public enum TrackTypeFlags : ushort
    {
        /// <summary>
        /// No track type has this flag.
        /// </summary>
        OnlyUnderwater = (1 << 0),

        TurnLeft = (1 << 1),
        TurnRight = (1 << 2),
        TurnBanked = (1 << 3),
        TurnSloped = (1 << 4),
        Down = (1 << 5),
        Up = (1 << 6),

        NormalToInversion = (1 << 7),
        IsGolfHole = (1 << 7),
        StartsAtHalfHeight = (1 << 8),
        OnlyAboveGround = (1 << 9),

        /// <summary>
        /// Used to allow steep backwards lifts on roller coasters that do
        /// not allow steep forward lift hills.
        /// </summary>
        IsSteepUp = (1 << 10),

        Helix = (1 << 11),
        AllowLiftHill = (1 << 12),
        CurveAllowsLift = (1 << 13),
        InversionToNormal = (1 << 14),

        /// <summary>
        /// Also set on Spinning Tunnel and Log Flume reverser, probably to save a flag.
        /// </summary>
        Banked = (1 << 15), 
    }
}
