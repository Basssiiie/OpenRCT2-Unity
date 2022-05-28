using System.IO;
using UnityEngine;

#nullable enable

namespace Lib
{
    public partial class OpenRCT2 : MonoBehaviour
    {
        const string PluginFile = "openrct2-bindings";


        // The relative path to the selected park file.
        public string? selectedPark;

        // Configuration for data paths
        static string? _openrctDataPath;
        static string? _rct2Path;
        static string? _rct1Path;
        static string? _parkPath;


        /// <summary>
        /// Starts the game based on the settings set in the editor.
        /// </summary>
        void Awake()
        {
            LoadPathSettings();
            string parkFilePath = GetParkFilePath();
            if (!File.Exists(parkFilePath))
            {
                Debug.LogError($"Could not load OpenRCT2: park path is invalid. ({parkFilePath})", gameObject);

                // disable everything to prevent crashes
                gameObject.SetActive(false); 
                return;
            }

            if (!StartGame())
            {
                // disable everything to prevent crashes
                gameObject.SetActive(false);
                return;
            }

            Debug.Log("Starting OpenRCT2...");

            LoadPark(GetParkFilePath());

            string parkname = GetParkName();
            Debug.Log($"OpenRCT2 started on park: {parkname}.");
        }


        /// <summary>
        /// Updates the game 40 times per second, similar to a regular RCT2 game tick.
        /// </summary>
        void FixedUpdate()
        {
            PerformGameUpdate();
        }


        /// <summary>
        /// Shuts down the game.
        /// </summary>
        void OnDestroy()
        {
            StopGame();
            Debug.Log("OpenRCT2 has shutdown.");
        }


        /// <summary>
        /// Gets the file path to the park file.
        /// </summary>
        string GetParkFilePath()
            => Path.Combine(_parkPath, selectedPark);
    }
}
