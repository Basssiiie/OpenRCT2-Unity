using System.Runtime.InteropServices;

#nullable enable

namespace Lib
{
    public partial class OpenRCT2
    {
        // Maximum number of colour schemes to retrieve.
        const int MaxColourSchemes = 4;


        /// <summary>
        /// Gets the path nodes in the pathing route for the specified track type.
        /// </summary>
        public static TrackSubposition[] GetTrackSubpositions(ushort trackType, ushort trackLength)
        {
            TrackSubposition[] nodes = new TrackSubposition[trackLength];
            GetTrackSubpositions(trackType, 0, nodes, trackLength);
            return nodes;
        }

        /// <summary>
        /// Writes all the path nodes up to size into the array.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        static extern void GetTrackSubpositions(ushort type, byte direction, [Out] TrackSubposition[] nodes, int size);
    }
}
