using System.Collections.Generic;
using Lib;
using MeshBuilding;
using UnityEngine;

#if UNITY_EDITOR
namespace Tracks
{
    /// <summary>
    /// Helper behaviour that draws gizmos for the track nodes on the attached
    /// track piece.
    /// </summary>
    public class TrackGizmosDrawer : MonoBehaviour
    {
        // The track type to render.
        public int trackType;

        [SerializeField] bool selectOnly = true;

        [Header("Colours")]
        // The nodes as they come from RCT2 import.
        [SerializeField] Color rct2Nodes = Color.white;
        // The points after smoothening.
        [SerializeField] Color routePosition = Color.cyan;
        // The rotation of each point after smoothening.
        [SerializeField] Color routeRotation = Color.blue;
        // The lerp points that are used for calculating the smooth rotation.
        [SerializeField] Color rotationLerpPoints = Color.yellow;
        // The zones with equal rotation values, which means every node in this zone uses the same sprite.
        [SerializeField] Color rotationZones = Color.red;


        TrackNode[] tracknodes;
        TrackPiece piece;
        int selectedTrackType;


        /// <summary>
        /// Updates the gizmos to the specified track piece.
        /// </summary>
        void UpdateTrackInformation()
        {
            if (trackType == selectedTrackType)
                return;

            selectedTrackType = trackType;

            tracknodes = OpenRCT2.GetTrackElementRoute(trackType);
            piece = TrackFactory.GetTrackPiece(trackType);
        }


        /// <summary>
        /// Draws all track gizmos.
        /// </summary>
        void DrawTrackGizmos()
        {
            if (tracknodes == null)
                return;

            Gizmos.matrix = transform.localToWorldMatrix;

            // Nodes position + rotation
            Gizmos.color = rct2Nodes;

            for (int i = 0; i < tracknodes.Length; i++)
            {
                var node = tracknodes[i];
                Gizmos.DrawRay(node.LocalPosition, (node.LocalRotation * new Vector3(0, 0.1f)));

                if (i > 0)
                    Gizmos.DrawLine(tracknodes[i - 1].LocalPosition, tracknodes[i].LocalPosition);
            }

            // Points position + rotation
            for (int i = 0; i < piece.Points.Length; i++)
            {
                TransformPoint point = piece.Points[i];

                Gizmos.color = routeRotation;
                Gizmos.DrawRay(point.Position, (point.Rotation * new Vector3(0, 0.2f)));

                if (i > 0)
                {
                    Gizmos.color = routePosition;
                    Gizmos.DrawLine(piece.Points[i - 1].Position, point.Position);
                }
            }

            // Rotation lerp points
            Gizmos.color = rotationLerpPoints;

            int chunkStart = 0, len = tracknodes.Length;
            List<int> lerpNodes = new List<int> { 0 };
            for (int idx = 0; idx < len; idx++)
            {
                if (TrackNode.HasEqualRotation(tracknodes[chunkStart], tracknodes[idx]))
                    continue;

                Gizmos.DrawRay(tracknodes[idx].LocalPosition, tracknodes[idx].LocalRotation * Vector3.up * 0.3f);

                lerpNodes.Add(chunkStart + (idx - chunkStart) / 2);
                chunkStart = idx;
            }
            lerpNodes.Add(len - 1);

            // Rotation zones
            Gizmos.color = rotationZones;

            foreach (int index in lerpNodes)
            {
                TrackNode current = tracknodes[index];

                Gizmos.DrawRay(current.LocalPosition, current.LocalRotation * Vector3.up * 0.3f);
            }
        }


        #region Unity events

        void Start()
            => UpdateTrackInformation();


        void OnValidate()
            => UpdateTrackInformation();


        void OnDrawGizmos()
        {
            if (!selectOnly)
                DrawTrackGizmos();
        }


        void OnDrawGizmosSelected()
        {
            if (selectOnly)
                DrawTrackGizmos();
        }

        #endregion
    }
}
#endif
