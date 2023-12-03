using System;
using OpenRCT2.Behaviours.Generators.Objects;
using OpenRCT2.Generators.Map;
using OpenRCT2.Generators.Map.Retro;
using OpenRCT2.Generators.Map.Retro.Data;
using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours.Generators
{
    /// <summary>
    /// A generator for small scenery tile elements.
    /// </summary>
    [CreateAssetMenu(menuName = (menuPath + "Retro/" + nameof(SmallSceneryGeneratorScript)))]
    public class SmallSceneryGeneratorScript : TileElementGeneratorScript
    {
        [SerializeField] Shader? _animationShader;
        [SerializeField, Required] GameObject _defaultPrefab = null!;
        [SerializeField] ObjectScaleMode _defaultScaleMode;
        [SerializeField] PrefabEntryObject[] _prefabOverrides = Array.Empty<PrefabEntryObject>();


        public override ITileElementGenerator CreateGenerator()
        {
            var prefabs = _prefabOverrides;
            var length = prefabs.Length;
            var entries = new ObjectEntry[length];

            for ( var i = 0; i < length; i++)
            {
                entries[i] = prefabs[i].GetObjectEntry();
            }

            return new SmallSceneryGenerator(_animationShader, _defaultPrefab, _defaultScaleMode, entries);
        }
    }
}
