using System.Runtime.InteropServices;
using UnityEngine;

namespace OpenRCT2.Unity
{
    /// <summary>
    /// The struct of a ride vehicle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 234)]
    public struct Vehicle : ISprite
    {
        public SpriteBase sprite;
        public byte spriteType;
        public byte bankRotation;
        public int remainingDistance;
        public int velocity;
        public int acceleration;
        public ushort rideId;
        public byte vehicleType;
        public byte colourBody;
        public byte colourTrim;
        public ushort trackProgress; // this memory slot is a union with 'var_34' and 'var_35'.
        public short trackDirectionOrType; // direction = first 2 bits, type = next 8 bits.
        public int trackLocationX;
        public int trackLocationY;
        public int trackLocationZ;

        // ...


        /// <summary>
        /// Returns an id of the vehicle, currently based on the sprite index.
        /// </summary>
        public ushort Id
            => sprite.spriteIndex;


        /// <summary>
        /// Returns the vehicle's position in RCT2 coordinates.
        /// </summary>
        public Vector3 Position
            => new Vector3(sprite.x, sprite.z, sprite.y);
    }
}
