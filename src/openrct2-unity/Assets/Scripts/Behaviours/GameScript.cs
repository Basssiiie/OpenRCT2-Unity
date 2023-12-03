using System.IO;
using OpenRCT2.Behaviours.Editor;
using OpenRCT2.Bindings;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours
{
    [RequireComponent(typeof(MapScript))]
    public class GameScript : MonoBehaviour
    {
        // The relative path to the selected park file.
        public string? selectedPark;

        Game? _game;


        /// <summary>
        /// Starts the game based on the settings set in the editor.
        /// </summary>
        void Start()
        {
            var openRCT2DataPath = GameConfiguration.OpenRCT2DataPath;
            var rct2Path = GameConfiguration.RCT2Path;
            var rct1Path = GameConfiguration.RCT1Path;
            var parkPath = GameConfiguration.ParkPath;

            if (string.IsNullOrWhiteSpace(openRCT2DataPath) || !Directory.Exists(openRCT2DataPath))
            {
                Debug.LogError($"Could not load OpenRCT2: openrct path is invalid. ({openRCT2DataPath})");
                return;
            }
            if (string.IsNullOrWhiteSpace(rct2Path) || !Directory.Exists(rct2Path))
            {
                Debug.LogError($"Could not load OpenRCT2: rct2 path is invalid. ({rct2Path})");
                return;
            }
            if (!string.IsNullOrWhiteSpace(rct1Path) && !Directory.Exists(rct1Path))
            {
                Debug.LogError($"Could not load OpenRCT2: rct1 path is invalid. ({rct1Path})");
                return;
            }

            var game = new Game(openRCT2DataPath, rct2Path, rct1Path);

            string parkFilePath = Path.Combine(parkPath, selectedPark);
            if (!File.Exists(parkFilePath))
            {
                Debug.LogError($"Could not load OpenRCT2: park path is invalid. ({parkFilePath})");
                return;
            }

            Debug.Log($"Starting OpenRCT2... ({parkFilePath})");
            game.OpenPark(parkFilePath);

            var map = GetComponent<MapScript>();
            map.Generate();

            _game = game;
        }


        /// <summary>
        /// Updates the game 40 times per second, similar to a regular RCT2 game tick.
        /// </summary>
        void FixedUpdate()
        {
            _game?.Update();
        }


        /// <summary>
        /// Shuts down the game.
        /// </summary>
        void OnDestroy()
        {
            _game?.ClosePark();
            Debug.Log("OpenRCT2 has shutdown.");
        }
    }
}
