using OpenRCT2.Bindings.Graphics;
using UnityEngine;

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
        public uint ImageIndex { get; }


        /// <summary>
        /// Width of the graphic in pixels.
        /// </summary>
        public short Width { get; }


        /// <summary>
        /// Height of the graphic in pixels.
        /// </summary>
        public short Height { get; }


        /// <summary>
        /// X offset of the graphic in pixels.
        /// </summary>
        public short OffsetX { get; }


        /// <summary>
        /// Y offset of the graphic in pixels.
        /// </summary>
        public short OffsetY { get; }


        /// <summary>
        /// Returns the total amount of pixels in this graphic.
        /// </summary>
        public int PixelCount
            => (Width * Height);


        /// <summary>
        /// The actual byte data of the graphic. Each byte represents a palette color index.
        /// </summary>
        public byte[] ColorData { get; }


        Texture2D? _texture;


        /// <summary>
        /// Creates a new graphic structure based on the input.
        /// </summary>
        public SpriteTexture(uint imageId, short width, short height, short offsetX, short offsetY, byte[] colorData)
        {
            ImageIndex = imageId;
            Width = width;
            Height = height;
            OffsetX = offsetX;
            OffsetY = offsetY;
            ColorData = colorData;
        }


        /// <summary>
        /// Creates a new graphic structure based on the input.
        /// </summary>
        public SpriteTexture(uint imageId, in SpriteData spriteData, byte[] colorData)
        {
            ImageIndex = imageId;
            Width = spriteData.width;
            Height = spriteData.height;
            OffsetX = spriteData.offsetX;
            OffsetY = spriteData.offsetY;
            ColorData = colorData;
        }


        /// <summary>
        /// Gets the texture for this graphic.
        /// </summary>
        public Texture2D GetTexture(TextureWrapMode wrapMode = TextureWrapMode.Clamp, bool makeTextureReadable = false)
        {
            if (_texture != null
                // if caller requires a readable texture, but cached is not; do not return the cached.
                && (!makeTextureReadable || _texture.isReadable)) 
            {
                return _texture;
            }

            uint imageIndex = ImageIndex;
            int pixelCount = PixelCount;
            int width = Width;
            int height = Height;

            // Convert to color and mirror sprite vertically
            Color32[] colors = new Color32[pixelCount];
            for (int outRow = 0, inRow = (pixelCount - width); outRow < pixelCount; outRow += width, inRow -= width)
            {
                for (int column = 0; column < width; ++column)
                {
                    colors[outRow + column] = SpriteFactory.PaletteToColor(ColorData[inRow + column]);
                }
            }

            // Export as Texture2D image.
            _texture = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false)
            {
                name = $"sprite{imageIndex & 0x7FFFF}(unmasked{imageIndex})",
                filterMode = FilterMode.Point,
                wrapMode = wrapMode
            };
            _texture.SetPixels32(colors);
            _texture.Apply(updateMipmaps: false, makeNoLongerReadable: !makeTextureReadable);
            return _texture;
        }
    }
}
