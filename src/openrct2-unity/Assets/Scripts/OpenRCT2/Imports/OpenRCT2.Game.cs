using System;
using System.Runtime.InteropServices;

#pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments -> this is deliberate

namespace OpenRCT
{
    public partial class OpenRCT2
    {
        /// <summary>
        /// Starts the game with the specified folder paths paths.
        /// </summary>
        /// <param name="datapath">
        /// The data folder of OpenRCT2.
        /// </param>
        /// <param name="rct2path">
        /// The absolute path to the RCT2 directory.
        /// </param>
        /// <param name="rct1path">
        /// The absolute path to the RCT1 directory (optional).
        /// </param>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void StartGame([MarshalAs(UnmanagedType.LPStr)] string datapath, [MarshalAs(UnmanagedType.LPStr)] string rct2path, [MarshalAs(UnmanagedType.LPStr)] string rct1path = default);


        /// <summary>
        /// Performs a single game update.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        static extern void PerformGameUpdate();


        /// <summary>
        /// Shuts down the game.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        static extern void StopGame();


        /// <summary>
        /// Loads a park from the specified path.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void LoadPark([MarshalAs(UnmanagedType.LPStr)] string path);


        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern IntPtr GetParkNamePtr();


        /// <summary>
        /// Returns the name of the currently loaded park.
        /// </summary>
        public static string GetParkName()
            => Marshal.PtrToStringAnsi(GetParkNamePtr());
    }
}
