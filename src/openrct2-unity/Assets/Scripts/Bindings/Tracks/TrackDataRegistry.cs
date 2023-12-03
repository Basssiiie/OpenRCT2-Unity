using System.Runtime.InteropServices;

#nullable enable

namespace OpenRCT2.Bindings.Tracks
{
    public static class TrackDataRegistry
    {
        /// <summary>
        /// Gets the total amount of track piece types in the game.
        /// </summary>
        public static int TrackTypesCount => _trackTypesCount ??= GetTrackTypesCount();
        private static int? _trackTypesCount;


        /// <summary>
        /// Gets the length of the path nodes route for the specified track type.
        /// </summary>
        public static ushort GetSubpositionsLength(ushort trackType)
        {
            return GetTrackSubpositionsLength(0, trackType, 0);
        }


        /// <summary>
        /// Gets the path nodes in the pathing route for the specified track type.
        /// </summary>
        public static TrackSubposition[] GetSubpositions(ushort trackType, ushort trackLength)
        {
            TrackSubposition[] nodes = new TrackSubposition[trackLength];
            GetTrackSubpositions(0, trackType, 0, nodes, trackLength);
            return nodes;
        }


        /// <summary>
        /// Get the upper bound of track piece type ids.
        /// </summary>
        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl)]
        static extern int GetTrackTypesCount();

        /// <summary>
        /// Get the subposition length of the specified track type and direction.
        /// </summary>
        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl)]
        static extern ushort GetTrackSubpositionsLength(byte subposition, ushort type, byte direction);

        /// <summary>
        /// Writes all the path nodes up to size into the array.
        /// </summary>
        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl)]
        static extern void GetTrackSubpositions(byte subposition, ushort type, byte direction, [Out] TrackSubposition[] nodes, int size);
    }
}
