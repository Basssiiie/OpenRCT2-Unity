using System;
using UnityEngine;

namespace Generation.Retro
{
    /// <summary>
    /// Object with all the options required to generate rides.
    /// </summary>
    public class RideOptions : ScriptableObject
    {
        // The default track piece generation options.
        [SerializeField] TrackPieceOptions defaultTrackOptions;


        // Specific overrides and exceptions to the default track piece options.
        // TODO: this can be serialized as a dictionary in Unity 2020.1. Update once its stable.
        [SerializeField] TrackPieceOverride[] trackOptionsOverrides;


        /// <summary>
        /// Returns the options for the specified track piece.
        /// </summary>
        public TrackPieceOptions FindTrackPieceOptions(int trackPiece)
        {
            TrackPieceOverride options = Array.Find(trackOptionsOverrides, o => o.TrackPiece == trackPiece);

            if (options != null)
                return options?.Options;

            return defaultTrackOptions;
        }


        // Key-value struct for mapping a track piece number to some additional options.
        class TrackPieceOverride
        {
            public int TrackPiece;
            public TrackPieceOptions Options;
        }
    }
}
