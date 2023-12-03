#nullable enable


namespace OpenRCT2.Generators.Map.Retro.Data
{
    /// <summary>
    /// Specifies the strategy to use when generation this track piece element.
    /// </summary>
    public enum TrackPieceGenerationStrategy
    {
        /// <summary>
        /// Do nothing with this track piece.
        /// </summary>
        None,


        /// <summary>
        /// Places a gameobject with the specified mesh.
        /// </summary>
        Place,


        /// <summary>
        /// Places a gameobject with an extruded mesh that follows the route
        /// of the ride vehicles.
        /// </summary>
        Extrude,
    }
}
