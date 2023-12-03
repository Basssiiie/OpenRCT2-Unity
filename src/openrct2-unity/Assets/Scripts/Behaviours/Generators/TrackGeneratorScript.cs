using OpenRCT2.Generators.Map;
using OpenRCT2.Generators.Map.Retro;
using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours.Generators
{
    [CreateAssetMenu(menuName = (menuPath + "Retro/" + nameof(TrackGeneratorScript)))]
    public class TrackGeneratorScript : TileElementGeneratorScript
    {
        [SerializeField, Required] GameObject _prefab = null!;
        [SerializeField, Required] Mesh _trackMesh = null!;


        public override ITileElementGenerator CreateGenerator()
        {
            return new TrackGenerator(_prefab, _trackMesh);
        }
    }
}
