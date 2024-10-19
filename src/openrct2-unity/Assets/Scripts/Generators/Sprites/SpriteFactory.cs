using System;
using System.Collections.Generic;
using OpenRCT2.Bindings.Graphics;

#nullable enable

namespace OpenRCT2.Generators.Sprites
{
    /// <summary>
    /// Factory for graphics related things in OpenRCT2.
    /// </summary>
    public static class SpriteFactory
    {
        const byte NoColour = 255;

        // A cache of previously generated graphics.
        static readonly Dictionary<ulong, SpriteTexture> _spriteCache = new();

        // Empty sprite texture for blank sprites.
        static SpriteTexture? _empty;


        /// <summary>
        /// Returns the (potentially cached) graphic for the given image index, with optional colouring.
        /// </summary>
        public static SpriteTexture GetOrCreate(uint imageIndex, byte colour1 = NoColour, byte colour2 = NoColour, byte colour3 = NoColour)
        {
            var key = GetCacheKey(imageIndex, colour1, colour2, colour3);

            if (_spriteCache.TryGetValue(key, out SpriteTexture sprite))
            {
                return sprite;
            }

            sprite = Create(imageIndex, colour1, colour2, colour3);
            _spriteCache.Add(key, sprite);

            return sprite;
        }

        /// <summary>
        /// Returns a new graphic for the given image index, with optional colouring.
        /// </summary>
        public static SpriteTexture Create(uint imageIndex, byte colour1 = NoColour, byte colour2 = NoColour, byte colour3 = NoColour)
        { 
            // Retrieve texture in bytes
            SpriteData data = GraphicsDataFactory.GetTextureData(imageIndex);

            int total = data.PixelCount;
            if (total == 0)
            {
                return _empty ??= new SpriteTexture(0, default, Array.Empty<byte>());
            }

            byte[] byteBuffer = new byte[total];
            GraphicsDataFactory.GetTexturePixels(imageIndex, colour1, colour2, colour3, byteBuffer);

            return new SpriteTexture(imageIndex, data, byteBuffer);
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
                frames[i] = GetOrCreate(imageIndices[i]);
            }
            return frames;
        }

        static ulong GetCacheKey(uint imageIndex, byte colour1, byte colour2, byte colour3)
        {
            var colours = (colour1 | colour2 << 8 | colour3 << 16);
            return ((ulong)colours << 32 | imageIndex);
        }
    }
}
