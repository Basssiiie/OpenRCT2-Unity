using System.Runtime.InteropServices;

#nullable enable

namespace OpenRCT2.Bindings.Tracks
{
    public static class TrackDataRegistry
    {
        /// <summary>
        /// Gets the path nodes in the pathing route for the specified track type.
        /// </summary>
        public static TrackSubposition[] GetSubpositions(ushort trackType, ushort trackLength)
        {
            TrackSubposition[] nodes = new TrackSubposition[trackLength];
            GetTrackSubpositions(trackType, 0, nodes, trackLength);
            return nodes;
        }

        /// <summary>
        /// Writes all the path nodes up to size into the array.
        /// </summary>
        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl)]
        static extern void GetTrackSubpositions(ushort type, byte direction, [Out] TrackSubposition[] nodes, int size);
    }
}
