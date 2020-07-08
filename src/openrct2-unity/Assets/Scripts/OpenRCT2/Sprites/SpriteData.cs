using System.Runtime.InteropServices;

namespace Lib
{
    /// <summary>
    /// Struct with data on the sprite.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 8)]
    public struct SpriteData
    {
        // Width + height of the sprite.
        public short width;
        public short height;

        // The x and y offset that is used to draw the sprite in
        // the correct position.
        public short offsetX;
        public short offsetY;


        /// <summary>
        /// Total amount of pixels in this sprite.
        /// </summary>
        public int PixelCount
            => (width * height);
    }
}
