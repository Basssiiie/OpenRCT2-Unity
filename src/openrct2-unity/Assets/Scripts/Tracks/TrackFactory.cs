using System.Collections.Generic;
using Lib;
using MeshBuilding;
using UnityEngine;

namespace Tracks
{
    /// <summary>
    /// Factory class for track related calculations and generation.
    /// </summary>
    public static class TrackFactory
    {
        const float BankingStep = 22.5f;

        // A cache of track pieces with the track type as key.
        static readonly Dictionary<int, TrackPiece> trackPiecesCache = new Dictionary<int, TrackPiece>();


        /// <summary>
        /// Gets a track piece by track type, from cache or newly generated.
        /// </summary>
        public static TrackPiece GetTrackPiece(int trackType)
        {
            if (!trackPiecesCache.TryGetValue(trackType, out TrackPiece trackPiece))
            {
                TrackNode[] nodes = OpenRCT2.GetTrackElementRoute(trackType);
                trackPiece = CreateTrackPiece(nodes);
                trackPiecesCache.Add(trackType, trackPiece);
            }
            return trackPiece;
        }


        #region Track piece generation

        /// <summary>
        /// Creates a track piece based on the specified RCT2 nodes.
        /// </summary>
        public static TrackPiece CreateTrackPiece(TrackNode[] nodes)
        {
            int len = nodes.Length;

            TransformPoint[] points = new TransformPoint[len];
            List<int> lerpNodes = GetLerpNodes(nodes);

            float length = 0;
            for (int n = 0; n < nodes.Length; n++)
            {
                points[n] = new TransformPoint(
                    GetSmoothPosition(nodes, n, 3),
                    GetSmoothRotation(nodes, n, lerpNodes)
                );

                if (n > 0)
                    length += Vector3.Distance(points[n - 1].Position, points[n].Position);
            }

            return new TrackPiece(points);
        }


        /// <summary>
        /// Gets a smoothend position at the specified index in the nodes array.
        /// This method smoothes the position by averaging it with neighbouring nodes.
        /// </summary>
        static Vector3 GetSmoothPosition(TrackNode[] nodes, int index, int smoothRate)
        {
            // First and last nodes do not need smoothing.
            if (index == 0)
                return nodes[0].LocalPosition;

            int len = nodes.Length;
            int upper = (len - 1);

            if (index == upper)
                return nodes[upper].LocalPosition;

            // If the index + smooth rate gets out of bounds, schrink the rate for this node.
            int start = (index - smoothRate);
            if (start < 0)
            {
                // Shrink smooth rate at the edge.
                smoothRate += start;
                start = 0;
            }

            int end = (index + smoothRate);
            if (end > upper)
            {
                // Shrink the start because the end is at the edge.
                start += (end - upper);
                end = upper;
            }

            // Calculate average position over this range of nodes.
            Vector3 result = Vector3.zero;
            for (int cur = start; cur <= end; cur++)
                result += nodes[cur].LocalPosition;

            return (result / ((end - start) + 1));
        }


        /// <summary>
        /// Gets a smoothend rotation at the specified index in the nodes array.
        /// This method smoothes the rotation by lerping between the next and
        /// previous 'rotation' sections.
        /// </summary>
        static Quaternion GetSmoothRotation(TrackNode[] nodes, int index, List<int> lerpNodes)
        {
            int result = lerpNodes.BinarySearch(index);

            // If result is exactly a lerp node, just return the rotation of that node.
            if (result >= 0)
                return nodes[index].LocalRotation;

            // Get the next and previous node and lerp the rotation in between.
            result = (~result);
            int next = lerpNodes[result];
            int previous = lerpNodes[result - 1];
            float lerp = (float)(index - previous) / (next - previous);

            return Quaternion.Lerp(nodes[previous].LocalRotation, nodes[next].LocalRotation, lerp);
        }


