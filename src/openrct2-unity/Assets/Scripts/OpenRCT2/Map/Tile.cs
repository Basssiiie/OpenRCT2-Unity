using System;

#nullable enable

namespace Lib
{
    /// <summary>
    /// A tile struct containing multiple elements.
    /// </summary>
    public readonly struct Tile : IEquatable<Tile>
    {
        /// <summary>
        /// All the elements on this tile.
        /// </summary>
        public TileElement[] Elements { get; }


        // The index at which the surface tile element is located.
        readonly int surfaceIndex;


        /// <summary>
        /// Creates a new tile from the specified buffer.
        /// </summary>
        public Tile(TileElement[] buffer, int size)
        {
            Elements = new TileElement[size];
            Array.Copy(buffer, Elements, size);

            surfaceIndex = Array.FindIndex(Elements, t => t.Type == TileElementType.Surface);
        }


        /// <summary>
        /// Returns the amount of tile elements on this tile.
        /// </summary>
        public int Count
            => Elements.Length;


        /// <summary>
        /// Returns the surface element on this tile.
        /// </summary>
        public SurfaceElement Surface
            => ((surfaceIndex != -1) ? Elements[surfaceIndex].AsSurface() : default);


        #region Equals override

        public static bool Equals(Tile left, Tile right)
            => (left.Count == right.Count)
            && (left.Elements == right.Elements);


        /// <inheritdoc/>
        public override bool Equals(object obj)
            => (obj is Tile tile && Equals(this, tile));


        /// <inheritdoc/>
        public bool Equals(Tile other)
            => (Equals(this, other));


        /// <inheritdoc/>
        public override int GetHashCode()
            => (Elements.GetHashCode());


        /// <inheritdoc/>
        public static bool operator ==(Tile left, Tile right)
            => (Equals(left, right));


        /// <inheritdoc/>
        public static bool operator !=(Tile left, Tile right)
            => (!Equals(left, right));

        #endregion
    }
}
