using System.Collections.Generic;
using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Bindings.Tracks;
using OpenRCT2.Generators.Extensions;
using OpenRCT2.Generators.MeshBuilding;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Tracks
{
    /// <summary>
    /// Factory class for track related calculations and generation.
    /// </summary>
    public static class TrackFactory
    {
        // A cache of track pieces with the track type as key.
        static readonly Dictionary<int, TrackPiece> _trackPiecesCache = new Dictionary<int, TrackPiece>();


        /// <summary>
        /// Gets a track piece by track type or throws if not found.
        /// </summary>
        public static TrackPiece GetTrackPiece(int trackType)
        {
            return _trackPiecesCache[trackType];
        }

        /// <summary>
        /// Gets a track piece by track type, from cache or newly generated.
        /// </summary>
        public static TrackPiece GetOrCreateTrackPiece(in TrackInfo track)
        {
            ushort type = track.trackType;

            if (!_trackPiecesCache.TryGetValue(type, out TrackPiece trackPiece))
            {
                TrackSubposition[] nodes = TrackDataRegistry.GetSubpositions(type, track.trackLength);
                trackPiece = CreateTrackPiece(nodes);
                _trackPiecesCache.Add(type, trackPiece);
            }
            return trackPiece;
        }


        #region Track piece generation

        /// <summary>
        /// Creates a track piece based on the specified RCT2 nodes.
        /// </summary>
        static TrackPiece CreateTrackPiece(TrackSubposition[] nodes)
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
                {
                    length += Vector3.Distance(points[n - 1].Position, points[n].Position);
                }
            }

            return new TrackPiece(points);
        }


        /// <summary>
        /// Gets a smoothend position at the specified index in the nodes array.
        /// This method smoothes the position by averaging it with neighbouring nodes.
        /// </summary>
        static Vector3 GetSmoothPosition(TrackSubposition[] nodes, int index, int smoothRate)
        {
            // First and last nodes do not need smoothing.
            if (index == 0)
            {
                return nodes[0].GetLocalPosition();
            }

            int len = nodes.Length;
            int upper = (len - 1);

            if (index == upper)
            {
                return nodes[upper].GetLocalPosition();
            }

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
            {
                result += nodes[cur].GetLocalPosition();
            }

            return (result / ((end - start) + 1));
        }


        /// <summary>
        /// Gets a smoothend rotation at the specified index in the nodes array.
        /// This method smoothes the rotation by lerping between the next and
        /// previous 'rotation' sections.
        /// </summary>
        static Quaternion GetSmoothRotation(TrackSubposition[] nodes, int index, List<int> lerpNodes)
        {
            int result = lerpNodes.BinarySearch(index);

            // If result is exactly a lerp node, just return the rotation of that node.
            if (result >= 0)
                return nodes[index].GetLocalRotation();

            // Get the next and previous node and lerp the rotation in between.
            result = (~result);
            int next = lerpNodes[result];
            int previous = lerpNodes[result - 1];
            float lerp = (float)(index - previous) / (next - previous);

            return Quaternion.Lerp(nodes[previous].GetLocalRotation(), nodes[next].GetLocalRotation(), lerp);
        }


        /// <summary>
        /// Gets all nodes indices which can be used for lerping rotations.
        /// Each index will be the center node of each 'rotation chunk'.
        /// </summary>
        static List<int> GetLerpNodes(TrackSubposition[] nodes)
        {
            var lerpNodes = new List<int> { 0 };
            int chunkStart = 0, len = nodes.Length;

            for (int idx = 0; idx < len; idx++)
            {
                if (TrackSubposition.HasEqualRotation(nodes[chunkStart], nodes[idx]))
                {
                    continue;
                }

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
            var rotation = new Vector3(
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
                case 1: // 1x
                case 2: // 2x
                    return (bankRotation * BankingStep);

                // Left way banking
                case 3: // -1x
                case 4: // -2x
                    return ((2 - bankRotation) * BankingStep);

                // Right way barrel roll
                case 5: // 3x
                case 6: // 4x
                case 7: // 5x
                case 8: // 6x
                case 9: // 7x
                    return ((bankRotation - 2) * BankingStep);

                // Left way barrel roll
                case 10: // -3x
                case 11: // -4x
                case 12: // -5x
                case 13: // -6x
                case 14: // -7x
                    return ((7 - bankRotation) * BankingStep);

                // Right way inverted barrel roll
                case 16: // 2x
                case 17: // 1x
                    return ((18 - bankRotation) * BankingStep);

                // Left way inverted barrel roll
                case 18: // -2x
                case 19: // -1x
                    return ((bankRotation - 20) * BankingStep);

            }

            Debug.LogWarning($"Unknown banking rotation: {bankRotation}");
            return 0;
        }


        /// <summary>
        /// Applies additional rotation to the vector based on the vehicle sprite index.
        /// </summary>
        public static void ApplySpriteRotation(byte vehicleSprite, ref Vector3 rot)
        {
            switch (vehicleSprite)
            {
                // Flat
                case 0: break;

                // Upwards
                case 1: rot.x = -SlopeFlatToGentle; break;
                case 2: rot.x = -SlopeGentle; break;
                case 3: rot.x = -SlopeGentleToSteep; break;
                case 4: rot.x = -SlopeSteep; break;

                // Downwards
                case 5: rot.x = SlopeFlatToGentle; break;
                case 6: rot.x = SlopeGentle; break;
                case 7: rot.x = SlopeGentleToSteep; break;
                case 8: rot.x = SlopeSteep; break;

                // Steep to vertical upwards
                case 9: rot.x = -SlopeSteepToVertical; break;
                case 10: rot.x = -SlopeVertical; break;

                // Vertical to inverted upwards
                case 11: rot.x = -SlopeInvertedStep1; break; 
                case 12: rot.x = -SlopeInvertedStep2; break; 
                case 13: rot.x = -SlopeInvertedStep3; break; 
                case 14: rot.x = -SlopeInvertedStep4; break; 
                case 15: rot.x = -SlopeInvertedStep5; break; 

                // Inverted
                case 16: rot.x = SlopeInvertedFull; break;

                // Steep to vertical downwards
                case 17: rot.x = SlopeSteepToVertical; break;
                case 18: rot.x = SlopeVertical; break;

                // Vertical to inverted downwards
                case 19: rot.x = SlopeInvertedStep1; break; 
                case 20: rot.x = SlopeInvertedStep2; break; 
                case 21: rot.x = SlopeInvertedStep3; break; 
                case 22: rot.x = SlopeInvertedStep4; break; 
                case 23: rot.x = SlopeInvertedStep5; break;

                // Corkscrew start rightward roll
                case 24: rot.x = -SlopeCorkscrew1; rot.y += RotationCorkscrew1; rot.z -= BankingCorkscrew1; break;
                case 25: rot.x = -SlopeCorkscrew2; rot.y += RotationCorkscrew2; rot.z -= BankingCorkscrew2; break;
                case 26: rot.x = -SlopeCorkscrew3; rot.y += RotationCorkscrew3; rot.z -= BankingCorkscrew3; break;
                case 27: rot.x = -SlopeCorkscrew4; rot.y += RotationCorkscrew4; rot.z -= BankingCorkscrew4; break;
                case 28: rot.x = -SlopeCorkscrew5; rot.y += RotationCorkscrew5; rot.z -= BankingCorkscrew5; break;

                // Corkscrew end rightward roll
                case 29: rot.x = SlopeCorkscrew1; rot.y += RotationCorkscrew1; rot.z += BankingCorkscrew1; break;
                case 30: rot.x = SlopeCorkscrew2; rot.y += RotationCorkscrew2; rot.z += BankingCorkscrew2; break;
                case 31: rot.x = SlopeCorkscrew3; rot.y += RotationCorkscrew3; rot.z += BankingCorkscrew3; break;
                case 32: rot.x = SlopeCorkscrew4; rot.y += RotationCorkscrew4; rot.z += BankingCorkscrew4; break;
                case 33: rot.x = SlopeCorkscrew5; rot.y += RotationCorkscrew5; rot.z += BankingCorkscrew5; break;

                // Corkscrew start leftward roll
                case 34: rot.x = -SlopeCorkscrew1; rot.y -= RotationCorkscrew1; rot.z += BankingCorkscrew1; break;
                case 35: rot.x = -SlopeCorkscrew2; rot.y -= RotationCorkscrew2; rot.z += BankingCorkscrew2; break;
                case 36: rot.x = -SlopeCorkscrew3; rot.y -= RotationCorkscrew3; rot.z += BankingCorkscrew3; break;
                case 37: rot.x = -SlopeCorkscrew4; rot.y -= RotationCorkscrew4; rot.z += BankingCorkscrew4; break;
                case 38: rot.x = -SlopeCorkscrew5; rot.y -= RotationCorkscrew5; rot.z += BankingCorkscrew5; break;

                // Corkscrew end leftward roll
                case 39: rot.x = SlopeCorkscrew1; rot.y -= RotationCorkscrew1; rot.z -= BankingCorkscrew1; break;
                case 40: rot.x = SlopeCorkscrew2; rot.y -= RotationCorkscrew2; rot.z -= BankingCorkscrew2; break;
                case 41: rot.x = SlopeCorkscrew3; rot.y -= RotationCorkscrew3; rot.z -= BankingCorkscrew3; break;
                case 42: rot.x = SlopeCorkscrew4; rot.y -= RotationCorkscrew4; rot.z -= BankingCorkscrew4; break;
                case 43: rot.x = SlopeCorkscrew5; rot.y -= RotationCorkscrew5; rot.z -= BankingCorkscrew5; break;

                // Spirals
                case 44: rot.x = -SlopeSpiralBig; break; // upwards big
                case 45: rot.x = -SlopeSpiralSmall; break; // upwards small
                case 46: rot.x = SlopeSpiralBig; break; // downwards big
                case 47: rot.x = SlopeSpiralSmall; break; // downwards small

                // Spirals that only turn 90 degrees
                case 48: rot.x = -SlopeSpiralQuarter; break; // upwards
                case 49: rot.x = SlopeSpiralQuarter; break; // downwards

                // Diagonal upwards
                case 50: rot.x = -SlopeDiagonalFlatToGentle; break;
                case 51: rot.x = -SlopeDiagonalGentle; break;
                case 52: rot.x = -SlopeDiagonalSteep; break;

                // Diagional downwards
                case 53: rot.x = SlopeDiagonalFlatToGentle; break;
                case 54: rot.x = SlopeDiagonalGentle; break;
                case 55: rot.x = SlopeDiagonalSteep; break;

                // Inverted half loop (bottom part only)
                case 56: rot.x = SlopeGentle; break;
                case 57: rot.x = SlopeGentleToSteep; break;
                case 58: rot.x = SlopeSteep; break;

                // Circular lifthil
                case 59: rot.x = -SlopeSpiralLifthill; break;

                // Unknown 
                default:
                    Debug.LogWarning($"Unknown track vehicle sprite: {vehicleSprite}");
                    break;
            }
        }


        // Thanks to:
        //  https://github.com/OpenRCT2/OpenRCT2/wiki/Sizes-and-angles-in-the-game-world
        //  https://github.com/OpenRCT2/OpenRCT2/wiki/Vehicle-Sprite-Layout

        // Banking angle constants
        const float BankingStep =               (22.5f);
        const float BankingCorkscrew1 =         (15.8f);
        const float BankingCorkscrew2 =         (34.4f);
        const float BankingCorkscrew3 =         (90f);
        const float BankingCorkscrew4 =         (180 - BankingCorkscrew2);
        const float BankingCorkscrew5 =         (180 - BankingCorkscrew1);

        // Regular slopes
        const float SlopeFlatToGentle =         (SlopeGentle / 2);
        const float SlopeGentle =               (22.2052f);
        const float SlopeGentleToSteep =        ((SlopeGentle + SlopeSteep) / 2);
        const float SlopeSteep =                (58.5148f);
        const float SlopeSteepToVertical =      ((SlopeSteep + SlopeVertical) / 2);
        const float SlopeVertical =             (90f);

        // Inverted slopes (for loopings etc.)
        const float SlopeInvertedFull =         (180f);
        const float SlopeInvertedStep1 =        (SlopeVertical + SlopeFlatToGentle);
        const float SlopeInvertedStep2 =        (SlopeVertical + SlopeGentle);
        const float SlopeInvertedStep3 =        (SlopeVertical + SlopeGentleToSteep);
        const float SlopeInvertedStep4 =        (SlopeVertical + SlopeSteep);
        const float SlopeInvertedStep5 =        (SlopeVertical + SlopeSteepToVertical);

        // Spiral slopes
        const float SlopeSpiralSmall =          (5.5266f);
        const float SlopeSpiralBig =            (3.2933f);
        const float SlopeSpiralQuarter =        (6.5366f);
        const float SlopeSpiralLifthill =       (10.8737f);

        // Diagonal slopes
        const float SlopeDiagonalFlatToGentle = (SlopeDiagonalGentle / 2);
        const float SlopeDiagonalGentle =       (16.1005f);
        const float SlopeDiagonalSteep =        (49.1035f);

        // Corkscrew slopes
        const float SlopeCorkscrew1 =           (16.4f);
        const float SlopeCorkscrew2 =           (43.3f);
        const float SlopeCorkscrew3 =           (45);
        const float SlopeCorkscrew4 =           (SlopeCorkscrew2);
        const float SlopeCorkscrew5 =           (SlopeCorkscrew1);

        // Corkscrew rotations
        const float RotationCorkscrew1 =        (2.3f); 
        const float RotationCorkscrew2 =        (14f); 
        const float RotationCorkscrew3 =        (45); 
        const float RotationCorkscrew4 =        (90 - RotationCorkscrew2); 
        const float RotationCorkscrew5 =        (90 - RotationCorkscrew1);

        #endregion
    }
}
