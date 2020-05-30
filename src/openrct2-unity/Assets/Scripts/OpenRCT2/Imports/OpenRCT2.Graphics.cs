using System.Runtime.InteropServices;
using UnityEngine;

#pragma warning disable CA1401 // P/Invokes should not be visible -> intended

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
        /// Gets the surface image entry for the specified surface tile.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetSurfaceImageIndex(TileElement tileElement, int tileX, int tileY, byte direction);


        /// <summary>
        /// Gets the surface edge image entry for the specified surface tile.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetSurfaceEdgeImageIndex(TileElement tileElement);


        /// <summary>
        /// Gets the image entry for a water tile.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetWaterImageIndex();


        /// <summary>
        /// Gets the image entry for the specified small scenery tile.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetSmallSceneryImageIndex(TileElement tileElement, byte direction);


        /// <summary>
        /// Gets the image entry for the specified small scenery tile.
        /// </summary>
        [DllImport(PluginFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetWallImageIndex(TileElement tileElement, byte direction);


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
