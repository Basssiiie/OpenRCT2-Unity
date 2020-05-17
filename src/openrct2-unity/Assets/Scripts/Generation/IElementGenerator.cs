using OpenRCT;

namespace Generation
{
    /// <summary>
    /// A generator that creates the tile elements.
    /// </summary>
    public interface IElementGenerator
    {
        /// <summary>
        /// Creates a tile element at the specified position.
        /// </summary>
        void CreateElement(int x, int y, ref TileElement tile);


        /// <summary>
        /// Initialises the generator for the specified map, before new elements
        /// are created.
        /// </summary>
        void StartGenerator(Map map);


        /// <summary>
        /// Finalizes the created elements and finishes the generator.
        /// </summary>
        void FinishGenerator();
    }
}
