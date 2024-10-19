using System;
using OpenRCT2.Behaviours.Generators.Objects;
using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Generators.Map;
using OpenRCT2.Generators.Map.Data;
using OpenRCT2.Generators.Map.Providers;
using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours.Generators
{
    /// <summary>
    /// A generator for small scenery tile elements.
    /// </summary>
    [CreateAssetMenu(menuName = (menuPath + nameof(SmallSceneryGeneratorScript)))]
    public class SmallSceneryGeneratorScript : TileElementGeneratorScript
    {
        [SerializeField] Shader? _animationShader;
        [SerializeField, Required] GameObject _defaultPrefab = null!;
        [SerializeField] ObjectScaleMode _defaultScaleMode;

        [SerializeField] ProviderObject<SmallSceneryInfo>[] _providers = Array.Empty<ProviderObject<SmallSceneryInfo>>();

        public override ITileElementGenerator CreateGenerator()
        {
            var defaultProvider = new SmallScenerySpriteObjectProvider(_defaultPrefab, _animationShader, _defaultScaleMode);
            var providers = ProviderHelper.CreateLookup(_providers);

            return new SmallSceneryGenerator(defaultProvider, providers);
        }
    }
}
