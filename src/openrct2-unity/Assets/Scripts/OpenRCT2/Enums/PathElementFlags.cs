using System;

namespace Lib
{
    /// <summary>
    /// Flags for the path element.
    /// </summary>
    [Flags]
    public enum PathElementFlags : byte
    {
        IsSloped = (1 << 0),
        HasQueueBanner = (1 << 1),
        AdditionIsGhost = (1 << 2),
        BlockedByVehicle = (1 << 3),
        AdditionIsBroken = (1 << 4)
    };
}
