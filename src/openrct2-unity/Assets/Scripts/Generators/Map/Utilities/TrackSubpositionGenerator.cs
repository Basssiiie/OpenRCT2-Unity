using System.Collections.Generic;
using OpenRCT2.Bindings;
using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Bindings.Tracks;
using OpenRCT2.Generators.Extensions;
using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map.Utilities
{
    /// <summary>
    /// A track generator that spawns a prefab at every subposition node with the node
    /// information written to the gameobject name.
    /// </summary>
    public class TrackSubpositionGenerator : ITileElementGenerator
    {
        static readonly Dictionary<ushort, TrackSubposition[]> _trackNodesCache = new Dictionary<ushort, TrackSubposition[]>();

        readonly GameObject? _prefab;


        public TrackSubpositionGenerator(GameObject? prefab)
        {
            _prefab = prefab;
        }

        /// <inheritdoc/>
        public IEnumerator<LoadStatus> Run(Map map, Transform transform)
        {
            Assert.IsNotNull(_prefab, nameof(_prefab));

            return map.ForEach("Creating subpositions...", (Tile tile, int index, in TileElementInfo element, in TrackInfo track) =>
            {
                CreateElement(transform, tile.x, tile.y, element, track);
            });
        }

        void CreateElement(Transform transform, int x, int y, in TileElementInfo element, in TrackInfo track)
        {
            if (track.sequenceIndex != 0)
                return;

            ushort trackType = track.trackType;

            if (!_trackNodesCache.TryGetValue(trackType, out TrackSubposition[] nodes))
            {
                var trackLength = TrackDataRegistry.GetSubpositionsLength(trackType);
                nodes = TrackDataRegistry.GetSubpositions(trackType, trackLength);
                _trackNodesCache.Add(trackType, nodes);
            }

            // Create objects
            GameObject parent = new GameObject($"[{x}, {y}] type: {trackType}, inv: {track.inverted}, rot: {element.rotation}, nodes: {nodes.Length}")
            {
                isStatic = true
            };

            float height = element.baseHeight + track.trackHeight * World.CoordsZMultiplier;
            Vector3 position = World.TileCoordsToUnity(x, y, height);

            Transform tfParent = parent.transform;
            tfParent.parent = transform;
            tfParent.SetLocalPositionAndRotation(position, Quaternion.Euler(0, element.rotation * 90f, 0));

            for (int i = 0; i < nodes.Length; i++)
            {
                TrackSubposition node = nodes[i];

                GameObject obj = Object.Instantiate(_prefab, Vector3.zero, Quaternion.identity, tfParent)!;
                obj.name = $"#{i} = ({node.x}, {node.y}, {node.z}) dir: {node.direction}, bank: {node.banking}, sprite: {node.pitch}";

                Transform tf = obj.transform;
                Vector3 localPosition = node.GetLocalPosition();
                Quaternion localRotation = node.GetLocalRotation();
                tf.SetLocalPositionAndRotation(localPosition, localRotation);
            }
        }
    }
}
