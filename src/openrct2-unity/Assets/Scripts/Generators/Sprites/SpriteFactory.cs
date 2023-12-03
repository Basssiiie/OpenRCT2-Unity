using System;
using System.Collections.Generic;
using OpenRCT2.Bindings.Graphics;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Sprites
{
    /// <summary>
    /// Factory for graphics related things in OpenRCT2.
    /// </summary>
    public static class SpriteFactory
    {
        static readonly PaletteEntry[] _palette = GraphicsDataFactory.GetPalette();

        // A cache of previously generated graphics.
        static readonly Dictionary<uint, SpriteTexture> _spriteCache = new Dictionary<uint, SpriteTexture>();


        /// <summary>
        /// Returns a Unity color for the specified RCT2 color index on the palette.
        /// </summary>
        public static Color32 PaletteToColor(ushort colorIndex)
        {
            return _palette[colorIndex].ToColor32();
        }


        /// <summary>
        /// Returns the (potentially cached) graphic for the given image index.
        /// </summary>
        public static SpriteTexture ForImageIndex(uint imageIndex)
        {
            if (_spriteCache.TryGetValue(imageIndex, out SpriteTexture sprite))
            {
                return sprite;
            }

            // Retrieve texture in bytes
            SpriteData data = GraphicsDataFactory.GetTextureData(imageIndex);

            int total = data.PixelCount;
            if (total == 0)
            {
                Debug.LogError($"Sprite at index '{imageIndex}' (masked: {imageIndex & 0x7FFFF}) has 0 pixels.");
                sprite = new SpriteTexture(imageIndex, data, Array.Empty<byte>());
            }
            else
            {
                byte[] byteBuffer = new byte[total];
                GraphicsDataFactory.GetTexturePixels(imageIndex, byteBuffer);

                sprite = new SpriteTexture(imageIndex, data, byteBuffer);
            }
            _spriteCache.Add(imageIndex, sprite);

            return sprite;
        }


        /// <summary>
        /// Returns an array of graphics as an animation, for the given image indices.
        /// </summary>
        public static SpriteTexture[] ForAnimationIndices(params uint[] imageIndices)
        {
            int frameCount = imageIndices.Length;
            SpriteTexture[] frames = new SpriteTexture[frameCount];

            for (int i = 0; i < frameCount; i++)
            {
                frames[i] = ForImageIndex(imageIndices[i]);
            }
            return frames;
        }
    }
}
