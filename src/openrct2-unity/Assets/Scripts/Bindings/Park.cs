using System;
using System.Runtime.InteropServices;
using OpenRCT2.Bindings.TileElements;

#nullable enable

namespace OpenRCT2.Bindings
{
    public static class Park
    {
        /// <summary>
        /// Returns the name of the currently loaded park.
        /// </summary>
        public static string GetName()
            => Marshal.PtrToStringAnsi(GetParkName());


        /// <summary>
        /// Gets the amount of tiles from one side of the map to the other side.
        /// </summary>
        public static MapSize GetMapSize()
        {
            GetMapSize(out MapSize size);
            return size;
        }


        /// <summary>
        /// Loads all the tile elements on the specified tile coordinate into the buffer.
        /// </summary>
        public static int GetMapElementsAt(int x, int y, TileElementInfo[] elements)
            => Tile.GetMapElementsAt(x, y, elements, elements.Length);


        /// <summary>
        /// Retrieves information about the small scenery tile element on the specified tile.
        /// </summary>
        public static SmallSceneryInfo GetSmallSceneryElementAt(int x, int y, int index)
        {
            GetSmallSceneryElementAt(x, y, index, out SmallSceneryInfo element);
            return element;
        }


        /// <summary>
        /// Retrieves information about the surface tile element on the specified tile.
        /// </summary>
        public static SurfaceInfo GetSurfaceElementAt(int x, int y, int index)
        {
            GetSurfaceElementAt(x, y, index, out SurfaceInfo element);
            return element;
        }


        /// <summary>
        /// Retrieves information about the track tile element on the specified tile.
        /// </summary>
        public static TrackInfo GetTrackElementAt(int x, int y, int index)
        {
            GetTrackElementAt(x, y, index, out TrackInfo element);
            return element;
        }


        /// <summary>
        /// Retrieves information about the path tile element on the specified tile.
        /// </summary>
        public static PathInfo GetPathElementAt(int x, int y, int index)
        {
            GetPathElementAt(x, y, index, out PathInfo element);
            return element;
        }


        /// <summary>
        /// Retrieves information about the wall tile element on the specified tile.
        /// </summary>
        public static WallInfo GetWallElementAt(int x, int y, int index)
        {
            GetWallElementAt(x, y, index, out WallInfo element);
            return element;
        }


        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr GetParkName();


        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl)]
        static extern int GetMapSize(out MapSize size);


        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void GetSmallSceneryElementAt(int x, int y, int index, out SmallSceneryInfo element);


        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void GetSurfaceElementAt(int x, int y, int index, out SurfaceInfo element);


        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void GetTrackElementAt(int x, int y, int index, out TrackInfo element);


        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void GetPathElementAt(int x, int y, int index, out PathInfo element);


        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void GetWallElementAt(int x, int y, int index, out WallInfo element);
    }
}
