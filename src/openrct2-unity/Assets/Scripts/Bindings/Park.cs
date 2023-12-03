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
            var size = new MapSize();
            GetMapSize(ref size);
            return size;
        }


        /// <summary>
        /// Loads all the tile elements on the specified tile coordinate into the buffer.
        /// </summary>
        public static int GetMapElementsAt(int x, int y, TileElementInfo[] elements)
            => GetMapElementsAt(x, y, elements, elements.Length);


        /// <summary>
        /// Retrieves information about the small scenery tile element on the specified tile.
        /// </summary>
        public static SmallSceneryInfo GetSmallSceneryElementAt(int x, int y, int index)
        {
            var element = new SmallSceneryInfo();
            GetSmallSceneryElementAt(x, y, index, ref element);
            return element;
        }


        /// <summary>
        /// Retrieves information about the surface tile element on the specified tile.
        /// </summary>
        public static SurfaceInfo GetSurfaceElementAt(int x, int y, int index)
        {
            var element = new SurfaceInfo();
            GetSurfaceElementAt(x, y, index, ref element);
            return element;
        }


        /// <summary>
        /// Retrieves information about the track tile element on the specified tile.
        /// </summary>
        public static TrackInfo GetTrackElementAt(int x, int y, int index)
        {
            var element = new TrackInfo();
            GetTrackElementAt(x, y, index, ref element);
            return element;
        }


        /// <summary>
        /// Retrieves information about the path tile element on the specified tile.
        /// </summary>
        public static PathInfo GetPathElementAt(int x, int y, int index)
        {
            var element = new PathInfo();
            GetPathElementAt(x, y, index, ref element);
            return element;
        }


        /// <summary>
        /// Retrieves information about the wall tile element on the specified tile.
        /// </summary>
        public static WallInfo GetWallElementAt(int x, int y, int index)
        {
            var element = new WallInfo();
            GetWallElementAt(x, y, index, ref element);
            return element;
        }


        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr GetParkName();


        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl)]
        static extern int GetMapSize(ref MapSize size);


        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl)]
        static extern int GetMapElementsAt(int x, int y, [Out] TileElementInfo[] elements, int arraySize);


        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void GetSmallSceneryElementAt(int x, int y, int index, ref SmallSceneryInfo element);


        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void GetSurfaceElementAt(int x, int y, int index, ref SurfaceInfo element);


        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void GetTrackElementAt(int x, int y, int index, ref TrackInfo element);


        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void GetPathElementAt(int x, int y, int index, ref PathInfo element);


        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void GetWallElementAt(int x, int y, int index, ref WallInfo element);
    }
}
