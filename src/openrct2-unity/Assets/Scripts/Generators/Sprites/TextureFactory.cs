using System;
using System.Collections.Generic;
using OpenRCT2.Utilities;
using UnityEngine;

namespace OpenRCT2.Generators.Sprites
{
    public static class TextureFactory
    {
        public static Texture2D CreatePaletted(SpriteTexture sprite)
        {
            Assert.IsTrue(sprite.pixelCount > 0);

            uint imageIndex = sprite.imageIndex;
            int width = sprite.width;
            int height = sprite.height;

            // Export as Texture2D image.
            var texture = new Texture2D(width, height, TextureFormat.R8, mipChain: false, linear: true, createUninitialized: true)
            {
                name = $"sprite({imageIndex})",
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            texture.SetPixelData(sprite.colorData, 0);
            texture.Apply(updateMipmaps: false, makeNoLongerReadable: true);
            return texture;
        }

        public static Texture2DArray CreatePalettedArray(IReadOnlyList<SpriteTexture> sprites)
        {
            var bounds = SpriteBounds.From(sprites);

            int width = bounds.width;
            int height = bounds.height;
            Assert.IsTrue(width > 0 && height > 0);

            var length = sprites.Count;

            // Export as Texture2D image array.
            var texture = new Texture2DArray(width, height, length, TextureFormat.R8, mipChain: false, linear: true, createUninitialized: true)
            {
                name = $"spritemap({length})",
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            var buffer = new byte[width * height];

            for (var i = 0; i < length; i++)
            {
                sprites[i].CopyTo(buffer, in bounds);
                texture.SetPixelData(buffer, 0, i);

                Array.Clear(buffer, 0, buffer.Length);
            }

            texture.Apply(updateMipmaps: false, makeNoLongerReadable: true);
            return texture;
        }


        /// <summary>
        /// Gets the texture for this graphic.
        /// </summary>
        public static Texture2D CreateFullColour(SpriteTexture sprite, TextureWrapMode wrapMode = TextureWrapMode.Clamp, bool makeTextureReadable = false)
        {
            Assert.IsTrue(sprite.pixelCount > 0);

            uint imageIndex = sprite.imageIndex;
            int pixelCount = sprite.pixelCount;
            int width = sprite.width;
            int height = sprite.height;

            // Convert to color and mirror sprite vertically
            Color32[] colors = new Color32[pixelCount];
            for (int row = 0; row < pixelCount; row += width)
            {
                for (int column = 0; column < width; ++column)
                {
                    colors[row + column] = PaletteFactory.IndexToColor(sprite.colorData[row + column]);
                }
            }

            // Export as Texture2D image.
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false, linear: true, createUninitialized: true)
            {
                name = $"sprite{imageIndex & 0x7FFFF}(unmasked{imageIndex})",
                filterMode = FilterMode.Point,
                wrapMode = wrapMode
            };
            texture.SetPixels32(colors);
            texture.Apply(updateMipmaps: false, makeNoLongerReadable: !makeTextureReadable);
            return texture;
        }
    }
}
