using UnityEngine;

#nullable enable

namespace Sprites
{
    /// <summary>
    /// Generic sprite information.
    /// </summary>
    public interface ISprite
    {
        /// <summary>
        /// Returns the sprite id.
        /// </summary>
        ushort Id { get; }


        /// <summary>
        /// Returns the position of the sprite in Unity coordinates.
        /// </summary>
        Vector3 Position { get; }
    }
}
