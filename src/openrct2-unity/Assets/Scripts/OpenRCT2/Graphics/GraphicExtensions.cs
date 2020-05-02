using System.Collections.Generic;
using UnityEngine;

namespace OpenRCT2.Unity
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
        public static Texture2D ToTexture2D(this Graphic graphic, TextureWrapMode wrapMode)
        {
            // Check cache first for this graphic.
            uint imageIndex = graphic.ImageIndex;

            if (textureCache.TryGetValue(imageIndex, out Texture2D texture))
            {
                return texture;
            }

            int count = graphic.PixelCount;

            // Convert to color and do 180 degrees rotate
            Color[] colors = new Color[count];
            for (int i = 0, c = count - 1; i < count; i++, c--)
            {
                colors[c] = GraphicsFactory.PaletteToColor(graphic.Data[i]);
            }

            texture = new Texture2D(graphic.Width, graphic.Height, TextureFormat.RGBA32, mipChain: false)
            {
                name = imageIndex.ToString(),
                filterMode = FilterMode.Point,
                wrapMode = wrapMode
            };
            texture.SetPixels(colors);
            texture.Apply(true);

            textureCache.Add(imageIndex, texture);
            return texture;
        }
    }
}
