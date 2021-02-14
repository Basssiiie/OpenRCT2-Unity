using System.Collections.Generic;
using UnityEngine;
using Utilities;

#nullable enable

namespace Generation.Retro
{
    /// <summary>
    /// Object with all the options required to generate rides.
    /// </summary>
    public class RideOptions : ScriptableObject
    {
        // The default track piece generation options.
        [SerializeField, Required] TrackPieceOptions? _defaultTrackOptions;


        // Specific overrides and exceptions to the default track piece options.
        [SerializeField, Required] Dictionary<int, TrackPieceOptions>? _trackOptionsOverrides;


        /// <summary>
        /// Returns the options for the specified track piece.
        /// </summary>
        public TrackPieceOptions FindTrackPieceOptions(int trackPiece)
        {
            Assert.IsNotNull(_defaultTrackOptions, nameof(_defaultTrackOptions));
            Assert.IsNotNull(_trackOptionsOverrides, nameof(_trackOptionsOverrides));

            if (_trackOptionsOverrides.TryGetValue(trackPiece, out TrackPieceOptions options))
            {
                return options;
            }
            return _defaultTrackOptions;
        }
    }
}
