using System.Runtime.InteropServices;
using Tracks;
using UnityEngine;

namespace Lib
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct TrackNode
    {
        public readonly short x;
        public readonly short y;
        public readonly short z;
        public readonly byte direction; // 0-31 to indicate direction, 0 = negative x axis direction
        public readonly byte vehicleSprite;
        public readonly byte bankRotation;


        // Offset to offset the node relative to the center of the start of the track, instead of the corner.
        const int trackOffset = -(Map.TileCoordsXYToCoordsXY / 2); 


        /// <summary>
        /// The local position of the track node in Unity coordinates, relative
        /// to the start of the track element.
        /// </summary>
        public Vector3 LocalPosition
            => Map.CoordsToVector3(x + trackOffset, z, y + trackOffset);


        /// <summary>
        /// The local rotation of the track node in Unity degrees.
        /// </summary>
        public Quaternion LocalRotation
            => TrackFactory.ConvertRotation(direction, bankRotation, vehicleSprite);


        /// <summary>
        /// Returns true if both track nodes have equal rotation bytes.
        /// </summary>
        public static bool HasEqualRotation(ref TrackNode left, ref TrackNode right)
            => (left.direction == right.direction)
            && (left.bankRotation == right.bankRotation)
            && (left.vehicleSprite == right.vehicleSprite);
    }
}
