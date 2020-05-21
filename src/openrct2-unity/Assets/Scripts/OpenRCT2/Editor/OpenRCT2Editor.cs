using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Lib
{
    /// <summary>
    /// An editor that helps configuring OpenRCT2 settings in Unity, for example
    /// the paths to the games, data folders or parks.
    /// </summary>
    [CustomEditor(typeof(OpenRCT2))]
    public class OpenRCT2Editor : Editor
    {
        const string DefaultOpenRCT2Path = @"bin\data"; // from repo root
        const string DefaultParkPath = @"Parks"; // from unity project/executable root

        static readonly string[] ValidParkExtensions = { ".sv6", ".sc6", ".sv4", ".sc4" };


        bool groupToggleDataPaths = true;
        string openrct2DataPath;
        string rct2Path;
        string rct1Path;
        string parkPath;

        bool groupToggleSelectedPark = true;
        string[] allDiscoveredParks;
        int selectedParkIndex = -1;


        void OnEnable()
        {
            openrct2DataPath = Configuration.OpenRCT2DataPath;

            // If openrct2 path is null, take the default bin folder in the project.
            if (string.IsNullOrWhiteSpace(openrct2DataPath))
                openrct2DataPath = GetDefaultOpenRCT2DataPath();

            rct2Path = Configuration.RCT2Path;
            rct1Path = Configuration.RCT1Path;

            parkPath = Configuration.ParkPath;

            // If park path is null, take the default park folder in the project.
            if (string.IsNullOrWhiteSpace(parkPath))
            {
                DirectoryInfo unityRoot = FindUnityRootFolder();
                parkPath = Path.Combine(unityRoot.FullName, DefaultParkPath);
            }
        }


        void OnDisable()
        {
            Configuration.OpenRCT2DataPath = openrct2DataPath;
            Configuration.RCT2Path = rct2Path;
            Configuration.RCT1Path = rct1Path;
            Configuration.ParkPath = parkPath;
        }


        /// <summary>
        /// This gets called every UI update.
        /// </summary>
        public override void OnInspectorGUI()
        {
            groupToggleDataPaths = EditorGUILayout.BeginFoldoutHeaderGroup(groupToggleDataPaths, "Source paths");

            if (groupToggleDataPaths)
            {
                // OpenRCT2
                openrct2DataPath = EditorGUILayout.TextField("OpenRCT2 data path", openrct2DataPath);

                if (string.IsNullOrWhiteSpace(openrct2DataPath))
                    EditorGUILayout.HelpBox("The OpenRCT2 data path is not specified! Point it to the 'data' folder for OpenRCT2.", MessageType.Error);
                else if (!Directory.Exists(openrct2DataPath))
                {
                    string defaultPath = GetDefaultOpenRCT2DataPath();
                    if (openrct2DataPath == defaultPath)
                        EditorGUILayout.HelpBox($"The default OpenRCT2 data path does not exist:\n'{openrct2DataPath}'\n\nYou need to succesfully build the C++ OpenRCT2 project at least one to use this data folder.", MessageType.Error);
                    else
                        EditorGUILayout.HelpBox($"The specified OpenRCT2 data path does not exist:\n'{openrct2DataPath}'\n\nPoint it to the 'data' folder for OpenRCT2.", MessageType.Error);
                }

                // RCT2 path
                rct2Path = EditorGUILayout.TextField("RCT2 path", rct2Path);

                if (string.IsNullOrWhiteSpace(rct2Path))
                    EditorGUILayout.HelpBox("The RCT2 path is not specified! Point it to the folder where RCT2 is installed.", MessageType.Error);
                else if (!Directory.Exists(rct2Path))
                    EditorGUILayout.HelpBox($"The specified RCT2 path does not exist:\n'{rct2Path}'\n\nPoint it to the folder where RCT2 is installed.", MessageType.Error);

                // RCT1 path
                rct1Path = EditorGUILayout.TextField("RCT1 path (optional)", rct1Path);

                if (!string.IsNullOrWhiteSpace(rct1Path) && !Directory.Exists(rct1Path))
                    EditorGUILayout.HelpBox($"The specified RCT1 path does not exist:\n{rct1Path}\n\nPoint it to the folder where RCT1 is installed or leave it empty.", MessageType.Error);

                // Parks path
                parkPath = EditorGUILayout.TextField("Parks path", parkPath);

                if (string.IsNullOrWhiteSpace(parkPath))
                    EditorGUILayout.HelpBox("The parks path is not specified! Point it to a folder where your parks are located.", MessageType.Error);
                else if (!Directory.Exists(parkPath))
                    EditorGUILayout.HelpBox($"The specified parks path does not exist:\n'{parkPath}'\n\nPoint it to the folder where your parks are located.", MessageType.Error);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            // Select a park
            groupToggleSelectedPark = EditorGUILayout.BeginFoldoutHeaderGroup(groupToggleSelectedPark, "Selected park");

            if (groupToggleSelectedPark)
            {
                bool parkFoundError = false;
                if (allDiscoveredParks == null)
                {
                    // Search for all park files in the specified park folder...
                    DirectoryInfo parkDirectory = new DirectoryInfo(parkPath);
                    if (parkDirectory.Exists)
                    {
                        int cutoff = parkDirectory.FullName.Length + 1;

                        allDiscoveredParks = parkDirectory
                            .EnumerateFiles("*.*", SearchOption.AllDirectories)
                            .Where(HasValidParkExtension)
                            .Select(e => e
                                .FullName
                                .Substring(cutoff)
                                .Replace('\\', '/')) // switch slash-type, so Unity recognizes it as folders.
                            .ToArray();

                        OpenRCT2 game = (OpenRCT2)target;
                        string selected = game.selectedPark;

                        selectedParkIndex = Array.IndexOf(allDiscoveredParks, selected);

                        if (selectedParkIndex != -1)
                            game.selectedPark = allDiscoveredParks[selectedParkIndex];
                    }
                    else
                        parkFoundError = true;
                }

                // Set the current selected park + update if another is selected
                if (!parkFoundError)
                {
                    int currentSelection = selectedParkIndex;
                    currentSelection = EditorGUILayout.Popup(currentSelection, allDiscoveredParks);

                    if (currentSelection != selectedParkIndex)
                    {
                        selectedParkIndex = currentSelection;

                        OpenRCT2 game = (OpenRCT2)target;
                        game.selectedPark = allDiscoveredParks[currentSelection];
                    }
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }


        /// <summary>
        /// Gets the root of the Unity project/executable.
        /// </summary>
        DirectoryInfo FindUnityRootFolder()
        {
            return Directory.GetParent(Application.dataPath);
        }


        /// <summary>
        /// Gets the root of the OpenRCT2 local repository.
        /// </summary>
        /// <returns></returns>
        DirectoryInfo FindOpenRCT2RootFolder()
        {
            DirectoryInfo unityRoot = FindUnityRootFolder();

            return unityRoot.Parent.Parent;
        }


        /// <summary>
        /// Gets the default for trying to find the openrct2 data folder.
        /// </summary>
        string GetDefaultOpenRCT2DataPath()
        {
            DirectoryInfo openrct2Root = FindOpenRCT2RootFolder();
            return Path.Combine(openrct2Root.FullName, DefaultOpenRCT2Path);
        }


        /// <summary>
        /// Returns whether the file entry has a valid extension.
        /// </summary>
        bool HasValidParkExtension(FileSystemInfo fileSystemInfo)
            => ValidParkExtensions.Any(e => e == fileSystemInfo.Extension);
    }
}
