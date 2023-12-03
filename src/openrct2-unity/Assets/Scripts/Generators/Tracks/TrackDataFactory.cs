using System.Collections.Generic;
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
    public static class TrackDataFactory
    {
        // A cache of track pieces with the track type as key.
        static readonly TrackPiece?[] _trackPiecesCache = new TrackPiece?[TrackDataRegistry.TrackTypesCount];


        /// <summary>
        /// Gets a track piece by track type, from cache or newly generated.
        /// </summary>
        public static TrackPiece GetTrackPiece(ushort trackType)
        {
            if (trackType >= _trackPiecesCache.Length)
            {
                Debug.LogError($"Invalid track type: {trackType} / {_trackPiecesCache.Length}");
                return default;
            }

            TrackPiece? piece = _trackPiecesCache[trackType];
            if (piece.HasValue)
            {
                return piece.Value;
            }

            ushort trackLength = TrackDataRegistry.GetSubpositionsLength(trackType);
            TrackPiece newPiece = CreateTrackPiece(trackType, trackLength);

            _trackPiecesCache[trackType] = newPiece;
            return newPiece;
        }


        /// <summary>
        /// Creates a track piece based on the specified RCT2 nodes.
        /// </summary>
        static TrackPiece CreateTrackPiece(ushort trackType, ushort trackLength)
        {
            TrackSubposition[] nodes = TrackDataRegistry.GetSubpositions(trackType, trackLength);
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


        #region Track piece values

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
                    return (bankRotation * _bankingStep);

                // Left way banking
                case 3: // -1x
                case 4: // -2x
                    return ((2 - bankRotation) * _bankingStep);

                // Right way barrel roll
                case 5: // 3x
                case 6: // 4x
                case 7: // 5x
                case 8: // 6x
                case 9: // 7x
                    return ((bankRotation - 2) * _bankingStep);

                // Left way barrel roll
                case 10: // -3x
                case 11: // -4x
                case 12: // -5x
                case 13: // -6x
                case 14: // -7x
                    return ((7 - bankRotation) * _bankingStep);

                // Right way inverted barrel roll
                case 16: // 2x
                case 17: // 1x
                    return ((18 - bankRotation) * _bankingStep);

                // Left way inverted barrel roll
                case 18: // -2x
                case 19: // -1x
                    return ((bankRotation - 20) * _bankingStep);

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
                case 1: rot.x = -_slopeFlatToGentle; break;
                case 2: rot.x = -_slopeGentle; break;
                case 3: rot.x = -_slopeGentleToSteep; break;
                case 4: rot.x = -_slopeSteep; break;

                // Downwards
                case 5: rot.x = _slopeFlatToGentle; break;
                case 6: rot.x = _slopeGentle; break;
                case 7: rot.x = _slopeGentleToSteep; break;
                case 8: rot.x = _slopeSteep; break;

                // Steep to vertical upwards
                case 9: rot.x = -_slopeSteepToVertical; break;
                case 10: rot.x = -_slopeVertical; break;

                // Vertical to inverted upwards
                case 11: rot.x = -_slopeInvertedStep1; break; 
                case 12: rot.x = -_slopeInvertedStep2; break; 
                case 13: rot.x = -_slopeInvertedStep3; break; 
                case 14: rot.x = -_slopeInvertedStep4; break; 
                case 15: rot.x = -_slopeInvertedStep5; break; 

                // Inverted
                case 16: rot.x = _slopeInvertedFull; break;

                // Steep to vertical downwards
                case 17: rot.x = _slopeSteepToVertical; break;
                case 18: rot.x = _slopeVertical; break;

                // Vertical to inverted downwards
                case 19: rot.x = _slopeInvertedStep1; break; 
                case 20: rot.x = _slopeInvertedStep2; break; 
                case 21: rot.x = _slopeInvertedStep3; break; 
                case 22: rot.x = _slopeInvertedStep4; break; 
                case 23: rot.x = _slopeInvertedStep5; break;

                // Corkscrew start rightward roll
                case 24: rot.x = -_slopeCorkscrew1; rot.y += _rotationCorkscrew1; rot.z -= _bankingCorkscrew1; break;
                case 25: rot.x = -_slopeCorkscrew2; rot.y += _rotationCorkscrew2; rot.z -= _bankingCorkscrew2; break;
                case 26: rot.x = -_slopeCorkscrew3; rot.y += _rotationCorkscrew3; rot.z -= _bankingCorkscrew3; break;
                case 27: rot.x = -_slopeCorkscrew4; rot.y += _rotationCorkscrew4; rot.z -= _bankingCorkscrew4; break;
                case 28: rot.x = -_slopeCorkscrew5; rot.y += _rotationCorkscrew5; rot.z -= _bankingCorkscrew5; break;

                // Corkscrew end rightward roll
                case 29: rot.x = _slopeCorkscrew1; rot.y += _rotationCorkscrew1; rot.z += _bankingCorkscrew1; break;
                case 30: rot.x = _slopeCorkscrew2; rot.y += _rotationCorkscrew2; rot.z += _bankingCorkscrew2; break;
                case 31: rot.x = _slopeCorkscrew3; rot.y += _rotationCorkscrew3; rot.z += _bankingCorkscrew3; break;
                case 32: rot.x = _slopeCorkscrew4; rot.y += _rotationCorkscrew4; rot.z += _bankingCorkscrew4; break;
                case 33: rot.x = _slopeCorkscrew5; rot.y += _rotationCorkscrew5; rot.z += _bankingCorkscrew5; break;

                // Corkscrew start leftward roll
                case 34: rot.x = -_slopeCorkscrew1; rot.y -= _rotationCorkscrew1; rot.z += _bankingCorkscrew1; break;
                case 35: rot.x = -_slopeCorkscrew2; rot.y -= _rotationCorkscrew2; rot.z += _bankingCorkscrew2; break;
                case 36: rot.x = -_slopeCorkscrew3; rot.y -= _rotationCorkscrew3; rot.z += _bankingCorkscrew3; break;
                case 37: rot.x = -_slopeCorkscrew4; rot.y -= _rotationCorkscrew4; rot.z += _bankingCorkscrew4; break;
                case 38: rot.x = -_slopeCorkscrew5; rot.y -= _rotationCorkscrew5; rot.z += _bankingCorkscrew5; break;

                // Corkscrew end leftward roll
                case 39: rot.x = _slopeCorkscrew1; rot.y -= _rotationCorkscrew1; rot.z -= _bankingCorkscrew1; break;
                case 40: rot.x = _slopeCorkscrew2; rot.y -= _rotationCorkscrew2; rot.z -= _bankingCorkscrew2; break;
                case 41: rot.x = _slopeCorkscrew3; rot.y -= _rotationCorkscrew3; rot.z -= _bankingCorkscrew3; break;
                case 42: rot.x = _slopeCorkscrew4; rot.y -= _rotationCorkscrew4; rot.z -= _bankingCorkscrew4; break;
                case 43: rot.x = _slopeCorkscrew5; rot.y -= _rotationCorkscrew5; rot.z -= _bankingCorkscrew5; break;

                // Spirals
                case 44: rot.x = -_slopeSpiralBig; break; // upwards big
                case 45: rot.x = -_slopeSpiralSmall; break; // upwards small
                case 46: rot.x = _slopeSpiralBig; break; // downwards big
                case 47: rot.x = _slopeSpiralSmall; break; // downwards small

                // Spirals that only turn 90 degrees
                case 48: rot.x = -_slopeSpiralQuarter; break; // upwards
                case 49: rot.x = _slopeSpiralQuarter; break; // downwards

                // Diagonal upwards
                case 50: rot.x = -_slopeDiagonalFlatToGentle; break;
                case 51: rot.x = -_slopeDiagonalGentle; break;
                case 52: rot.x = -_slopeDiagonalSteep; break;

                // Diagional downwards
                case 53: rot.x = _slopeDiagonalFlatToGentle; break;
                case 54: rot.x = _slopeDiagonalGentle; break;
                case 55: rot.x = _slopeDiagonalSteep; break;

                // Inverted half loop (bottom part only)
                case 56: rot.x = _slopeGentle; break;
                case 57: rot.x = _slopeGentleToSteep; break;
                case 58: rot.x = _slopeSteep; break;

                // Circular lifthil
                case 59: rot.x = -_slopeSpiralLifthill; break;

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
        const float _bankingStep =               (22.5f);
        const float _bankingCorkscrew1 =         (15.8f);
        const float _bankingCorkscrew2 =         (34.4f);
        const float _bankingCorkscrew3 =         (90f);
        const float _bankingCorkscrew4 =         (180 - _bankingCorkscrew2);
        const float _bankingCorkscrew5 =         (180 - _bankingCorkscrew1);

        // Regular slopes
        const float _slopeFlatToGentle =         (_slopeGentle / 2);
        const float _slopeGentle =               (22.2052f);
        const float _slopeGentleToSteep =        ((_slopeGentle + _slopeSteep) / 2);
        const float _slopeSteep =                (58.5148f);
        const float _slopeSteepToVertical =      ((_slopeSteep + _slopeVertical) / 2);
        const float _slopeVertical =             (90f);

        // Inverted slopes (for loopings etc.)
        const float _slopeInvertedFull =         (180f);
        const float _slopeInvertedStep1 =        (_slopeVertical + _slopeFlatToGentle);
        const float _slopeInvertedStep2 =        (_slopeVertical + _slopeGentle);
        const float _slopeInvertedStep3 =        (_slopeVertical + _slopeGentleToSteep);
        const float _slopeInvertedStep4 =        (_slopeVertical + _slopeSteep);
        const float _slopeInvertedStep5 =        (_slopeVertical + _slopeSteepToVertical);

        // Spiral slopes
        const float _slopeSpiralSmall =          (5.5266f);
        const float _slopeSpiralBig =            (3.2933f);
        const float _slopeSpiralQuarter =        (6.5366f);
        const float _slopeSpiralLifthill =       (10.8737f);

        // Diagonal slopes
        const float _slopeDiagonalFlatToGentle = (_slopeDiagonalGentle / 2);
        const float _slopeDiagonalGentle =       (16.1005f);
        const float _slopeDiagonalSteep =        (49.1035f);

        // Corkscrew slopes
        const float _slopeCorkscrew1 =           (16.4f);
        const float _slopeCorkscrew2 =           (43.3f);
        const float _slopeCorkscrew3 =           (45);
        const float _slopeCorkscrew4 =           (_slopeCorkscrew2);
        const float _slopeCorkscrew5 =           (_slopeCorkscrew1);

        // Corkscrew rotations
        const float _rotationCorkscrew1 =        (2.3f); 
        const float _rotationCorkscrew2 =        (14f); 
        const float _rotationCorkscrew3 =        (45); 
        const float _rotationCorkscrew4 =        (90 - _rotationCorkscrew2); 
        const float _rotationCorkscrew5 =        (90 - _rotationCorkscrew1);

        #endregion
    }
}
