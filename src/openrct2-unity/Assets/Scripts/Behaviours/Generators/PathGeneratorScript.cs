using OpenRCT2.Generators.Map;
using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours.Generators
{
    [CreateAssetMenu(menuName = (menuPath + nameof(PathGeneratorScript)))]
    public class PathGeneratorScript : TileElementGeneratorScript
    {
        [SerializeField, Required] Material _pathMaterial = null!;
        [SerializeField, Required] string _pathTextureName = null!;


        public override ITileElementGenerator CreateGenerator()
        {
            return new PathGenerator(_pathMaterial, _pathTextureName);
        }
    }
}
