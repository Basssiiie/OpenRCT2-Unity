
#nullable enable

namespace OpenRCT2.Generators.Sprites
{
    /// <summary>
    /// Factory to create flipbook graphics.
    /// </summary>
    public static class FlipbookFactory
    {
        /// <summary>
        /// Create a flipbook texture which contains all specified frames with their
        /// specified offsets aligned.
        /// </summary>
        public static SpriteTexture CreateFlipbookGraphic(SpriteTexture[] frames)
        {
            // Distance to the shared offset:
            int distanceLeft = 0;
            int distanceRight = 0;
            int distanceUpper = 0;
            int distanceLower = 0;

            int length = frames.Length;

            // Find the size of the maximum required size for each frame in the flipbook.
            for (int i = 0; i < length; i++)
            {
                SpriteTexture frame = frames[i];
                int current;

                current = -frame.OffsetX; // left
                if (distanceLeft < current) distanceLeft = current;

                current = (frame.Width + frame.OffsetX); // right
                if (distanceRight < current) distanceRight = current;

                current = -frame.OffsetY; // upper
                if (distanceUpper < current) distanceUpper = current;

                current = (frame.Height + frame.OffsetY); // lower
                if (distanceLower < current) distanceLower = current;
            }

            // Set up flipbook texture.
            int widthFrame = (distanceLeft + distanceRight);
            int widthFlipbook = (widthFrame * length);
            int heightFlipbook = (distanceUpper + distanceLower); // only 1 row
            int pixelCount = (widthFlipbook * heightFlipbook);

            byte[] outputColors = new byte[pixelCount];
            SpriteTexture output = new SpriteTexture(0, (short)widthFlipbook, (short)heightFlipbook, (short)-distanceLeft, (short)-distanceUpper, outputColors);

            for (int i = 0; i < length; i++)
            {
                // Copy all frames into the flipbook where each frame's offset is at the same spot.
                SpriteTexture frame = frames[i];
                int startX = (widthFrame * i);

                CopyColorBlock(
                    target: output,
                    x: (startX + distanceLeft + frame.OffsetX),
                    y: (distanceUpper + frame.OffsetY),
                    source: frame
                );
            }

            return output;
        }


        /// <summary>
        /// Copies the graphic from source into the bigger target graphic.
        /// </summary>
        static void CopyColorBlock(SpriteTexture target, int x, int y, SpriteTexture source)
        {
            byte[] targetData = target.ColorData;
            byte[] sourceData = source.ColorData;

            int sourceWidth = source.Width;
            int sourceHeight = source.Height;

            int targetWidth = target.Width;

            // Go through all rows.
            for (int cy = 0; cy < sourceHeight; cy++)
            {
                int sourceRowIdx = (cy * sourceWidth);
                int targetRowIdx = (y + cy) * targetWidth;

                // Copy current row.
                for (int cx = 0; cx < sourceWidth; cx++)
                {
                    int sourceIdx = (cx + sourceRowIdx);
                    int targetIdx = (x + cx + targetRowIdx);

                    targetData[targetIdx] = sourceData[sourceIdx];
                }
            }
        }
    }
}