        /// <summary>
        /// Gets all nodes indices which can be used for lerping rotations.
        /// Each index will be the center node of each 'rotation chunk'.
        /// </summary>
        static List<int> GetLerpNodes(TrackNode[] nodes)
        {
            List<int> lerpNodes = new List<int> { 0 };
            int chunkStart = 0, len = nodes.Length;

            for (int idx = 0; idx < len; idx++)
            {
                if (TrackNode.HasEqualRotation(ref nodes[chunkStart], ref nodes[idx]))
                    continue;

                lerpNodes.Add(chunkStart + (idx - chunkStart) / 2);
                chunkStart = idx;
            }

            lerpNodes.Add(len - 1);
            return lerpNodes;
        }

        #endregion


        #region RCT2 to Unity rotation conversion

        /// <summary>
        /// Gets the rotation based on the direction, bank rotation and vehicle
        /// sprite/vertical angle as bytes.
        /// </summary>
        public static Quaternion ConvertRotation(byte direction, byte bankRotation, byte vehicleSprite)
        {
            Vector3 rotation = new Vector3(
                0,
                GetDirectionAngle(direction),
                GetBankingRotationAngle(bankRotation)
            );
            ApplySpriteRotation(vehicleSprite, ref rotation);
            return Quaternion.Euler(rotation);
        }


        /// <summary>
        /// Gets the angle based on the specified direction byte.
        /// </summary>
        public static float GetDirectionAngle(byte direction)
            => ((360f / 32f) * direction) + 270f;


        /// <summary>
        /// Gets the banking rotation angle.
        /// </summary>
        public static float GetBankingRotationAngle(byte bankRotation)
        {
            switch (bankRotation)
            {
                // Flat track
                case 0:
                    return 0;

                // Right way banking
                case 1:
                case 2:
                    return (bankRotation * BankingStep);

                // Right way barrel roll
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    return ((bankRotation - 2) * BankingStep);

                // Left way banking
                case 3:
                case 4:
                    return -((bankRotation - 2) * BankingStep);

                // Left way barrel roll
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                    return -((bankRotation - 7) * BankingStep);
            }

            Debug.LogWarning($"Unknown banking rotation: {bankRotation}");
            return 0;
        }


        public static void ApplySpriteRotation(byte vehicleSprite, ref Vector3 rotation)
        {
            switch (vehicleSprite)
            {
                // Flat
                case 0: break;

                // Upwards
                case 1: rotation.x = -13f; break;
                case 2: rotation.x = -26.56f; break;
                case 3: rotation.x = -43.15f; break;
                case 4: rotation.x = -63.79f; break;

                // Downwards
                case 5: rotation.x = 13f; break;
                case 6: rotation.x = 26.56f; break;
                case 7: rotation.x = 43.15f; break;
                case 8: rotation.x = 63.79f; break;

                // Steep to vertical upwards
                case 9: rotation.x = -76.5f; break;
                case 10: rotation.x = -90f; break;

                // Vertical to inverted upwards
                case 11: rotation.x = -100.31f; break;
                case 12: rotation.x = -111.04f; break;
                case 13: rotation.x = -119.75f; break;
                case 14: rotation.x = -132.51f; break;
                case 15: rotation.x = -148.4f; break;

                // Inverted
                case 16: rotation.x = 180f; break;

                // Steep to vertical downwards
                case 17: rotation.x = 76.5f; break;
                case 18: rotation.x = 90f; break;

                // Vertical to inverted downwards
                case 19: rotation.x = 100.31f; break;
                case 20: rotation.x = 111.04f; break;
                case 21: rotation.x = 119.75f; break;
                case 22: rotation.x = 132.51f; break;
                case 23: rotation.x = 148.4f; break;

                // Spirals
                case 44: rotation.x = -2.156f; break; // upwards big
                case 45: break; // upwards small
                case 46: rotation.x = 2.156f; break; // downwards big
                case 47: break; // downwards small

                // Diagonal upwards
                case 50: rotation.x = -4.57f; break;
                case 51: rotation.x = -8.741f; break;

                // Diagional downwards
                case 53: rotation.x = 4.57f; break;
                case 54: rotation.x = 8.741f; break;
                case 55: rotation.x = 46.686f; break; // steep

                // Unknown
                default:
                    Debug.LogWarning($"Unknown track vehicle sprite: {vehicleSprite}");
                    break;
            }
        }

        #endregion
    }
}
