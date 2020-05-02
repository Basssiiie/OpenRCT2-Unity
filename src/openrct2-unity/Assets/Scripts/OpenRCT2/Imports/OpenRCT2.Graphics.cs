using System.Runtime.InteropServices;

namespace OpenRCT2.Unity
{
    public partial class OpenRCT2
    {
        // Amount of colors in a RCT palette.
        const int PalleteSize = 256;


        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        static extern void GetPalette([Out] PaletteEntry[] entries);


        /// <summary>
        /// Returns all the palette color entries.
        /// </summary>
        internal static PaletteEntry[] GetPalette()
        {
            PaletteEntry[] entries = new PaletteEntry[PalleteSize];
            GetPalette(entries);

            return entries;
        }


        /// <summary>
        /// Gets the image entry for the specified surface tile.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint GetSurfaceImageIndex(TileElement tileElement, int tileX, int tileY, byte direction);


        /// <summary>
        /// Gets the image entry for the specified small scenery tile.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint GetSmallSceneryImageIndex(TileElement tileElement, byte direction);


        /// <summary>
        /// Gets the texture size for the specified image index.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        static extern void GetTextureSize(uint imageIndex, ref SpriteSize textureSize);


        /// <summary>
        /// Gets the texture size for the specified image index.
        /// </summary>
        public static SpriteSize GetTextureSize(uint imageIndex)
        {
            SpriteSize size = new SpriteSize();
            GetTextureSize(imageIndex, ref size);
            return size;
        }


        /// <summary>
        /// Gets the pixel bytes of a texture specified by its image index.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        static extern void GetTexturePixels(uint imageIndex, [Out] byte[] bytes, int amountOfBytes);


        /// <summary>
        /// Writes the pixel bytes of a texture specified by its image index to the buffer.
        /// </summary>
        public static void GetTexturePixels(uint imageIndex, byte[] buffer)
            => GetTexturePixels(imageIndex, buffer, buffer.Length);


        /// <summary>
        /// Gets the pixel bytes of a texture specified by its image index.
        /// </summary>
        public static byte[] GetTexturePixels(uint imageIndex)
        {
            SpriteSize size = GetTextureSize(imageIndex);

            int total = size.Total;
            byte[] byteBuffer = new byte[total];
            GetTexturePixels(imageIndex, byteBuffer, total);
            return byteBuffer;
        }
    }
}
