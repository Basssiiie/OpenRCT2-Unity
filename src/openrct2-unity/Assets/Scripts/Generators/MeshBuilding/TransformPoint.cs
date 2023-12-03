using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.MeshBuilding
{
    /// <summary>
    /// A position and a rotation in one.
    /// </summary>
    public readonly struct TransformPoint
    {
        /// <summary>
        /// The position of this transform point.
        /// </summary>
        public Vector3 Position { get; }


        /// <summary>
        /// The rotation of this transform point.
        /// </summary>
        public Quaternion Rotation { get; }


        /// <summary>
        /// The rotation as a normal.
        /// </summary>
        public Vector3 Normal
            => Rotation * Vector3.up;


        /// <summary>
        /// Create a new transform point with a position and a rotation.
        /// </summary>
        public TransformPoint(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}
