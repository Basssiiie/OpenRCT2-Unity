using System;
using System.Runtime.InteropServices;
using UnityEngine;

#pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments -> this is deliberate

namespace OpenRCT2.Unity
{
    public partial class OpenRCT2
    {
        const string PluginFile = "openrct2-dll";


        #region Game

        /// <summary>
        /// Starts the game with the specified folder paths paths.
        /// </summary>
        /// <param name="datapath">
        /// The data folder of OpenRCT2.
        /// </param>
        /// <param name="rct2path">
        /// The absolute path to the RCT2 directory.
        /// </param>
        /// <param name="rct1path">
        /// The absolute path to the RCT1 directory.
        /// </param>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void StartGame([MarshalAs(UnmanagedType.LPStr)] string datapath, [MarshalAs(UnmanagedType.LPStr)] string rct2path, [MarshalAs(UnmanagedType.LPStr)] string rct1path);


        /// <summary>
        /// Performs a single game update.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        static extern void PerformGameUpdate();


        /// <summary>
        /// Shuts down the game.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        static extern void StopGame();

        #endregion


        #region Parks

        /// <summary>
        /// Loads a park from the specified path.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void LoadPark([MarshalAs(UnmanagedType.LPStr)] string path);


        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern IntPtr GetParkNamePtr();


        /// <summary>
        /// Returns the name of the currently loaded park.
        /// </summary>
        public static string GetParkName()
            => Marshal.PtrToStringAnsi(GetParkNamePtr());

        #endregion


        #region Map

        /// <summary>
        /// Gets the amount of tiles from one side of the map to the other side.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetMapSize();


        /// <summary>
        /// Gets the first map element that is found at the specified tile coordinate.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetMapElementAt(int x, int y, ref TileElement element);


        /// <summary>
        /// Loads all the tile elements on the specified tile coordinate into the buffer.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        static extern int GetMapElementsAt(int x, int y, [Out] TileElement[] elements, int arraySize);


        /// <summary>
        /// Writes all elements to the given buffer at the specified position.
        /// </summary>
        public static int GetMapElementsAt(int x, int y, TileElement[] elements)
            => GetMapElementsAt(x, y, elements, elements.Length);

        #endregion


        #region Sprites

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
        {
            return GetAllPeeps(buffer, buffer.Length);
        }

        #endregion


        #region Scenery entries

        /// <summary>
        /// Retrieves information about the scenery entry. Only works for the
        /// following types: path, small scenery, wall, large scenery, banner.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        static extern void GetSceneryEntry(TileElementType type, uint entryIndex, ref SmallSceneryEntry entry);


        /// <summary>
        /// Retrieves the entry for the specified small scenery element.
        /// </summary>
        public static SmallSceneryEntry GetSmallSceneryEntry(uint entryIndex)
        {
            SmallSceneryEntry entry = new SmallSceneryEntry();
            GetSceneryEntry(TileElementType.SmallScenery, entryIndex, ref entry);
            return entry;
        }


        #endregion


        #region Textures

        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        static extern void GetPalette([Out] PaletteEntry[] entries);


        /// <summary>
        /// Returns all the palette color entries.
        /// </summary>
        internal static PaletteEntry[] GetPalette()
        {
            PaletteEntry[] entries = new PaletteEntry[256];
            GetPalette(entries);

            entries[0].alpha = 0;

            return entries;
        }


        /// <summary>
        /// Gets the texture size and image entry for the specified tile element and direction.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint GetTileElementTextureInfo(TileElement tileElement, byte direction, ref SpriteSize textureSize);


        /// <summary>
        /// Gets the bytes of a texture specified by its image index.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetTexture(uint imageIndex, [Out] byte[] bytes, int amountOfBytes);

        #endregion
    }


    /// <summary>
    /// Static helper class for pointer information.
    /// </summary>
    public static class Ptr
    {
        /// <summary>
        /// The current pointer size; 8 bytes on 64-bit, or 4 bytes on 32-bit.
        /// </summary>
        public const byte Size =
#if (UNITY_64 || UNITY_EDITOR_64)
            8;
#else
            4;
#endif
    }
}
