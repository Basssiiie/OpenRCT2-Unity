using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours.Editor
{
    /// <summary>
    /// An editor that helps configuring OpenRCT2 settings in Unity, for example
    /// the paths to the games, data folders or parks.
    /// </summary>
    [CustomEditor(typeof(GameScript))]
    public class GameEditor : UnityEditor.Editor
    {
        const string _defaultOpenRCT2Path = @"bin\data"; // from repo root
        const string _defaultParkPath = @"Parks"; // from unity project/executable root

        static readonly string[] _validParkExtensions = { ".park", ".sv6", ".sc6", ".sv4", ".sc4" };


        bool _groupToggleDataPaths = true;
        string? _openrct2DataPath;
        string? _rct2Path;
        string? _rct1Path;
        string? _parkPath;

        bool _groupToggleSelectedPark = true;
        string[]? _allDiscoveredParks;
        int _selectedParkIndex = -1;


        void OnEnable()
        {
            _openrct2DataPath = GameConfiguration.OpenRCT2DataPath;

            // If openrct2 path is null, take the default bin folder in the project.
            if (string.IsNullOrWhiteSpace(_openrct2DataPath))
                _openrct2DataPath = GetDefaultOpenRCT2DataPath();

            _rct2Path = GameConfiguration.RCT2Path;
            _rct1Path = GameConfiguration.RCT1Path;

            _parkPath = GameConfiguration.ParkPath;

            // If park path is null, take the default park folder in the project.
            if (string.IsNullOrWhiteSpace(_parkPath))
            {
                DirectoryInfo unityRoot = FindUnityRootFolder();
                _parkPath = Path.Combine(unityRoot.FullName, _defaultParkPath);
            }
        }


        void OnDisable()
        {
            GameConfiguration.OpenRCT2DataPath = _openrct2DataPath;
            GameConfiguration.RCT2Path = _rct2Path;
            GameConfiguration.RCT1Path = _rct1Path;
            GameConfiguration.ParkPath = _parkPath;
        }


        /// <summary>
        /// This gets called every UI update.
        /// </summary>
        public override void OnInspectorGUI()
        {
            _groupToggleDataPaths = EditorGUILayout.BeginFoldoutHeaderGroup(_groupToggleDataPaths, "Source paths");

            if (_groupToggleDataPaths)
            {
                // OpenRCT2
                _openrct2DataPath = EditorGUILayout.TextField("OpenRCT2 data path", _openrct2DataPath);

                if (string.IsNullOrWhiteSpace(_openrct2DataPath))
                    EditorGUILayout.HelpBox("The OpenRCT2 data path is not specified! Point it to the 'data' folder for OpenRCT2.", MessageType.Error);
                else if (!Directory.Exists(_openrct2DataPath))
                {
                    string defaultPath = GetDefaultOpenRCT2DataPath();
                    if (_openrct2DataPath == defaultPath)
                        EditorGUILayout.HelpBox($"The default OpenRCT2 data path does not exist:\n'{_openrct2DataPath}'\n\nYou need to succesfully build the C++ OpenRCT2 project at least one to use this data folder.", MessageType.Error);
                    else
                        EditorGUILayout.HelpBox($"The specified OpenRCT2 data path does not exist:\n'{_openrct2DataPath}'\n\nPoint it to the 'data' folder for OpenRCT2.", MessageType.Error);
                }

                // RCT2 path
                _rct2Path = EditorGUILayout.TextField("RCT2 path", _rct2Path);

                if (string.IsNullOrWhiteSpace(_rct2Path))
                    EditorGUILayout.HelpBox("The RCT2 path is not specified! Point it to the folder where RCT2 is installed.", MessageType.Error);
                else if (!Directory.Exists(_rct2Path))
                    EditorGUILayout.HelpBox($"The specified RCT2 path does not exist:\n'{_rct2Path}'\n\nPoint it to the folder where RCT2 is installed.", MessageType.Error);

                // RCT1 path
                _rct1Path = EditorGUILayout.TextField("RCT1 path (optional)", _rct1Path);

                if (!string.IsNullOrWhiteSpace(_rct1Path) && !Directory.Exists(_rct1Path))
                    EditorGUILayout.HelpBox($"The specified RCT1 path does not exist:\n{_rct1Path}\n\nPoint it to the folder where RCT1 is installed or leave it empty.", MessageType.Error);

                // Parks path
                _parkPath = EditorGUILayout.TextField("Parks path", _parkPath);

                if (string.IsNullOrWhiteSpace(_parkPath))
                    EditorGUILayout.HelpBox("The parks path is not specified! Point it to a folder where your parks are located.", MessageType.Error);
                else if (!Directory.Exists(_parkPath))
                    EditorGUILayout.HelpBox($"The specified parks path does not exist:\n'{_parkPath}'\n\nPoint it to the folder where your parks are located.", MessageType.Error);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            // Select a park
            _groupToggleSelectedPark = EditorGUILayout.BeginFoldoutHeaderGroup(_groupToggleSelectedPark, "Selected park");

            if (_groupToggleSelectedPark)
            {
                bool parkFoundError = false;
                if (_allDiscoveredParks == null)
                {
                    // Search for all park files in the specified park folder...
                    DirectoryInfo parkDirectory = new DirectoryInfo(_parkPath);
                    if (!parkDirectory.Exists)
                    {
                        parkFoundError = true;
                    }
                    else
                    {
                        int cutoff = parkDirectory.FullName.Length + 1;

                        _allDiscoveredParks = parkDirectory
                            .EnumerateFiles("*.*", SearchOption.AllDirectories)
                            .Where(HasValidParkExtension)
                            .Select(e => e
                                .FullName
                                .Substring(cutoff)
                                .Replace('\\', '/')) // switch slash-type, so Unity recognizes it as folders.
                            .ToArray();

                        GameScript game = (GameScript)target;
                        string? selected = game.selectedPark;

                        if (!string.IsNullOrEmpty(selected))
                        {
                            _selectedParkIndex = Array.IndexOf(_allDiscoveredParks, selected);

                            if (_selectedParkIndex != -1)
                            {
                                game.selectedPark = _allDiscoveredParks[_selectedParkIndex];
                            }
                        }
                    }
                }

                // Set the current selected park + update if another is selected
                if (!parkFoundError && _allDiscoveredParks != null)
                {
                    int currentSelection = _selectedParkIndex;
                    currentSelection = EditorGUILayout.Popup(currentSelection, _allDiscoveredParks);

                    if (currentSelection != _selectedParkIndex)
                    {
                        _selectedParkIndex = currentSelection;

                        GameScript game = (GameScript)target;
                        Undo.RecordObject(game, "Select park");

                        game.selectedPark = _allDiscoveredParks[currentSelection];
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
            return Path.Combine(openrct2Root.FullName, _defaultOpenRCT2Path);
        }


        /// <summary>
        /// Returns whether the file entry has a valid extension.
        /// </summary>
        bool HasValidParkExtension(FileSystemInfo fileSystemInfo)
            => _validParkExtensions.Any(e => string.Equals(e, fileSystemInfo.Extension, StringComparison.InvariantCultureIgnoreCase));
    }
}
