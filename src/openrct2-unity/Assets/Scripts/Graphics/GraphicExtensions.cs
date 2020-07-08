using System.Collections.Generic;
using UnityEngine;

namespace Graphics
{
    /// <summary>
    /// Extensions for the Graphic struct.
    /// </summary>
    public static class GraphicExtensions
    {
        // A cache of previously generated textures.
        static readonly Dictionary<uint, Texture2D> textureCache = new Dictionary<uint, Texture2D>();


        /// <summary>
        /// Converts a Graphic to a Unity Texture2D.
        /// </summary>
        public static Texture2D ToTexture2D(this Graphic graphic, TextureWrapMode wrapMode = TextureWrapMode.Clamp)
        {
            // Check cache first for this graphic.
            uint imageIndex = graphic.ImageIndex;
            if (imageIndex == 0)
                return null;

            if (textureCache.TryGetValue(imageIndex, out Texture2D texture))
                return texture;

            int pixelCount = graphic.PixelCount;
            if (pixelCount == 0)
                return null;

            int width = graphic.Width;
            int height = graphic.Height;

            // Convert to color and mirror sprite vertically
            Color[] colors = new Color[pixelCount];
            for (int outRow = 0, inRow = (pixelCount - width); outRow < pixelCount; outRow += width, inRow -= width)
            {
                for (int column = 0; column < width; ++column)
                {
                    colors[outRow + column] = GraphicsFactory.PaletteToColor(graphic.Data[inRow + column]);
                }
            }

            // Export as Texture2D image.
            texture = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false)
            {
                name = $"i:{imageIndex}",
                filterMode = FilterMode.Point,
                wrapMode = wrapMode
            };
            texture.SetPixels(colors);
            texture.Apply();

            textureCache.Add(imageIndex, texture);
            return texture;
        }
    }
}
