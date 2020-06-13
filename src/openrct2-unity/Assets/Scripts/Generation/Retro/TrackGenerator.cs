using System.Collections.Generic;
using Graphics;
using Lib;
using MeshBuilding;
using Tracks;
using UnityEngine;

namespace Generation.Retro
{
    public class TrackGenerator : IElementGenerator
    {
        static readonly Dictionary<short, Mesh> trackMeshCache = new Dictionary<short, Mesh>();
        static readonly Dictionary<short, TrackColour[]> trackColoursCache = new Dictionary<short, TrackColour[]>();


        [SerializeField] GameObject prefab;
        [SerializeField] Mesh trackMesh;

        Map map;
        MeshExtruder meshExtruder; 


        /// <inheritdoc/>
        public void StartGenerator(Map map)
        {
            this.map = map;
            meshExtruder = new MeshExtruder(trackMesh);
        }


        /// <inheritdoc/>
        public void FinishGenerator()
        {
            map = null;
            meshExtruder = null;
        }



        /// <inheritdoc/>
        public void CreateElement(int x, int y, ref TileElement tile)
        {
            TrackElement track = tile.AsTrack();

            if (track.PartIndex != 0)
                return;

            short trackType = track.TrackType;

            if (!trackMeshCache.TryGetValue(trackType, out Mesh trackMesh))
            {
                TrackPiece piece = TrackFactory.GetTrackPiece(trackType);
                meshExtruder.Clear();

                float offset = 0;
                int len = (piece.Points.Length - 1);
                for (int i = 0, j; i < len; i = j)
                {
                    j = (i + 1);

                    TransformPoint nodeA = piece.Points[i];
                    TransformPoint nodeB = piece.Points[j];

                    Vector3 start = nodeA.Position;
                    Vector3 end = nodeB.Position;
                    float length = Vector3.Distance(start, end);

                    meshExtruder.AddSegment(nodeA, nodeB, offset, 1, 0);

                    offset += length;
                }

                trackMesh = meshExtruder.ToMesh();
                trackMeshCache.Add(trackType, trackMesh);
            }

            if (track.IsInverted)
                Debug.Log($"Is inverted tracktype: {trackType}");

            Vector3 position = Map.TileCoordsToUnity(x, tile.baseHeight, y);
            position.y += OpenRCT2.GetTrackHeightOffset(track.RideIndex) * Map.CoordsXYMultiplier;

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

            var trackGizmos = obj.AddComponent<TrackGizmosDrawer>();
            trackGizmos.trackType = trackType;
            return; 
        }
    }
}
