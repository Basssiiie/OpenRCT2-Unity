using OpenRCT2.Bindings.TileElements;

#nullable enable

namespace OpenRCT2.Generators.Map
{
    /// <summary>
    /// A base generator that creates the tile elements.
    /// </summary>
    public interface ITileElementGenerator
    {
        /// <summary>
        /// Returns the name of the generator.
        /// </summary>
        string name => GetType().Name;

        /// <summary>
        /// Override to add startup code.
        /// </summary>
        void Start(in MapData map) => Start();

        /// <summary>
        /// Override to add startup code.
        /// </summary>
        void Start()
        {
            // Optional method
        }

        /// <summary>
        /// Override to add finish code.
        /// </summary>
        void Finish(in MapData map) => Finish();

        /// <summary>
        /// Override to add finish code.
        /// </summary>
        void Finish()
        {
            // Optional method
        }

        /// <summary>
        /// Creates a tile element at the specified tile position.
        /// </summary>
        void CreateElement(in MapData map, int x, int y, int index, in TileElementInfo element);
    }
}
