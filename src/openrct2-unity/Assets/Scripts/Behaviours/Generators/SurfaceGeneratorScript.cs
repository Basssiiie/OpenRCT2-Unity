using OpenRCT2.Behaviours.Generators;
using OpenRCT2.Generators.Map;
using OpenRCT2.Generators.Map.Retro;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours
{
    /// <summary>
    /// A generator that generates the surface of a map.
    /// </summary>
    [CreateAssetMenu(menuName = (menuPath + "Retro/" + nameof(SurfaceGeneratorScript)))]
    public class SurfaceGeneratorScript : TileElementGeneratorScript
    {
        [SerializeField] int _chunkSize = 64;
        [SerializeField] Shader? _surfaceShader;
        [SerializeField] string _surfaceTextureField = "Surface";
        [SerializeField] Shader? _edgeShader;
        [SerializeField] string _edgeTextureField = "Edge";
        [SerializeField] Shader? _waterShader;
        [SerializeField] string _waterTextureField = "Water";
        [SerializeField] string _waterRefractionField = "WaterRefraction";


        public override ITileElementGenerator CreateGenerator()
        {
            return new SurfaceGenerator(_chunkSize, _surfaceShader, _surfaceTextureField, _edgeShader, _edgeTextureField, _waterShader, _waterTextureField, _waterRefractionField);
        }
    }
}
