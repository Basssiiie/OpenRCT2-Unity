using UnityEngine;

#nullable enable

namespace Lib
{
    /// <summary>
    /// The map of the park.
    /// </summary>
    public partial class Map
    {
        /// <summary>
        /// The amount of park tiles per Unity meter. Used for converting RCT2 TileCoords to
        /// Unity Vector3 positions.
        /// </summary>
        public const float TileCoordsXYMultiplier = 1;

        /// <summary>
        /// Used for converting RCT2 TileCoords height units to Unity Vector3 positions.
        /// </summary>
        public const float TileCoordsZMultiplier = 0.2041f;

        /// <summary>
        /// The amount of RCT2 coordinate units per park tile. Used for converting
        /// RCT2 TileCoords to RCT2 Coords.
        /// </summary>
        public const int TileCoordsXYToCoordsXY = 32;

        /// <summary>
        /// The amount of RCT2 coordinate units per height unit. Used for converting
        /// RCT2 TileCoords to RCT2 Coords.
        /// </summary>
        public const int TileCoordsZToCoordsZ = 8;

        /// <summary>
        /// Value that is used for converting RCT2 Coords to Unity Vector3 positions.
        /// </summary>
        public const float CoordsXYMultiplier = (TileCoordsXYMultiplier / TileCoordsXYToCoordsXY);

        /// <summary>
        /// Value that is used for converting RCT2 Coords to an Unity Vector3 height.
        /// </summary>
        public const float CoordsZMultiplier = (TileCoordsZMultiplier / TileCoordsZToCoordsZ);

        /// <summary>
        /// Amount of RCT2 coordinate units there are in a single surface tile height difference.
        /// </summary>
        public const int TileHeightStep = 2;

        /// <summary>
        /// Roughly the amount of drawn pixels there are per Unity units.
        /// </summary>
        public const float PixelPerUnitMultiplier = 0.022f;


        /// <summary>
        /// Transforms a OpenRCT2 TileCoords to the Unity coordinate system. Returns a
        /// position at the center of the tile.
        /// </summary>
        public static Vector3 TileCoordsToUnity(float x, float y, float height)
        {
            float halftile = TileCoordsXYMultiplier / 2f;

            return new Vector3(
                (x * TileCoordsXYMultiplier) + halftile,
                (height * TileCoordsZMultiplier),
                (y * TileCoordsXYMultiplier) + halftile
            );
        }


        /// <summary>
        /// Transforms a OpenRCT2 Coords to the Unity coordinate system.
        /// </summary>
        public static Vector3 CoordsToVector3(float x, float y, float z)
        {
            return new Vector3(
                (x * CoordsXYMultiplier),
                (y * CoordsZMultiplier),
                (z * CoordsXYMultiplier)
            );
        }


        /// <summary>
        /// Transforms a OpenRCT2 Coords to the Unity coordinate system.
        /// </summary>
        public static Vector3 CoordsToVector3(Vector3 position)
            => CoordsToVector3(position.x, position.y, position.z);
    }
}
