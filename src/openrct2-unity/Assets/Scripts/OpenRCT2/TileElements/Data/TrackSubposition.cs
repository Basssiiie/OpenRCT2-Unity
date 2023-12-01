using System.Runtime.InteropServices;
using Tracks;
using UnityEngine;

#nullable enable

namespace Lib
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct TrackSubposition
    {
        public readonly short x;
        public readonly short y;
        public readonly short z;
        public readonly byte direction; // 0-31 to indicate direction, 0 = negative x axis direction
        public readonly byte pitch; // or specific vehicle sprite for corkscrew
        public readonly byte banking;


        // Offset to offset the node relative to the center of the start of the track, instead of the corner.
        const int _trackOffset = -(Map.TileCoordsXYToCoordsXY / 2); 


        /// <summary>
        /// The local position of the track node in Unity coordinates, relative
        /// to the start of the track element.
        /// </summary>
        public Vector3 LocalPosition
            => Map.CoordsToVector3(x + _trackOffset, z, y + _trackOffset);


        /// <summary>
        /// The local rotation of the track node in Unity degrees.
        /// </summary>
        public Quaternion LocalRotation
            => TrackFactory.ConvertRotation(direction, banking, pitch);


        /// <summary>
        /// Returns true if both track nodes have equal rotation bytes.
        /// </summary>
        public static bool HasEqualRotation(in TrackSubposition left, in TrackSubposition right)
            => (left.direction == right.direction)
            && (left.banking == right.banking)
            && (left.pitch == right.pitch);
    }
}
