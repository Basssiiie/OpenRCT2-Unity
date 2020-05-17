using System.Runtime.InteropServices;

namespace OpenRCT
{
    public partial class OpenRCT2
    {
        /// <summary>
        /// Gets the amount of tiles from one side of the map to the other side.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetMapSize();


        /// <summary>
        /// Gets the first map element that is found at the specified tile coordinate.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        static extern void GetMapElementAt(int x, int y, ref TileElement element);


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
    }
}
