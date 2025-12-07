using OpenRCT2.Behaviours.Generators.Objects;
using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Generators.Map;
using OpenRCT2.Generators.Map.Providers;
using OpenRCT2.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours.Generators
{
    [CreateAssetMenu(menuName = (menuPath + nameof(TrackGeneratorScript)))]
    public class TrackGeneratorScript : TileElementGeneratorScript
    {
        [SerializeField, Required] GameObject _defaultPrefab = null!;
        [SerializeField] ProviderObject<TrackInfo>[] _providers = Array.Empty<ProviderObject<TrackInfo>>();

        public override ITileElementGenerator CreateGenerator()
        {
            var defaultProvider = new TrackObjectProvider(_defaultPrefab, Array.Empty<KeyValuePair<int, GameObject>>());
            var providers = ProviderHelper.CreateLookup(_providers);

            return new TrackGenerator(defaultProvider, providers, null!, null!);
        }
    }
}
