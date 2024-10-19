using System.Collections.Generic;
using OpenRCT2.Generators.Map.Data;
using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours.Generators.Objects
{
    /// <summary>
    /// Object with all the options required to generate rides.
    /// </summary>
    public class RideOptionsObject : ScriptableObject
    {
        // The default track piece generation options.
        [SerializeField, Required] TrackPieceOptions _defaultTrackOptions;


        // Specific overrides and exceptions to the default track piece options.
        [SerializeField, Required] Dictionary<int, TrackPieceOptions>? _trackOptionsOverrides;


        public RideOptions GetOptions()
        {
            Assert.IsNotNull(_defaultTrackOptions, nameof(_defaultTrackOptions));
            Assert.IsNotNull(_trackOptionsOverrides, nameof(_trackOptionsOverrides));

            return new RideOptions(_defaultTrackOptions, _trackOptionsOverrides);
        }
    }
}
