using System.Runtime.InteropServices;

#nullable enable

namespace OpenRCT2.Bindings.Entities
{
    public static class EntityRegistry
    {
        /// <summary>
        /// Gets the amount of sprites for the specified type currently on the map.
        /// </summary>
        public static int GetCount(EntityType spriteType)
            => GetEntityCount(spriteType);

        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl)]
        static extern int GetEntityCount(EntityType spriteType);


        /// <summary>
        /// Reads all guests in the park into the specified buffer.
        /// </summary>
        public static int GetAllGuests(PeepEntity[] buffer)
            => GetAllGuests(buffer, buffer.Length);

        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl)]
        static extern int GetAllGuests([Out] PeepEntity[] elements, int length);


        /// <summary>
        /// Reads all staff in the park into the specified buffer.
        /// </summary>
        public static int GetAllStaff(PeepEntity[] buffer)
            => GetAllStaff(buffer, buffer.Length);

        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl)]
        static extern int GetAllStaff([Out] PeepEntity[] elements, int length);


        /// <summary>
        /// Gets additional data about the animation group and type combination.
        /// </summary>
        public static PeepAnimationData GetPeepAnimationData(byte group, byte type)
        {
            GetPeepAnimationData(group, type, out PeepAnimationData data);
            return data;
        }

        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl)]
        static extern void GetPeepAnimationData(byte group, byte type, out PeepAnimationData data);


        /// <summary>
        /// Gets certain statiscics about this peep, like its hunger or energy.
        /// Returns true if the peep stats were succesfully read, or false if
        /// the peep does not exist (anymore).
        /// </summary>
        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetGuestStats(ushort spriteIndex, out GuestStats peepStats);

        /// <summary>
        /// Reads all vehicles in the park into the specified buffer.
        /// </summary>
        public static int GetAllVehicles(VehicleEntity[] buffer)
            => GetAllVehicles(buffer, buffer.Length);

        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl)]
        static extern int GetAllVehicles([Out] VehicleEntity[] elements, int arraySize);
    }
}
