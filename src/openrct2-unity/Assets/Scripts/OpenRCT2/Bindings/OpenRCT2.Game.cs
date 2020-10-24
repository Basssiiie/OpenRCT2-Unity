using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

#pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments -> this is deliberate
#pragma warning disable CA1401 // P/Invokes should not be visible -> this is also deliberate

namespace Lib
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
        /// Starts the game.
        /// </summary>
        public static bool StartGame()
        {
            LoadPathSettings();
            if (!ArePathSettingsValid())
                return false;

            StartGame(openrctDataPath, rct2Path, rct1Path);
            return true;
        }


        /// <summary>
        /// Performs a single game update.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        static extern void PerformGameUpdate();


        /// <summary>
        /// Shuts down the game.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void StopGame();


        /// <summary>
        /// Loads a park from the specified path.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void LoadPark([MarshalAs(UnmanagedType.LPStr)] string path);


        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr GetParkNamePtr();


        /// <summary>
        /// Returns the name of the currently loaded park.
        /// </summary>
        public static string GetParkName()
            => Marshal.PtrToStringAnsi(GetParkNamePtr());


        /// <summary>
        /// Loads the path settings from the player preferences configuration.
        /// </summary>
        static void LoadPathSettings()
        {
            openrctDataPath = Configuration.OpenRCT2DataPath;
            rct2Path = Configuration.RCT2Path;
            rct1Path = Configuration.RCT1Path;
            parkPath = Configuration.ParkPath;
        }


        /// <summary>
        /// Checks whether all paths exist.
        /// </summary>
        static bool ArePathSettingsValid()
        {
            if (!Directory.Exists(openrctDataPath))
            {
                Debug.LogError($"Could not load OpenRCT2: openrct path is invalid. ({openrctDataPath})");
                return false;
            }
            if (!Directory.Exists(rct2Path))
            {
                Debug.LogError($"Could not load OpenRCT2: rct2 path is invalid. ({rct2Path})");
                return false;
            }
            if (!string.IsNullOrWhiteSpace(rct1Path) && !Directory.Exists(rct1Path))
            {
                Debug.LogError($"Could not load OpenRCT2: rct1 path is invalid. ({rct1Path})");
                return false;
            }
            return true;
        }
    }
}
