using System.Runtime.InteropServices;
using UnityEngine;

namespace OpenRCT2.Unity
{
    public partial class OpenRCT2
    {
        /// <summary>
        /// Gets the amount of sprites for the specified type currently on the map.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetSpriteCount(SpriteType spriteType);


        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        static extern int GetAllPeeps([Out] Peep[] elements, int arraySize);


        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        static extern int GetAllVehicles([Out] Vehicle[] elements, int arraySize);


        /// <summary>
        /// Returns all peeps in the park.
        /// </summary>
        public static Peep[] GetAllPeeps()
        {
            int spriteCount = GetSpriteCount(SpriteType.Peep);
            Debug.Log($"Peeps found: {spriteCount}");

            Peep[] peeps = new Peep[spriteCount];

            GetAllPeeps(peeps, spriteCount);
            return peeps;
        }


        /// <summary>
        /// Reads all peeps in the park into the specified buffer.
        /// </summary>
        public static int GetAllPeeps(Peep[] buffer)
            => GetAllPeeps(buffer, buffer.Length);


        /// <summary>
        /// Reads all vehicles in the park into the specified buffer.
        /// </summary>
        public static int GetAllVehicles(Vehicle[] buffer)
            => GetAllVehicles(buffer, buffer.Length);
    }
}
