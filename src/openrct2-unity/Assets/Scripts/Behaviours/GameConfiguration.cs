using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours.Editor
{
    /// <summary>
    /// Static class to access OpenRCT2-Unity configuration.
    /// </summary>
    public static class GameConfiguration
    {
        // Configuration keys
        const string _openRCT2DataPathKey = "OpenRCT2DataPath";
        const string _rct2PathKey = "RCT2Path";
        const string _rct1PathKey = "RCT1Path";
        const string _parkPathKey = "ParkPath";


        /// <summary>
        /// Location of the OpenRCT2 data folder.
        /// </summary>
        public static string? OpenRCT2DataPath
        {
            get => PlayerPrefs.GetString(_openRCT2DataPathKey);
            set => PlayerPrefs.SetString(_openRCT2DataPathKey, value);
        }


        /// <summary>
        /// Location of the RCT2 installation.
        /// </summary>
        public static string? RCT2Path
        {
            get => PlayerPrefs.GetString(_rct2PathKey);
            set => PlayerPrefs.SetString(_rct2PathKey, value);
        }


        /// <summary>
        /// Location of the RCT1 installation.
        /// </summary>
        public static string? RCT1Path
        {
            get => PlayerPrefs.GetString(_rct1PathKey);
            set => PlayerPrefs.SetString(_rct1PathKey, value);
        }


        /// <summary>
        /// Location of where to look for parks.
        /// </summary>
        public static string? ParkPath
        {
            get => PlayerPrefs.GetString(_parkPathKey);
            set => PlayerPrefs.SetString(_parkPathKey, value);
        }
    }
}
