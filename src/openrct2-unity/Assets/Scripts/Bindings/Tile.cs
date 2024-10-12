using System;
using System.Runtime.InteropServices;
using OpenRCT2.Bindings.TileElements;

namespace OpenRCT2.Bindings
{
    public class Tile
    {
        public readonly int x;
        public readonly int y;

        public readonly TileElementInfo[] elements;
        public readonly SurfaceInfo[] surfaces; // todo how to get z
        public readonly PathInfo[] paths;
        public readonly TrackInfo[] tracks;
        public readonly SmallSceneryInfo[] smallScenery;
        //public readonly EntranceInfo[] entrances;
        public readonly WallInfo[] walls;
        //public readonly LargeSceneryInfo[] largeScenery;
        //public readonly BannerInfo banners;


        Tile(int x, int y)
        {
            this.x = x;
            this.y = y;

            GetElementCounts(x, y, out var counts);
            elements = GetElementsAt<TileElementInfo>(x, y, counts.total, GetMapElementsAt);
            surfaces = GetElementsAt<SurfaceInfo>(x, y, counts.surfaces, GetAllSurfaceElementsAt);
            paths = GetElementsAt<PathInfo>(x, y, counts.paths, GetAllPathElementsAt);
            tracks = GetElementsAt<TrackInfo>(x, y, counts.tracks, GetAllTrackElementsAt);
            smallScenery = GetElementsAt<SmallSceneryInfo>(x, y, counts.smallScenery, GetAllSmallSceneryElementsAt);
            walls = GetElementsAt<WallInfo>(x, y, counts.walls, GetAllWallElementsAt);
        }

        T[] GetElementsAt<T>(int x, int y, int count, Func<int, int, T[], int, int> getter) where T : struct
        {
            if (count == 0)
            {
                return Array.Empty<T>();
            }

            var array = new T[count];
            getter(x, y, array, count);
            return array;
        }

        public static Tile GetAt(int x, int y)
        {
            return new Tile(x, y);
        }


        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl)]
        static extern void GetElementCounts(int x, int y, out TileCounts counts);

        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetMapElementsAt(int x, int y, [Out] TileElementInfo[] elements, int length);

        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern int GetAllSurfaceElementsAt(int x, int y, [Out] SurfaceInfo[] elements, int length);

        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern int GetAllPathElementsAt(int x, int y, [Out] PathInfo[] elements, int length);

        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern int GetAllTrackElementsAt(int x, int y, [Out] TrackInfo[] elements, int length);

        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern int GetAllSmallSceneryElementsAt(int x, int y, [Out] SmallSceneryInfo[] elements, int length);

        [DllImport(Plugin.FileName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern int GetAllWallElementsAt(int x, int y, [Out] WallInfo[] elements, int length);
    }
}
