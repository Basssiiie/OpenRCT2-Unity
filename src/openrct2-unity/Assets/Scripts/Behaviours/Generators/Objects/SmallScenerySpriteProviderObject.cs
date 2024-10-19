using System;
using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Generators.Map.Retro.Data;
using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map.Retro.Providers
{
    [CreateAssetMenu(menuName = ("OpenRCT2/Objects/" + nameof(SmallScenerySpriteProviderObject)))]
    public class SmallScenerySpriteProviderObject : ProviderObject<SmallSceneryInfo>
    {
        [SerializeField, Required] GameObject? _prefab;
        [SerializeField] Shader? _animationShader;
        [SerializeField] ObjectScaleMode _scaleMode;

        [ContextMenuItem("Sort Alphabetically ", nameof(SortObjectIds))]
        [SerializeField] string[] _identifiers = Array.Empty<string>();

        public override (string[] identifiers, IObjectProvider<SmallSceneryInfo> provider) GetEntries()
        {
            Assert.IsNotNull(_prefab);

            var provider = new SmallScenerySpriteObjectProvider(_prefab, _animationShader, _scaleMode);
            return (_identifiers, provider);
        }

        /// <summary>
        /// Sorts all object ids alphabetically.
        /// </summary>
        void SortObjectIds()
        {
            Array.Sort(_identifiers);
        }
    }
}
