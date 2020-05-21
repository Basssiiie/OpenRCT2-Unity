using System.Runtime.InteropServices;


namespace Lib
{
    public partial class OpenRCT2
    {
        /// <summary>
        /// Returns the amount of path nodes in the pathing route for the specified track type.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern int GetTrackElementRouteSize(int trackVariant, int typeAndDirection);


        /// <summary>
        /// Writes all the path nodes up to size into the array.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void GetTrackElementRoute(int trackVariant, int typeAndDirection, [Out] TrackNode[] nodes, int size);


        /// <summary>
        /// Gets the path nodes in the pathing route for the specified track type.
        /// </summary>
        /// <param name="trackType">
        /// The track type to retrieve.
        /// </param>
        /// <param name="trackVariant">
        /// The pathing information table to use.
        /// <list type="bullet">
        ///     <item><term>0</term><description>Regular tracks.</description></item>
        ///     <item><term>1-4</term><description>Custom routing for chairlifts.</description></item>
        ///     <item><term>5-8</term><description>Custom routing for go karts.</description></item>
        ///     <item><term>9-14</term><description>Custom routing for mini golf.</description></item>
        ///     <item><term>15-16</term><description>Custom routing for reversers.</description></item>
        /// </list>
        /// </param>
        public static TrackNode[] GetTrackElementRoute(int trackType, int trackVariant)
        {
            int typeAndDirection = trackType << 2; // direction right now is defaulted to 0;
            int size = GetTrackElementRouteSize(trackVariant, typeAndDirection);

            TrackNode[] nodes = new TrackNode[size];
            GetTrackElementRoute(trackVariant, typeAndDirection, nodes, size);
            return nodes;
        }


        /// <summary>
        /// Gets the path nodes in the pathing route for the specified track type.
        /// </summary>
        /// <param name="trackType">
        /// The track type to retrieve.
        /// </param>
        public static TrackNode[] GetTrackElementRoute(int trackType)
            => GetTrackElementRoute(trackType, 0);
    }
}
