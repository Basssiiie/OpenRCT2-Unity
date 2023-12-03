using OpenRCT2.Bindings.Tracks;
using OpenRCT2.Generators.Tracks;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Extensions
{
    public static class TrackSubpositionExtensions
    {
        const int _trackOffset = -(World.TileCoordsXYToCoordsXY / 2);


        /// <summary>
        /// The local position of the track node in Unity coordinates, relative
        /// to the start of the track element.
        /// </summary>
        public static Vector3 GetLocalPosition(this in TrackSubposition track)
            => World.CoordsToVector3(track.x + _trackOffset, track.z, track.y + _trackOffset);


        /// <summary>
        /// The local rotation of the track node in Unity degrees.
        /// </summary>
        public static Quaternion GetLocalRotation(this in TrackSubposition track)
            => TrackDataFactory.ConvertRotation(track.direction, track.banking, track.pitch);
    }
}
