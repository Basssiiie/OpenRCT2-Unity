using System.IO;
using UnityEngine;

namespace OpenRCT2.Unity
{
    public partial class OpenRCT2 : MonoBehaviour
    {
        // The relative path to the selected park file.
        public string selectedPark;

        // Configuration for data paths
        string openrctDataPath;
        string rct2Path;
        string rct1Path;
        string parkPath;


        /// <summary>
        /// Starts the game based on the settings set in the editor.
        /// </summary>
        void Awake()
        {
            LoadPathSettings();
            if (!ArePathSettingsValid())
            {
                Debug.LogError("Could not load OpenRCT2: one of the specified paths is invalid.", gameObject);

                // disable everything to prevent crashes
                gameObject.SetActive(false); 
                return;
            }

            Print("Start OpenRCT2...");

            StartGame(openrctDataPath, rct2Path, rct1Path);
            LoadPark(GetParkFilePath());

            string parkname = GetParkName();
            Print($"Park name: {parkname}");

            Print("OpenRCT2 started.");
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
            Print("OpenRCT2 has shutdown.");
        }


        /// <summary>
        /// Loads the path settings from the player preferences configuration.
        /// </summary>
        void LoadPathSettings()
        {
            openrctDataPath = Configuration.OpenRCT2DataPath;
            rct2Path = Configuration.RCT2Path;
            rct1Path = Configuration.RCT1Path;
            parkPath = Configuration.ParkPath;
        }


        /// <summary>
        /// Checks whether all paths exist.
        /// </summary>
        bool ArePathSettingsValid()
        {
            return (Directory.Exists(openrctDataPath)
                && Directory.Exists(rct2Path)
                && (string.IsNullOrWhiteSpace(rct1Path) || Directory.Exists(rct1Path))
                && File.Exists(GetParkFilePath()));
        }


        /// <summary>
        /// Gets the file path to the park file.
        /// </summary>
        string GetParkFilePath()
            => Path.Combine(parkPath, selectedPark);
    }
}
