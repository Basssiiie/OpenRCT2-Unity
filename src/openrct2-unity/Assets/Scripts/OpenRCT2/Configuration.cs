using UnityEngine;

namespace OpenRCT
{
    /// <summary>
    /// Static class to access OpenRCT2-Unity configuration.
    /// </summary>
    public static class Configuration
    {
        // Configuration keys
        const string OpenRCT2DataPathKey = "OpenRCT2DataPath";
        const string RCT2PathKey = "RCT2Path";
        const string RCT1PathKey = "RCT1Path";
        const string ParkPathKey = "ParkPath";


        /// <summary>
        /// Location of the OpenRCT2 data folder.
        /// </summary>
        public static string OpenRCT2DataPath
        {
            get => PlayerPrefs.GetString(OpenRCT2DataPathKey);
            set => PlayerPrefs.SetString(OpenRCT2DataPathKey, value);
        }


        /// <summary>
        /// Location of the RCT2 installation.
        /// </summary>
        public static string RCT2Path
        {
            get => PlayerPrefs.GetString(RCT2PathKey);
            set => PlayerPrefs.SetString(RCT2PathKey, value);
        }


        /// <summary>
        /// Location of the RCT1 installation.
        /// </summary>
        public static string RCT1Path
        {
            get => PlayerPrefs.GetString(RCT1PathKey);
            set => PlayerPrefs.SetString(RCT1PathKey, value);
        }


        /// <summary>
        /// Location of where to look for parks.
        /// </summary>
        public static string ParkPath
        {
            get => PlayerPrefs.GetString(ParkPathKey);
            set => PlayerPrefs.SetString(ParkPathKey, value);
        }
    }
}
