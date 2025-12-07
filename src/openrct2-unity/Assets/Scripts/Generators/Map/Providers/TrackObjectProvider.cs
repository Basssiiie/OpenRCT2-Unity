
#nullable enable

using OpenRCT2.Bindings;
using OpenRCT2.Bindings.Graphics;
using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Generators.MeshBuilding;
using OpenRCT2.Generators.Sprites;
using OpenRCT2.Generators.Tracks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OpenRCT2.Generators.Map.Providers
{
    /// <summary>
    /// A provider that generates tracks according to subposition data.
    /// </summary>
    public class TrackObjectProvider : IObjectProvider<TrackInfo>
    {
        readonly GameObject _track;
        readonly KeyValuePair<int, GameObject>[] _special;

        Dictionary<int, Mesh>? _trackMeshCache;
        MeshExtruder? _meshExtruder;

        public TrackObjectProvider(GameObject track, KeyValuePair<int, GameObject>[] special)
        {
            _track = track;
            _special = special;
        }

        public GameObject? CreateObject(Map map, in Element<TrackInfo> element)
        {
            ref TrackInfo data = ref element.GetData();

            if (data.sequenceIndex != 0)
                return null;

            var cache = _trackMeshCache ??= new Dictionary<int, Mesh>();

            ushort trackType = data.trackType;
            int invertedBit = Convert.ToInt32(data.inverted);
            int chainliftBit = Convert.ToInt32(data.chainlift);
            int cableliftBit = Convert.ToInt32(data.cablelift);
            int meshKey = trackType << 3 | cableliftBit << 2 | chainliftBit << 1 | invertedBit;

            if (!cache.TryGetValue(meshKey, out Mesh trackMesh))
            {
                trackMesh = CreateNewMesh(data);
                cache.Add(meshKey, trackMesh);
            }

            float trackOffset = data.trackHeight * World.CoordsZMultiplier;
            if (data.inverted)
            {
                trackOffset *= 2;
            }

            Tile tile = element.tile;
            byte baseHeight = element.info.baseHeight;
            Vector3 position = World.TileCoordsToUnity(tile.x, tile.y, baseHeight);
            position.y += trackOffset;

            GameObject obj = UnityEngine.Object.Instantiate(_track);
            obj.name = $"Track [{tile.x}, {tile.y}, {baseHeight}] type: {trackType}, key: {meshKey:x}, colours: ({data.mainColour}, {data.additionalColour}, {data.supportsColour}), rot: {element.info.rotation}, inv: {data.inverted}";

            MeshFilter filter = obj.GetComponent<MeshFilter>();
            filter.sharedMesh = trackMesh;

            MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
            byte paletteIndex = GraphicsDataFactory.GetPaletteIndexForColourId(data.mainColour);
            renderer.material.color = PaletteFactory.IndexToColor(paletteIndex);

            return obj;
        }

        Mesh CreateNewMesh(in TrackInfo track)
        {
            var meshExtruder = GetMeshExtruder();

            TrackPiece piece = TrackDataFactory.GetTrackPiece(track.trackType);
            meshExtruder.Clear();

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

                meshExtruder.AddSegment(nodeA, nodeB, offset, 1);

                offset += length;
            }

            return meshExtruder.ToMesh();
        }

        MeshExtruder GetMeshExtruder()
        {
            if (_meshExtruder is not null)
            {
                return _meshExtruder;
            }

            var mesh = _track.GetComponent<MeshFilter>().sharedMesh;
            return _meshExtruder = new MeshExtruder(mesh);
        }
    }
}
