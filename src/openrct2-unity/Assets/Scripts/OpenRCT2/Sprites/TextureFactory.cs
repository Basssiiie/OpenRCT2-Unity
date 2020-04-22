using UnityEngine;

namespace OpenRCT2.Unity
{
    public static class TextureFactory
    {
        static readonly PaletteEntry[] palette = OpenRCT2.GetPalette();


        /// <summary>
        /// Returns a Unity color for the specified RCT2 color index on the palette.
        /// </summary>
        public static Color PaletteToColor(ushort colorIndex)
        {
            return palette[colorIndex].Color;
        }


        /// <summary>
        /// Returns a Texture2D from the specified tile element.
        /// </summary>
        public static Texture2D ForTileElement(ref TileElement tileElement)
        {
            // Retrieve texture information
            SpriteSize size = new SpriteSize();
            uint imageIndex = OpenRCT2.GetTileElementTextureInfo(tileElement, 0, ref size);

            // Retrieve texture in bytes
            int total = size.Total;
            byte[] byteBuffer = new byte[total];
            OpenRCT2.GetTexture(imageIndex, byteBuffer, total);

            // Convert to color and do 180 degrees rotate
            Color[] colors = new Color[total];
            for (int i = 0, c = total - 1; i < total; i++, c--)
            {
                colors[c] = PaletteToColor(byteBuffer[i]);
            }

            Texture2D texture = new Texture2D(size.width, size.height);
            texture.SetPixels(colors);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply(true);
            return texture;
        }
    }
}
