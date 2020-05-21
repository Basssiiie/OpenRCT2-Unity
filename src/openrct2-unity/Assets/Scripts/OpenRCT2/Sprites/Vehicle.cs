using System.Runtime.InteropServices;
using UnityEngine;

namespace Lib
{
    /// <summary>
    /// The struct of a ride vehicle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 17)]
    public struct Vehicle : ISprite
    {
        public ushort idx;
        public int x;
        public int y;
        public int z;
        public byte direction; // 0-31 to indicate direction, 0 = negative x axis direction
        public byte bankRotation;
        public byte pitchRotation; // this is a index describing what sprite should be used; maybe useless for pitch?


        /// <summary>
        /// Returns an id of the vehicle, currently based on the sprite index.
        /// </summary>
        public ushort Id
            => idx;


        /// <summary>
        /// Returns the vehicle's position in Unity coordinates.
        /// </summary>
        public Vector3 Position
            => Map.CoordsToVector3(x, z, y);


        /// <summary>
        /// Gets the vehicles rotation as a quaternion.
        /// </summary>
        public Quaternion Rotation
            => Quaternion.Euler(
                0,
                ((360f / 32f) * direction) + 270f,
                GetBankingRotationAngle()
            );


        /// <summary>
        /// Gets the banking rotation angle.
        /// </summary>
        float GetBankingRotationAngle()
        {
            switch (bankRotation)
            {
                case 1: return 22.5f;
                case 2: return 45f;
                case 3: return -22.5f;
                case 4: return -45f;
                default: return 0;
            }
        }
    }
}
