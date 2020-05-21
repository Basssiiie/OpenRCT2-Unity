using System.Collections.Generic;
using Lib;
using UnityEngine;

namespace Generation.Retro
{
    public class TrackGenerator : IElementGenerator
    {
        static Dictionary<int, TrackNode[]> trackNodesCache = new Dictionary<int, TrackNode[]>();


        [SerializeField] GameObject prefab;

        Map map;


        /// <inheritdoc/>
        public void StartGenerator(Map map)
        {
            this.map = map;
        }


        /// <inheritdoc/>
        public void FinishGenerator()
        {
            map = null;
        }



        /// <inheritdoc/>
        public void CreateElement(int x, int y, ref TileElement tile)
        {
            TrackElement track = tile.AsTrack();

            if (track.PartIndex != 0)
                return;

            int trackType = track.TrackType;

            if (!trackNodesCache.TryGetValue(trackType, out TrackNode[] nodes))
            {
                nodes = OpenRCT2.GetTrackElementRoute(trackType);
                trackNodesCache.Add(trackType, nodes);
            }

            const float trackOffset = -0.5f;

            GameObject parent = new GameObject($"[{x}, {y}] rot: {tile.Rotation}, type: {trackType}")
            {
                isStatic = true
            };

            Transform tfParent = parent.transform;
            tfParent.parent = map.transform;
            tfParent.localPosition = Map.TileCoordsToUnity(x, tile.baseHeight, y);
            tfParent.localRotation = Quaternion.Euler(0, tile.Rotation * 90f, 0);

            for (int i = 0; i < nodes.Length; i++)
            {
                TrackNode node = nodes[i];

                GameObject obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, tfParent);
                obj.name = $"#{i} = dir: {node.direction}, bank: {node.bankRotation}, sprite: {node.vehicleSprite}";

                Vector3 local = node.LocalPosition;
                Transform tf = obj.transform;
                tf.localPosition = new Vector3(local.x + trackOffset, local.y, local.z + trackOffset);
                tf.localRotation = Quaternion.Euler(0, ((360f / 32f) * node.direction) + 270f, 0);
            }
        }
    }
}
