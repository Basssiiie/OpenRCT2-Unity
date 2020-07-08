using System.Collections.Generic;
using Lib;
using UnityEngine;

namespace Graphics
{
    /// <summary>
    /// Factory for graphics related things in OpenRCT2.
    /// </summary>
    public static class GraphicsFactory
    {
        static readonly PaletteEntry[] palette = OpenRCT2.GetPalette();

        // A cache of previously generated graphics.
        static readonly Dictionary<uint, Graphic> graphicCache = new Dictionary<uint, Graphic>();


        /// <summary>
        /// Returns a Unity color for the specified RCT2 color index on the palette.
        /// </summary>
        public static Color PaletteToColor(ushort colorIndex)
        {
            return palette[colorIndex].Color;
        }


        /// <summary>
        /// Returns the graphic for the given image index.
        /// </summary>
        public static Graphic ForImageIndex(uint imageIndex)
        {
            if (graphicCache.TryGetValue(imageIndex, out Graphic graphic))
            {
                return graphic;
            }

            // Retrieve texture in bytes
            SpriteData data = OpenRCT2.GetTextureData(imageIndex);

            int total = data.PixelCount;
            if (total == 0)
                return new Graphic();

            byte[] byteBuffer = new byte[total];
            OpenRCT2.GetTexturePixels(imageIndex, byteBuffer);

            graphic = new Graphic(imageIndex, data.width, data.height, byteBuffer);
            graphicCache.Add(imageIndex, graphic);

            return graphic;
        }
    }
}
