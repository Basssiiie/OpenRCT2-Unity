#nullable enable


namespace OpenRCT2.Generators.Map.Retro.Data
{
    /// <summary>
    /// Defines how to scale this mesh.
    /// </summary>
    public enum ObjectScaleMode
    {
        /// <summary>
        /// No scaling is applied.
        /// </summary>
        None = 0,


        /// <summary>
        /// Scales the object relatively to the tile element's height, where 1 Unity
        /// unit equals 1 height unit in RCT2.
        /// </summary>
        ObjectHeight = 1,


        /// <summary>
        /// Scales the object relatively to the pixel size of the sprite.
        /// </summary>
        SpriteSize = 2,
    }
}
