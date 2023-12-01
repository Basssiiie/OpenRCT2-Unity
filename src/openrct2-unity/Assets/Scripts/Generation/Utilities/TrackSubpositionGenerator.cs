using System.Collections.Generic;
using Lib;
using UnityEngine;
using Utilities;

#nullable enable

namespace Generation
{
    /// <summary>
    /// A track generator that spawns a prefab at every subposition node with the node
    /// information written to the gameobject name.
    /// </summary>
    [CreateAssetMenu(menuName = (MenuPath + "Utilities/" + nameof(TrackSubpositionGenerator)))]
    public class TrackSubpositionGenerator : TileElementGenerator
    {
        static readonly Dictionary<ushort, TrackSubposition[]> _trackNodesCache = new Dictionary<ushort, TrackSubposition[]>();


        [SerializeField, Required] GameObject? _prefab;


        /// <inheritdoc/>
        public override void CreateElement(Map map, int x, int y, int index, in TileElementInfo tile)
        {
            Assert.IsNotNull(_prefab, nameof(_prefab));


            TrackInfo track = OpenRCT2.GetTrackElementAt(x, y, index);
            if (track.sequenceIndex != 0)
                return;

            ushort trackType = track.trackType;

            if (!_trackNodesCache.TryGetValue(trackType, out TrackSubposition[] nodes))
            {
                nodes = OpenRCT2.GetTrackSubpositions(trackType, track.trackLength);
                _trackNodesCache.Add(trackType, nodes);
            }

            // Create objects
            GameObject parent = new GameObject($"[{x}, {y}] type: {trackType}, inv: {track.inverted}, rot: {tile.rotation}, nodes: {nodes.Length}")
            {
                isStatic = true
            };

            float height = (tile.baseHeight + track.trackHeight * Map.CoordsZMultiplier);
            Vector3 position = Map.TileCoordsToUnity(x, y, height);

            Transform tfParent = parent.transform;
            tfParent.parent = map.transform;
            tfParent.localPosition = position;
            tfParent.localRotation = Quaternion.Euler(0, tile.rotation * 90f, 0);

            for (int i = 0; i < nodes.Length; i++)
            {
                TrackSubposition node = nodes[i];

                GameObject obj = GameObject.Instantiate(_prefab, Vector3.zero, Quaternion.identity, tfParent);
                obj.name = $"#{i} = ({node.x}, {node.y}, {node.z}) dir: {node.direction}, bank: {node.banking}, sprite: {node.pitch}";

                Transform tf = obj.transform;
                tf.localPosition = node.LocalPosition;
                tf.localRotation = node.LocalRotation;
            }
        }
    }
}
