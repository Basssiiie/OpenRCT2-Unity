#nullable enable

namespace OpenRCT2.Bindings.Entities
{
    /// <summary>
    /// This struct contains certain statistics about a guest, like its energy
    /// and happiness.
    /// </summary>
    public readonly struct GuestStats
    {
        public readonly byte energy;
        public readonly byte happiness;
        public readonly byte nausea;
        public readonly byte hunger;
        public readonly byte thirst;
        public readonly byte toilet;
        public readonly byte minimumIntensity;
        public readonly byte maximumIntensity;
    }
}
