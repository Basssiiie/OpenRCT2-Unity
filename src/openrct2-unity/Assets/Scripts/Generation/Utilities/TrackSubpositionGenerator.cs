using System.Collections.Generic;
using Lib;
using UnityEngine;

namespace Generation
{
    /// <summary>
    /// A track generator that spawns a prefab at every subposition node with the node
    /// information written to the gameobject name.
    /// </summary>
    [CreateAssetMenu(menuName = (MenuPath + "Utilities/" + nameof(TrackSubpositionGenerator)))]
    public class TrackSubpositionGenerator : TileElementGenerator
    {
        static readonly Dictionary<short, TrackNode[]> _trackNodesCache = new Dictionary<short, TrackNode[]>();


        [SerializeField] GameObject _prefab;


        /// <inheritdoc/>
        public override void CreateElement(int x, int y, in TileElement tile)
        {
            TrackElement track = tile.AsTrack();
            if (track.PartIndex != 0)
                return;

            short trackType = track.TrackType;

            if (!_trackNodesCache.TryGetValue(trackType, out TrackNode[] nodes))
            {
                nodes = OpenRCT2.GetTrackElementRoute(trackType);
                _trackNodesCache.Add(trackType, nodes);
            }

            // Create objects
            GameObject parent = new GameObject($"[{x}, {y}] type: {trackType}, inv: {track.IsInverted}, rot: {tile.Rotation}, nodes: {nodes.Length}")
            {
                isStatic = true
            };

            Vector3 position = Map.TileCoordsToUnity(x, tile.baseHeight, y);
            position.y += OpenRCT2.GetTrackHeightOffset(track.RideIndex) * Map.CoordsXYMultiplier;

            Transform tfParent = parent.transform;
            tfParent.parent = _map.transform;
            tfParent.localPosition = position;
            tfParent.localRotation = Quaternion.Euler(0, tile.Rotation * 90f, 0);

            for (int i = 0; i < nodes.Length; i++)
            {
                TrackNode node = nodes[i];

                GameObject obj = GameObject.Instantiate(_prefab, Vector3.zero, Quaternion.identity, tfParent);
                obj.name = $"#{i} = ({node.x}, {node.y}, {node.z}) dir: {node.direction}, bank: {node.bankRotation}, sprite: {node.vehicleSprite}";

                Transform tf = obj.transform;
                tf.localPosition = node.LocalPosition;
                tf.localRotation = node.LocalRotation;
            }
        }
    }
}
