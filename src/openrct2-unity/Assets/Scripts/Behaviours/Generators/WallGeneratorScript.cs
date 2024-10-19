using OpenRCT2.Generators.Map;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours.Generators
{
    /// <summary>
    /// A generator for wall elements.
    /// </summary>
    [CreateAssetMenu(menuName = (menuPath + nameof(WallGeneratorScript)))]
    public class WallGeneratorScript : TileElementGeneratorScript
    {
        [SerializeField] GameObject? _prefab;
        [SerializeField] string? _textureField = "Wall";


        public override ITileElementGenerator CreateGenerator()
        {
            return new WallGenerator(_prefab, _textureField);
        }
    }
}
