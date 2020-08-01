using System.Collections.Generic;
using Graphics;
using Lib;
using MeshBuilding;
using Tracks;
using UnityEngine;

namespace Generation.Retro
{
    [CreateAssetMenu(menuName = (MenuPath + "Retro/" + nameof(TrackGenerator)))]
    public class TrackGenerator : TileElementGenerator
    {
        static readonly Dictionary<int, Mesh> trackMeshCache = new Dictionary<int, Mesh>();
        static readonly Dictionary<short, TrackColour[]> trackColoursCache = new Dictionary<short, TrackColour[]>();


        [SerializeField] GameObject prefab;
        [SerializeField] Mesh trackMesh;


        // Only the first 3 flags matter for the mesh.
        const byte KeyFlagsMask = 0b111;

        MeshExtruder meshExtruder; 


        /// <inheritdoc/>
        protected override void Start()
        {
            meshExtruder = new MeshExtruder(trackMesh);
        }


        /// <inheritdoc/>
        protected override void Finish()
        {
            meshExtruder = null;
        }



        /// <inheritdoc/>
        public override void CreateElement(int x, int y, in TileElement tile)
        {
            TrackElement track = tile.AsTrack();

            if (track.PartIndex != 0)
                return;

            short trackType = track.TrackType;
            int meshKey = (trackType << 3) | ((byte)track.Flags2 & KeyFlagsMask);

            if (!trackMeshCache.TryGetValue(meshKey, out Mesh trackMesh))
            {
                TrackPiece piece = TrackFactory.GetTrackPiece(trackType);
                TrackTypeFlags flags = OpenRCT2.GetTrackTypeFlags(trackType);
                meshExtruder.Clear();

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

                    meshExtruder.AddSegment(nodeA, nodeB, offset, 1, 0);

                    offset += length;
                }

                trackMesh = meshExtruder.ToMesh();
                trackMeshCache.Add(meshKey, trackMesh);
            }

            float trackOffset = (OpenRCT2.GetTrackHeightOffset(track.RideIndex) * Map.CoordsXYMultiplier);
            if (track.IsInverted)
                trackOffset *= 2;

            Vector3 position = Map.TileCoordsToUnity(x, tile.baseHeight, y);
            position.y += trackOffset;

            GameObject obj = GameObject.Instantiate(prefab, position, Quaternion.Euler(0, tile.Rotation * 90f, 0), map.transform);

            obj.name = $"[{x}, {y}] scheme: {track.ColourScheme}, rot: {tile.Rotation}, type: {trackType}, inv: {track.IsInverted}";
            MeshFilter filter = obj.GetComponent<MeshFilter>();
            filter.sharedMesh = trackMesh;

            short rideIndex = track.RideIndex;
            if (!trackColoursCache.TryGetValue(rideIndex, out TrackColour[] colours))
            {
                colours = OpenRCT2.GetRideTrackColours(rideIndex);
                trackColoursCache.Add(rideIndex, colours);
            }

            MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
            renderer.material.color = GraphicsFactory.PaletteToColor(OpenRCT2.GetPaletteIndexForColourId(colours[0].main));
            return; 
        }
    }
}
