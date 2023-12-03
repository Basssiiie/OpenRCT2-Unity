using System.Collections.Generic;
using OpenRCT2.Bindings.Tracks;
using OpenRCT2.Generators.Extensions;
using OpenRCT2.Generators.MeshBuilding;
using UnityEngine;

#nullable enable

#if UNITY_EDITOR
namespace OpenRCT2.Generators.Tracks
{
    /// <summary>
    /// Helper behaviour that draws gizmos for the track nodes on the attached
    /// track piece.
    /// </summary>
    public class TrackGizmosDrawer : MonoBehaviour
    {
        // The track type to render.
        public int trackType;

        [SerializeField] bool _selectOnly = true;

        [Header("Colours")]
        // The nodes as they come from RCT2 import.
        [SerializeField] Color _rct2Nodes = Color.white;
        // The points after smoothening.
        [SerializeField] Color _routePosition = Color.cyan;
        // The rotation of each point after smoothening.
        [SerializeField] Color _routeRotation = Color.blue;
        // The lerp points that are used for calculating the smooth rotation.
        [SerializeField] Color _rotationLerpPoints = Color.yellow;
        // The zones with equal rotation values, which means every node in this zone uses the same sprite.
        [SerializeField] Color _rotationZones = Color.red;


        TrackSubposition[]? _tracknodes;
        TrackPiece _piece;
        int _selectedTrackType;


        /// <summary>
        /// Updates the gizmos to the specified track piece.
        /// </summary>
        void UpdateTrackInformation()
        {
            if (trackType == _selectedTrackType)
                return;

            _selectedTrackType = trackType;

            _piece = TrackFactory.GetTrackPiece(trackType);
            _tracknodes = TrackDataRegistry.GetSubpositions((ushort)trackType, (ushort)_piece.Points.Length);
        }


        /// <summary>
        /// Draws all track gizmos.
        /// </summary>
        void DrawTrackGizmos()
        {
            if (_tracknodes == null)
                return;

            Gizmos.matrix = transform.localToWorldMatrix;

            // Nodes position + rotation
            Gizmos.color = _rct2Nodes;

            for (int i = 0; i < _tracknodes.Length; i++)
            {
                TrackSubposition node = _tracknodes[i];
                Gizmos.DrawRay(node.GetLocalPosition(), (node.GetLocalRotation() * new Vector3(0, 0.1f)));

                if (i > 0)
                {
                    Gizmos.DrawLine(_tracknodes[i - 1].GetLocalPosition(), _tracknodes[i].GetLocalPosition());
                }
            }

            // Points position + rotation
            for (int i = 0; i < _piece.Points.Length; i++)
            {
                TransformPoint point = _piece.Points[i];

                Gizmos.color = _routeRotation;
                Gizmos.DrawRay(point.Position, (point.Rotation * new Vector3(0, 0.2f)));

                if (i > 0)
                {
                    Gizmos.color = _routePosition;
                    Gizmos.DrawLine(_piece.Points[i - 1].Position, point.Position);
                }
            }

            // Rotation lerp points
            Gizmos.color = _rotationLerpPoints;

            int chunkStart = 0, len = _tracknodes.Length;
            List<int> lerpNodes = new List<int> { 0 };
            for (int idx = 0; idx < len; idx++)
            {
                if (TrackSubposition.HasEqualRotation(_tracknodes[chunkStart], _tracknodes[idx]))
                    continue;

                Gizmos.DrawRay(_tracknodes[idx].GetLocalPosition(), _tracknodes[idx].GetLocalRotation() * Vector3.up * 0.3f);

                lerpNodes.Add(chunkStart + (idx - chunkStart) / 2);
                chunkStart = idx;
            }
            lerpNodes.Add(len - 1);

            // Rotation zones
            Gizmos.color = _rotationZones;

            foreach (int index in lerpNodes)
            {
                TrackSubposition current = _tracknodes[index];

                Gizmos.DrawRay(current.GetLocalPosition(), current.GetLocalRotation() * Vector3.up * 0.3f);
            }
        }


        #region Unity events

        void Start()
            => UpdateTrackInformation();


        void OnValidate()
            => UpdateTrackInformation();


        void OnDrawGizmos()
        {
            if (!_selectOnly)
                DrawTrackGizmos();
        }


        void OnDrawGizmosSelected()
        {
            if (_selectOnly)
                DrawTrackGizmos();
        }

        #endregion
    }
}
#endif
