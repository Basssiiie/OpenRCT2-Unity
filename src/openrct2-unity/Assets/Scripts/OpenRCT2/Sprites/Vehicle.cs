using System.Runtime.InteropServices;
using Generation.Retro;
using Sprites;
using Tracks;
using UnityEngine;

namespace Lib
{
    /// <summary>
    /// The struct of a ride vehicle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Vehicle : ISprite
    {
        public ushort idx;
        public int x;
        public int y;
        public int z;
        public byte direction; // 0-31 to indicate direction, 0 = negative x axis direction
        public byte bankRotation;
        public byte vehicleSprite; // this is a index describing what sprite should be used; maybe useless for pitch?

        public byte trackType; // current track type its on.
        public byte trackDirection; // the direction this track type is in.
        public ushort trackProgress; // current track node index.


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
            => TrackFactory
                .GetTrackPiece(trackType)
                .GetVehicleRotation(trackDirection, trackProgress);
    }
}
