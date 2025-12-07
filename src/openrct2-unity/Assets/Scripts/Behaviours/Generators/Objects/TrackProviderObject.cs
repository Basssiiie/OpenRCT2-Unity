using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map.Providers
{
    [CreateAssetMenu(menuName = ("OpenRCT2/Objects/" + nameof(TrackProviderObject)))]
    public class TrackProviderObject : ProviderObject<TrackInfo>
    {
        [SerializeField, Required] string identifier = string.Empty;
        [SerializeField, Required] GameObject track = null!;
        [SerializeField] KeyValuePair<int, GameObject>[] special = Array.Empty<KeyValuePair<int, GameObject>>();

        public override (string identifier, IObjectProvider<TrackInfo> provider)[] GetEntries()
        {
            return new (string, IObjectProvider<TrackInfo>)[]
            {
                (identifier, new TrackObjectProvider(track, special))
            };
        }
    }
}
