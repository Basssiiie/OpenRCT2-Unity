using System.Runtime.InteropServices;
using UnityEngine;

namespace Lib
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TrackNode
    {
        public short x;
        public short y;
        public short z;
        public byte direction; // 0-31 to indicate direction, 0 = negative x axis direction
        public byte vehicleSprite;
        public byte bankRotation;


        /// <summary>
        /// The local position of the track node in Unity coordinates, relative
        /// to the start of the track element.
        /// </summary>
        public Vector3 LocalPosition
            => Map.CoordsToVector3(x, z, y);
    }
}
