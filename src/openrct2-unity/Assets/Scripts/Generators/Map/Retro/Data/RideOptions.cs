using System.Collections.Generic;

#nullable enable

namespace OpenRCT2.Generators.Map.Retro.Data
{
    /// <summary>
    /// Object with all the options required to generate rides.
    /// </summary>
    public readonly struct RideOptions
    {
        /// <summary>
        /// The default track piece generation options.
        /// </summary>
        public readonly TrackPieceOptions defaultTrackOptions;


        /// <summary>
        /// Specific overrides and exceptions to the default track piece options.
        /// </summary>
        public readonly IReadOnlyDictionary<int, TrackPieceOptions> trackOptionsOverrides;


        public RideOptions(TrackPieceOptions defaultTrackOptions, IReadOnlyDictionary<int, TrackPieceOptions> trackOptionsOverrides)
        {
            this.defaultTrackOptions = defaultTrackOptions;
            this.trackOptionsOverrides = trackOptionsOverrides;
        }


        /// <summary>
        /// Returns the options for the specified track piece.
        /// </summary>
        public TrackPieceOptions FindTrackPieceOptions(int trackPiece)
        {
            if (trackOptionsOverrides.TryGetValue(trackPiece, out TrackPieceOptions options))
            {
                return options;
            }
            return defaultTrackOptions;
        }
    }
}
