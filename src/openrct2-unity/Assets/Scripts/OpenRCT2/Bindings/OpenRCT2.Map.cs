using System.Drawing;
using System.Dynamic;
using System.Runtime.InteropServices;

#nullable enable

namespace Lib
{
    public partial class OpenRCT2
    {
        /// <summary>
        /// Gets the amount of tiles from one side of the map to the other side.
        /// </summary>
        public static MapSize GetMapSize()
        {
            var size = new MapSize();
            GetMapSize(ref size);
            return size;
        }

        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        static extern int GetMapSize(ref MapSize size);


        /// <summary>
        /// Gets the first map element that is found at the specified tile coordinate.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        static extern void GetMapElementAt(int x, int y, ref TileElementInfo element);


        /// <summary>
        /// Loads all the tile elements on the specified tile coordinate into the buffer.
        /// </summary>
        public static int GetMapElementsAt(int x, int y, TileElementInfo[] elements)
            => GetMapElementsAt(x, y, elements, elements.Length);

        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        static extern int GetMapElementsAt(int x, int y, [Out] TileElementInfo[] elements, int arraySize);


        /// <summary>
        /// Retrieves information about the small scenery tile element on the specified tile.
        /// </summary>
        public static SmallSceneryInfo GetSmallSceneryElementAt(int x, int y, int index)
        {
            var element = new SmallSceneryInfo();
            GetSmallSceneryElementAt(x, y, index, ref element);
            return element;
        }

        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void GetSmallSceneryElementAt(int x, int y, int index, ref SmallSceneryInfo element);


        /// <summary>
        /// Retrieves information about the surface tile element on the specified tile.
        /// </summary>
        public static SurfaceInfo GetSurfaceElementAt(int x, int y, int index)
        {
            var element = new SurfaceInfo();
            GetSurfaceElementAt(x, y, index, ref element);
            return element;
        }

        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void GetSurfaceElementAt(int x, int y, int index, ref SurfaceInfo element);


        /// <summary>
        /// Retrieves information about the track tile element on the specified tile.
        /// </summary>
        public static TrackInfo GetTrackElementAt(int x, int y, int index)
        {
            var element = new TrackInfo();
            GetTrackElementAt(x, y, index, ref element);
            return element;
        }

        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void GetTrackElementAt(int x, int y, int index, ref TrackInfo element);


        /// <summary>
        /// Retrieves information about the path tile element on the specified tile.
        /// </summary>
        public static PathInfo GetPathElementAt(int x, int y, int index)
        {
            var element = new PathInfo();
            GetPathElementAt(x, y, index, ref element);
            return element;
        }

        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void GetPathElementAt(int x, int y, int index, ref PathInfo element);


        /// <summary>
        /// Retrieves information about the wall tile element on the specified tile.
        /// </summary>
        public static WallInfo GetWallElementAt(int x, int y, int index)
        {
            var element = new WallInfo();
            GetWallElementAt(x, y, index, ref element);
            return element;
        }

        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void GetWallElementAt(int x, int y, int index, ref WallInfo element);
    }
}
