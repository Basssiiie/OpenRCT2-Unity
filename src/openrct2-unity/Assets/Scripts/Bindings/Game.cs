using System.Runtime.InteropServices;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Bindings
{
    public class Game
    {
        // Configuration for data paths
        readonly string _openRCT2DataPath;
        readonly string _rct2Path;
        readonly string? _rct1Path;


        /// <summary>
        /// Starts the game based on the settings set in the editor.
        /// </summary>
        public Game(string openRCT2DataPath, string rct2Path, string? rct1Path)
        {
            _openRCT2DataPath = openRCT2DataPath;
            _rct2Path = rct2Path;
            _rct1Path = rct1Path;
        }


        /// <summary>
        /// Starts the game by opening a park.
        /// </summary>
        public void OpenPark(string parkPath)
        {
            StartGame(_openRCT2DataPath, _rct2Path, _rct1Path);
            LoadPark(parkPath);

            string parkname = Park.GetName();
            Debug.Log($"OpenRCT2 started on park: {parkname}.");
        }


        /// <summary>
        /// Shuts down the game.
        /// </summary>
        public void ClosePark()
        {
            StopGame();
        }


        /// <summary>
        /// Performs a single game update.
        /// </summary>
        public void Update()
        {
            PerformGameUpdate();
        }


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
        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void StartGame([MarshalAs(UnmanagedType.LPStr)] string datapath, [MarshalAs(UnmanagedType.LPStr)] string rct2path, [MarshalAs(UnmanagedType.LPStr)] string? rct1path = default);


        /// <summary>
        /// Performs a single game update.
        /// </summary>
        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl)]
        static extern void PerformGameUpdate();


        /// <summary>
        /// Shuts down the game.
        /// </summary>
        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl)]
        static extern void StopGame();


        /// <summary>
        /// Loads a park from the specified path.
        /// </summary>
        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void LoadPark([MarshalAs(UnmanagedType.LPStr)] string path);
    }
}
