using System;
using System.Collections.Generic;
using Graphics;
using Lib;
using MeshBuilding;
using Tracks;
using UnityEngine;
using Utilities;

#nullable enable

namespace Generation.Retro
{
    [CreateAssetMenu(menuName = (MenuPath + "Retro/" + nameof(TrackGenerator)))]
    public class TrackGenerator : TileElementGenerator
    {
        static readonly Dictionary<int, Mesh> _trackMeshCache = new Dictionary<int, Mesh>();


        [SerializeField, Required] GameObject _prefab = null!;
        [SerializeField, Required] Mesh _trackMesh = null!;


        // Only the first 3 flags matter for the mesh.
        const byte KeyFlagsMask = 0b111;

        MeshExtruder? _meshExtruder; 


        /// <inheritdoc/>
        protected override void Startup(Map map)
        {
            _meshExtruder = new MeshExtruder(_trackMesh);
        }


        /// <inheritdoc/>
        protected override void Finish(Map map)
        {
            _meshExtruder = null;
        }



        /// <inheritdoc/>
        public override void CreateElement(Map map, int x, int y, int index, in TileElementInfo tile)
        {
            Assert.IsNotNull(_meshExtruder, nameof(_meshExtruder));

            TrackInfo track = OpenRCT2.GetTrackElementAt(x, y, index);

            if (track.sequenceIndex != 0)
                return;

            ushort trackType = track.trackType;
            int invertedBit = Convert.ToInt32(track.inverted);
            int chainliftBit = Convert.ToInt32(track.chainlift);
            int cableliftBit = Convert.ToInt32(track.cablelift);
            int meshKey = (trackType << 3) | (cableliftBit << 2) | (chainliftBit << 1) | (invertedBit);

            if (!_trackMeshCache.TryGetValue(meshKey, out Mesh trackMesh))
            {
                TrackPiece piece = TrackFactory.GetOrCreateTrackPiece(track);
                _meshExtruder.Clear();

                float offset = 0;
                int len = (piece.Points.Length - 1);
                for (int i = 0, j; i < len; i = j)
                {
                    j = (i + 1);

                    TransformPoint nodeA = piece.Points[i];
                    TransformPoint nodeB = piece.Points[j];

                    // If the track is inverted, and its not an inversion, rotate
                    // TODO: still not perfect; some inverted inversions still mess up. Also performance could be better.
                    if (track.inverted && (track.normalToInverted || track.invertedToNormal))
                    {
                        Quaternion inversionAngle = Quaternion.AngleAxis(180, Vector3.forward);
                        nodeA = new TransformPoint(nodeA.Position, nodeA.Rotation * inversionAngle);
                        nodeB = new TransformPoint(nodeB.Position, nodeB.Rotation * inversionAngle);
                    }

                    Vector3 start = nodeA.Position;
                    Vector3 end = nodeB.Position;
                    float length = Vector3.Distance(start, end);

                    _meshExtruder.AddSegment(nodeA, nodeB, offset, 1, 0);

                    offset += length;
                }

                trackMesh = _meshExtruder.ToMesh();
                _trackMeshCache.Add(meshKey, trackMesh);
            }

            float trackOffset = (track.trackHeight * Map.CoordsZMultiplier);
            if (track.inverted)
                trackOffset *= 2;

            Vector3 position = Map.TileCoordsToUnity(x, y, tile.baseHeight);
            position.y += trackOffset;

            GameObject obj = GameObject.Instantiate(_prefab, position, Quaternion.Euler(0, tile.rotation * 90f, 0), map.transform);

            obj.name = $"[{x}, {y}] colours: ({track.mainColour}, {track.additionalColour}, {track.supportsColour}), rot: {tile.rotation}, type: {trackType}, inv: {track.inverted}";
            MeshFilter filter = obj.GetComponent<MeshFilter>();
            filter.sharedMesh = trackMesh;

            MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
            renderer.material.color = GraphicsFactory.PaletteToColor(OpenRCT2.GetPaletteIndexForColourId(track.mainColour));
        }
    }
}
