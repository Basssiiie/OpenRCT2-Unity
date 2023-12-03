using System.Runtime.InteropServices;

#nullable enable

namespace OpenRCT2.Bindings.Graphics
{
    /// <summary>
    /// Struct with data on the sprite.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct SpriteData
    {
        // Width + height of the sprite.
        public readonly short width;
        public readonly short height;

        // The x and y offset that is used to draw the sprite in
        // the correct position.
        public readonly short offsetX;
        public readonly short offsetY;


        /// <summary>
        /// Total amount of pixels in this sprite.
        /// </summary>
        public int PixelCount
            => (width * height);
    }
}
