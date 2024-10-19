using System.Collections.Generic;

namespace OpenRCT2.Generators.Sprites
{
    public readonly struct SpriteBounds
    {
        public readonly int left;
        public readonly int right;
        public readonly int top;
        public readonly int bottom;

        public int width => left + right;
        public int height => top + bottom;

        public SpriteBounds(int left, int right, int top, int bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }

        public static SpriteBounds From(IReadOnlyList<SpriteTexture> sprites)
        {
            int left = 0;
            int right = 0;
            int upper = 0;
            int lower = 0;

            // Find the size of the maximum required size for each frame in the flipbook.
            foreach (SpriteTexture frame in sprites)
            {
                int current;

                current = -frame.offsetX; // left
                if (left < current) left = current;

                current = (frame.width + frame.offsetX); // right
                if (right < current) right = current;

                current = -frame.offsetY; // upper
                if (upper < current) upper = current;

                current = (frame.height + frame.offsetY); // lower
                if (lower < current) lower = current;
            }

            return new SpriteBounds(left, right, upper, lower);
        }
    }
}
