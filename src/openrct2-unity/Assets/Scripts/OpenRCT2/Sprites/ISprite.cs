using UnityEngine;

namespace OpenRCT2.Unity
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
