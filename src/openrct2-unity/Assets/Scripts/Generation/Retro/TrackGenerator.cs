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
        static readonly Dictionary<ushort, TrackColour[]> _trackColoursCache = new Dictionary<ushort, TrackColour[]>();


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
        public override void CreateElement(Map map, int x, int y, in TileElement tile)
        {
            Assert.IsNotNull(_meshExtruder, nameof(_meshExtruder));

            TrackElement track = tile.AsTrack();

            if (track.PartIndex != 0)
                return;

            short trackType = track.TrackType;
            int meshKey = (trackType << 3) | ((byte)track.Flags2 & KeyFlagsMask);

            if (!_trackMeshCache.TryGetValue(meshKey, out Mesh trackMesh))
            {
                TrackPiece piece = TrackFactory.GetTrackPiece(trackType);
                TrackTypeFlags flags = OpenRCT2.GetTrackTypeFlags(trackType);
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
                    if (track.IsInverted && (flags & (TrackTypeFlags.NormalToInversion | TrackTypeFlags.InversionToNormal)) == 0)
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

            float trackOffset = (OpenRCT2.GetTrackHeightOffset(track.RideIndex) * Map.CoordsXYMultiplier);
            if (track.IsInverted)
                trackOffset *= 2;

            Vector3 position = Map.TileCoordsToUnity(x, tile.baseHeight, y);
            position.y += trackOffset;

            GameObject obj = GameObject.Instantiate(_prefab, position, Quaternion.Euler(0, tile.Rotation * 90f, 0), map.transform);

            obj.name = $"[{x}, {y}] scheme: {track.ColourScheme}, rot: {tile.Rotation}, type: {trackType}, inv: {track.IsInverted}";
            MeshFilter filter = obj.GetComponent<MeshFilter>();
            filter.sharedMesh = trackMesh;

            ushort rideIndex = track.RideIndex;
            if (!_trackColoursCache.TryGetValue(rideIndex, out TrackColour[] colours))
            {
                colours = OpenRCT2.GetRideTrackColours(rideIndex);
                _trackColoursCache.Add(rideIndex, colours);
            }

            MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
            renderer.material.color = GraphicsFactory.PaletteToColor(OpenRCT2.GetPaletteIndexForColourId(colours[0].main));
            return; 
        }
    }
}
