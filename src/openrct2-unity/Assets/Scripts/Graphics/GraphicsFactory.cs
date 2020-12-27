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
        static readonly PaletteEntry[] _palette = OpenRCT2.GetPalette();

        // A cache of previously generated graphics.
        static readonly Dictionary<uint, Graphic> _graphicCache = new Dictionary<uint, Graphic>();


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
        public static Graphic ForImageIndex(uint imageIndex)
        {
            if (_graphicCache.TryGetValue(imageIndex, out Graphic graphic))
            {
                return graphic;
            }

            // Retrieve texture in bytes
            SpriteData data = OpenRCT2.GetTextureData(imageIndex);

            int total = data.PixelCount;
            if (total == 0)
            {
                Debug.LogError($"Sprite at index '{imageIndex}' has 0 pixels.");
                return null;
            }

            byte[] byteBuffer = new byte[total];
            OpenRCT2.GetTexturePixels(imageIndex, byteBuffer);

            graphic = new Graphic(imageIndex, data, byteBuffer);
            _graphicCache.Add(imageIndex, graphic);

            return graphic;
        }


        /// <summary>
        /// Returns an array of graphics as an animation, for the given image indices.
        /// </summary>
        public static Graphic[] ForAnimationIndices(params uint[] imageIndices)
        {
            int frameCount = imageIndices.Length;
            Graphic[] frames = new Graphic[frameCount];

            for (int i = 0; i < frameCount; i++)
            {
                frames[i] = ForImageIndex(imageIndices[i]);
            }
            return frames;
        }
    }
}
