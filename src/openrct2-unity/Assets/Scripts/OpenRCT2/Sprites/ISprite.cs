using UnityEngine;

namespace OpenRCT
{
    /// <summary>
    /// Generic sprite information.
    /// </summary>
    public interface ISprite
    {
        ushort Id { get; }
        Vector3 Position { get; }
    }
}
