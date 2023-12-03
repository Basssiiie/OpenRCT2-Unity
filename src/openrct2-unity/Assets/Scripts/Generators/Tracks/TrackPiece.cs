using OpenRCT2.Generators.MeshBuilding;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Tracks
{
    /// <summary>
    /// A complete track piece with the corrected and smoother angles.
    /// </summary>
    public readonly struct TrackPiece
    {
        /// <summary>
        /// All track the nodes in this track piece.
        /// </summary>
        public TransformPoint[] Points { get; }


        /// <summary>
        /// Creates a smoother track piece based on the given RCT2 nodes.
        /// </summary>
        public TrackPiece(TransformPoint[] points)
        {
            Points = points;
        }


        /// <summary>
        /// Gets the vehicle rotation for the specified track rotation and progress.
        /// </summary>
        public Quaternion GetVehicleRotation(byte trackRotation, ushort progress)
        {
            if (Points == null)
                return Quaternion.identity;

            int index = Mathf.Clamp(progress, 0, Points.Length - 1);
            Quaternion rot = Points[index].Rotation;

            if (trackRotation > 0)
                return (Quaternion.Euler(0, trackRotation * 90f, 0) * rot);

            return rot;
        }
    }
}
