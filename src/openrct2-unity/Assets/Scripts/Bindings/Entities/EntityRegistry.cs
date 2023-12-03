using System.Runtime.InteropServices;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Bindings.Entities
{
    public static class EntityRegistry
    {
        /// <summary>
        /// Returns all peeps in the park.
        /// </summary>
        public static Peep[] GetAllPeeps()
        {
            int spriteCount = GetSpriteCount(SpriteType.Guest) + GetSpriteCount(SpriteType.Staff);
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
        /// Gets the amount of sprites for the specified type currently on the map.
        /// </summary>
        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl)]
        static extern int GetSpriteCount(SpriteType spriteType);


        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl)]
        static extern int GetAllPeeps([Out] Peep[] elements, int arraySize);


        /// <summary>
        /// Gets certain statiscics about this peep, like its hunger or energy.
        /// Returns true if the peep stats were succesfully read, or false if
        /// the peep does not exist (anymore).
        /// </summary>
        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetGuestStats(ushort spriteIndex, ref GuestStats peepStats);


        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl)]
        static extern int GetAllVehicles([Out] Vehicle[] elements, int arraySize);


        /// <summary>
        /// Reads all vehicles in the park into the specified buffer.
        /// </summary>
        public static int GetAllVehicles(Vehicle[] buffer)
            => GetAllVehicles(buffer, buffer.Length);
    }
}
