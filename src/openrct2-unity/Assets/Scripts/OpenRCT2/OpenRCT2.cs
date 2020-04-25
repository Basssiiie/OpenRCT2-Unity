using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace OpenRCT2.Unity
{
    public partial class OpenRCT2 : MonoBehaviour
    {
        [SerializeField] string park = "My test park with burgers.sv6";
        [SerializeField] string rct2Path;
        [SerializeField] string rct1Path;


        // OpenRCT2 takes the executing directory by default; which is where the Unity Editor is installed here...
        const string rootpath = @"D:\Projects\Visual Studio\OpenRCT2-Unity";
        const string datapath = rootpath + @"\bin\data";
        const string parkpath = rootpath + @"\src\openrct2-unity\parks";


        void Awake()
        {
            if (ArePathSettingsInvalid())
            {
                // disable everything to prevent crashes
                gameObject.SetActive(false); 
                return;
            }

            Print("Start OpenRCT2...");

            StartGame(datapath, rct2Path, rct1Path);
            LoadPark($@"{parkpath}\{park}");

            string parkname = GetParkName();
            Print($"Park name: {parkname}");

            Print("OpenRCT2 started.");
        }


        void FixedUpdate()
        {
            PerformGameUpdate();
        }


        void OnDestroy()
        {
            StopGame();
            Print("OpenRCT2 has shutdown.");
        }


        bool ArePathSettingsInvalid()
        {
            bool error = false;

            // RCT2 data folder
            if (string.IsNullOrWhiteSpace(rct2Path))
            {
                Debug.LogError("The RCT2 path is not specified!\nPoint it to the folder where RCT2 is installed.");
                error = true;
            }
            else if (!Directory.Exists(rct2Path))
            {
                Debug.LogError($"The RCT2 path does not exist: '{rct2Path}'\nPoint it to the folder where RCT2 is installed.");
                error = true;
            }

            // RCT1 data folder (optional)
            if (!string.IsNullOrWhiteSpace(rct1Path) && !Directory.Exists(rct1Path))
            {
                Debug.LogError($"The RCT1 path does not exist: '{rct1Path}'\nPoint it to the folder where RCT1 is installed or leave it empty.");
                error = true;
            }

            // Park file
            if (string.IsNullOrWhiteSpace(park))
            {
                Debug.LogError($"Please enter a valid park file.");
                error = true;
            }
            else
            {
                string fullparkpath = $@"{parkpath}\{park}";
                if (!File.Exists(fullparkpath))
                {
                    Debug.LogError($"The specified park file does not exist: '{fullparkpath}'\nPoint it to a valid park file and make sure to include the file extension.");
                    error = true;
                }
            }
            return error;
        }
    }
}
