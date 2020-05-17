using System;
using UnityEngine;

namespace OpenRCT
{
    /// <summary>
    /// The map of the park.
    /// </summary>
    public partial class Map
    {
        public const int TileCoordsToCoords = 32;
        public const float TileCoordsToVector3Multiplier = 1f;
        public const float CoordsToVector3Multiplier = TileCoordsToVector3Multiplier / TileCoordsToCoords;

        public const float TileHeightMultiplier = 0.25f;
        public const int TileHeightStep = 2;

        public const float PixelPerUnitMultiplier = 0.022f;


        /// <summary>
        /// Transforms a OpenRCT2 TileCoords to the Unity coordination system.
        /// </summary>
        public static Vector3 TileCoordsToUnity(float x, float y, float z)
        {
            float halftile = TileCoordsToVector3Multiplier / 2f;

            return new Vector3(
                (x * TileCoordsToVector3Multiplier) + halftile,
                y * TileHeightMultiplier,
                (z * TileCoordsToVector3Multiplier) + halftile
            );
        }


        /// <summary>
        /// Transforms a OpenRCT2 Coords to the Unity coordination system.
        /// </summary>
        public static Vector3 CoordsToVector3(float x, float y, float z)
        {
            return new Vector3(
                (x * CoordsToVector3Multiplier),
                y * CoordsToVector3Multiplier,
                (z * CoordsToVector3Multiplier)
            );
        }


        /// <summary>
        /// Transforms a OpenRCT2 Coords to the Unity coordination system.
        /// </summary>
        public static Vector3 CoordsToVector3(Vector3 position)
            => CoordsToVector3(position.x, position.y, position.z);
    }
}
