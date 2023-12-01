using System.Runtime.InteropServices;

#nullable enable

namespace Lib
{
    public partial class OpenRCT2
    {
        // Amount of colors in a RCT palette.
        const int PaletteSize = 256;


        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        static extern void GetPalette([Out] PaletteEntry[] entries);


        /// <summary>
        /// Returns all the palette color entries.
        /// </summary>
        public static PaletteEntry[] GetPalette()
        {
            PaletteEntry[] entries = new PaletteEntry[PaletteSize];
            GetPalette(entries);
            return entries;
        }


        /// <summary>
        /// Converts any of the colour ids (0-31) to the corresponding palette index (0-255).
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte GetPaletteIndexForColourId(byte colourId);


        /// <summary>
        /// Gets the image entry for a water tile.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetWaterImageIndex();


        /// <summary>
        /// Gets all image entries in the animation for the specified small scenery tile.
        /// </summary>
        public static uint[] GetSmallSceneryAnimationIndices(int x, int y, int index, int count)
        {
            var indices = new uint[count];
            GetSmallSceneryAnimationIndices(x, y, index, indices, count);
            return indices;
        }

        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        static extern int GetSmallSceneryAnimationIndices(int x, int y, int index, [Out] uint[] indices, int arraySize);


        /// <summary>
        /// Gets the texture size for the specified image index.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        static extern void GetTextureData(uint imageIndex, ref SpriteData data);


        /// <summary>
        /// Gets the texture size for the specified image index.
        /// </summary>
        public static SpriteData GetTextureData(uint imageIndex)
        {
            SpriteData data = new SpriteData();
            GetTextureData(imageIndex, ref data);
            return data;
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
            SpriteData data = GetTextureData(imageIndex);

            int total = data.PixelCount;
            byte[] byteBuffer = new byte[total];
            GetTexturePixels(imageIndex, byteBuffer, total);
            return byteBuffer;
        }
    }
}
