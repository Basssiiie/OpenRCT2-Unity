using System;
using System.Collections.Generic;
using OpenRCT2.Bindings;
using OpenRCT2.Bindings.Graphics;
using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Generators.Extensions;
using OpenRCT2.Generators.MeshBuilding;
using OpenRCT2.Generators.Sprites;
using OpenRCT2.Generators.Tracks;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map
{
    public class TrackGenerator : ITileElementGenerator
    {
        readonly Dictionary<int, Mesh> _trackMeshCache = new Dictionary<int, Mesh>();
        readonly GameObject _prefab;
        readonly MeshExtruder _meshExtruder;


        public TrackGenerator(GameObject prefab, Mesh trackMesh)
        {
            _prefab = prefab;
            _meshExtruder = new MeshExtruder(trackMesh);
        }

        /// <inheritdoc/>
        public IEnumerator<LoadStatus> Run(Map map, Transform transform)
        {
            return map.ForEach("Creating tracks...", (Tile tile, int index, in TileElementInfo element, in TrackInfo track) =>
            {
                CreateElement(transform, tile.x, tile.y, element, track);
            });
        }

        void CreateElement(Transform transform, int x, int y, in TileElementInfo element, in TrackInfo track)
        {
            if (track.sequenceIndex != 0)
                return;

            ushort trackType = track.trackType;
            int invertedBit = Convert.ToInt32(track.inverted);
            int chainliftBit = Convert.ToInt32(track.chainlift);
            int cableliftBit = Convert.ToInt32(track.cablelift);
            int meshKey = trackType << 3 | cableliftBit << 2 | chainliftBit << 1 | invertedBit;

            if (!_trackMeshCache.TryGetValue(meshKey, out Mesh trackMesh))
            {
                trackMesh = CreateNewMesh(track);
                _trackMeshCache.Add(meshKey, trackMesh);
            }

            float trackOffset = track.trackHeight * World.CoordsZMultiplier;
            if (track.inverted)
            {
                trackOffset *= 2;
            }
            Vector3 position = World.TileCoordsToUnity(x, y, element.baseHeight);
            position.y += trackOffset;

            GameObject obj = UnityEngine.Object.Instantiate(_prefab, position, Quaternion.Euler(0, element.rotation * 90f, 0), transform);
            obj.name = $"Track [{x}, {y}, {element.baseHeight}] type: {trackType}, key: {meshKey:x}, colours: ({track.mainColour}, {track.additionalColour}, {track.supportsColour}), rot: {element.rotation}, inv: {track.inverted}";
            obj.isStatic = true;

            MeshFilter filter = obj.GetComponent<MeshFilter>();
            filter.sharedMesh = trackMesh;

            MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
            byte paletteIndex = GraphicsDataFactory.GetPaletteIndexForColourId(track.mainColour);
            renderer.material.color = PaletteFactory.IndexToColor(paletteIndex);
        }

        Mesh CreateNewMesh(in TrackInfo track)
        {
            TrackPiece piece = TrackDataFactory.GetTrackPiece(track.trackType);
            _meshExtruder.Clear();

            float offset = 0;
            int len = piece.Points.Length - 1;
            for (int i = 0, j; i < len; i = j)
            {
                j = i + 1;

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

            return _meshExtruder.ToMesh();
        }
    }
}
