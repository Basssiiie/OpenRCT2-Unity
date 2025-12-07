using OpenRCT2.Bindings;
using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Generators.MeshBuilding;
using OpenRCT2.Generators.Tracks;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map.Providers
{
    public class TunnelObjectProvider : IObjectProvider<TunnelInfo>
    {
        readonly GameObject _tunnel;
        readonly GameObject _door;

        Dictionary<int, Mesh>? _trackMeshCache;
        MeshExtruder? _meshExtruder;

        public TunnelObjectProvider(GameObject tunnel, GameObject door)
        {
            _tunnel = tunnel;
            _door = door;
        }

        public GameObject? CreateObject(Map map, in Element<TunnelInfo> element)
        {
            ref TunnelInfo data = ref element.GetData();

            var cache = _trackMeshCache ??= new Dictionary<int, Mesh>();

            ushort trackType = data.trackType;

            if (!cache.TryGetValue(trackType, out Mesh trackMesh))
            {
                trackMesh = CreateNewMesh(data.trackType);
                cache.Add(trackType, trackMesh);
            }

            Tile tile = element.tile;
            byte baseHeight = element.info.baseHeight;
            Vector3 position = World.TileCoordsToUnity(tile.x, tile.y, baseHeight);

            GameObject obj = UnityEngine.Object.Instantiate(_tunnel);
            obj.name = $"Tunnel [{tile.x}, {tile.y}, {baseHeight}] type: {trackType}";

            MeshFilter filter = obj.GetComponent<MeshFilter>();
            filter.sharedMesh = trackMesh;

            return obj;
        }

        Mesh CreateNewMesh(ushort trackType)
        {
            var meshExtruder = GetMeshExtruder();

            TrackPiece piece = TrackDataFactory.GetTrackPiece(trackType);
            meshExtruder.Clear();

            float offset = 0;
            int len = piece.Points.Length - 1;
            for (int i = 0, j; i < len; i = j)
            {
                j = i + 1;

                TransformPoint nodeA = piece.Points[i];
                TransformPoint nodeB = piece.Points[j];

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

            var mesh = _tunnel.GetComponent<MeshFilter>().sharedMesh;
            return _meshExtruder = new MeshExtruder(mesh);
        }
    }
}
