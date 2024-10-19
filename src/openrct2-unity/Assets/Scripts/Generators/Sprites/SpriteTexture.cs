using System;
using OpenRCT2.Bindings.Graphics;

#nullable enable

namespace OpenRCT2.Generators.Sprites
{
    /// <summary>
    /// A small object that contains RCT graphic information.
    /// </summary>
    public class SpriteTexture
    {
        /// <summary>
        /// The image index of the specific graphic, that is used in the OpenRCT2 lib.
        /// </summary>
        public readonly uint imageIndex;

        /// <summary>
        /// Width of the graphic in pixels.
        /// </summary>
        public readonly int width;

        /// <summary>
        /// Height of the graphic in pixels.
        /// </summary>
        public readonly int height;

        /// <summary>
        /// X offset of the graphic in pixels.
        /// </summary>
        public readonly int offsetX;

        /// <summary>
        /// Y offset of the graphic in pixels.
        /// </summary>
        public readonly int offsetY;

        /// <summary>
        /// The actual byte data of the graphic. Each byte represents a palette color index.
        /// </summary>
        public readonly byte[] colorData;

        /// <summary>
        /// Returns the total amount of pixels in this graphic.
        /// </summary>
        public int pixelCount => (width * height);


        /// <summary>
        /// Creates a new graphic structure based on the input.
        /// </summary>
        public SpriteTexture(uint imageId, int width, int height, int offsetX, int offsetY, byte[] colorData)
        {
            imageIndex = imageId;
            this.width = width;
            this.height = height;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.colorData = colorData;
        }

        /// <summary>
        /// Creates a new graphic structure based on the input.
        /// </summary>
        public SpriteTexture(uint imageId, in SpriteData spriteData, byte[] colorData)
        {
            imageIndex = imageId;
            width = spriteData.width;
            height = spriteData.height;
            offsetX = spriteData.offsetX;
            offsetY = spriteData.offsetY;
            this.colorData = colorData;
        }

        /// <summary>
        /// Copies the sprite into the target buffer with the specified bounds on the target.
        /// </summary>
        public void CopyTo(byte[] target, in SpriteBounds bounds)
            => CopyTo(this, target, in bounds, Array.Copy);

        /// <summary>
        /// Overlay another sprite over the current sprite.
        /// Returns a new sprite object with the original id but combined data.
        /// </summary>
        public SpriteTexture Overlay(SpriteTexture overlay)
        {
            var bounds = SpriteBounds.From(new[] { this, overlay });

            var buffer = new byte[bounds.width * bounds.height];
            CopyTo(this, buffer, in bounds, Array.Copy);
            CopyTo(overlay, buffer, in bounds, CopyNonZeroBytes);

            return new SpriteTexture(imageIndex, bounds.width, bounds.height, -bounds.left, -bounds.top, buffer);
        }

        /// <summary>
        /// Copies the current sprite into the target array using the specified sprite bounds and row copy method.
        /// </summary>
        private static void CopyTo(SpriteTexture source, byte[] target, in SpriteBounds bounds, Action<byte[], int, byte[], int, int> copyRow)
        {
            var targetX = bounds.left + source.offsetX;
            var targetY = bounds.bottom - (source.height + source.offsetY);
            var targetWidth = bounds.width;
            var sourceWidth = source.width;
            var sourceHeight = source.height;
            var sourceData = source.colorData;

            // Go through all rows and copy them over.
            for (var row = 0; row < sourceHeight; row++)
            {
                var sourceRowIdx = row * sourceWidth;
                var targetRowIdx = (targetY + row) * targetWidth + targetX;

                copyRow(sourceData, sourceRowIdx, target, targetRowIdx, sourceWidth);
            }
        }

        /// <summary>
        /// Apply bytes from source to destination only if the byte value is not 0.
        /// </summary>
        private static void CopyNonZeroBytes(byte[] sourceArray, int sourceIndex, byte[] destinationArray, int destinationIndex, int length)
        {
            for (var idx = 0; idx < length; idx++)
            {
                var source = sourceArray[idx + sourceIndex];

                if (source != 0)
                {
                    destinationArray[idx + destinationIndex] = source;
                }
            }
        }
    }
}
