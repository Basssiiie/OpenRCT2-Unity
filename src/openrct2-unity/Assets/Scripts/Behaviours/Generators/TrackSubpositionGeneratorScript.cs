using OpenRCT2.Generators.Map;
using OpenRCT2.Generators.Map.Utilities;
using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours.Generators
{
    /// <summary>
    /// A track generator that spawns a prefab at every subposition node with the node
    /// information written to the gameobject name.
    /// </summary>
    [CreateAssetMenu(menuName = (menuPath + "Utilities/" + nameof(TrackSubpositionGeneratorScript)))]
    public class TrackSubpositionGeneratorScript : TileElementGeneratorScript
    {
        [SerializeField, Required] GameObject? _prefab;


        public override ITileElementGenerator CreateGenerator()
        {
            return new TrackSubpositionGenerator(_prefab);
        }
    }
}
