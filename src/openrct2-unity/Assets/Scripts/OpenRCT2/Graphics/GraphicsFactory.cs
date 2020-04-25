using System.Collections.Generic;
using UnityEngine;

namespace OpenRCT2.Unity
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
        /// Returns a Graphic from the specified tile element.
        /// </summary>
        public static Graphic ForTileElement(ref TileElement tileElement)
        {
            // Retrieve texture information
            SpriteSize size = new SpriteSize();
            uint imageIndex = OpenRCT2.GetTileElementTextureInfo(tileElement, 0, ref size);

            if (graphicCache.TryGetValue(imageIndex, out Graphic graphic))
            {
                return graphic;
            }

            // Retrieve texture in bytes
            int total = size.Total;
            byte[] byteBuffer = new byte[total];
            OpenRCT2.GetTexture(imageIndex, byteBuffer, total);

            graphic = new Graphic(imageIndex, size.width, size.height, byteBuffer);
            graphicCache.Add(imageIndex, graphic);

            return graphic;
        }
    }
}
