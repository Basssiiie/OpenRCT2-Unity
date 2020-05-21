using System;

namespace Lib
{
    /// <summary>
    /// A small struct that contains RCT graphic information.
    /// </summary>
    public readonly struct Graphic : IEquatable<Graphic>
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
        /// The actual byte data of the graphic. Each byte represents a palette color index.
        /// </summary>
        public byte[] Data { get; }


        /// <summary>
        /// Returns the total amount of pixels in this graphic.
        /// </summary>
        public int PixelCount
            => (Width * Height);


        /// <summary>
        /// Creates a new graphic structure based on the input.
        /// </summary>
        public Graphic(uint imageId, short width, short height, byte[] data)
        {
            ImageIndex = imageId;
            Width = width;
            Height = height;
            Data = data;
        }


        #region IEquatable implementation

        /// <summary>
        /// Graphics are considered equal by their internal image id, regardless
        /// of the other data it contains.
        /// </summary>
        public static bool Equals(Graphic left, Graphic right)
            => (left.ImageIndex == right.ImageIndex);


        /// <inheritdoc/>
        public override bool Equals(object obj)
            => (obj is Graphic graphic && Equals(this, graphic));


        /// <inheritdoc/>
        public bool Equals(Graphic other)
            => (Equals(this, other));


        /// <inheritdoc/>
        public override int GetHashCode()
            => (ImageIndex.GetHashCode());


        /// <inheritdoc/>
        public static bool operator ==(Graphic left, Graphic right)
            => (Equals(left, right));


        /// <inheritdoc/>
        public static bool operator !=(Graphic left, Graphic right)
            => (!Equals(left, right));

        #endregion
    }
}
