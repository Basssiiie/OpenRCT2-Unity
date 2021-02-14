using System.Runtime.InteropServices;

#nullable enable

namespace Lib
{
    /// <summary>
    /// This struct contains certain statistics about a peep, like its energy
    /// and happiness.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct PeepStats
    {
        public readonly byte energy;
        public readonly byte happiness;
        public readonly byte nausea;
        public readonly byte hunger;
        public readonly byte thirst;
        public readonly byte toilet;
        public readonly byte intensity; // First 4 bits = max intensity, second 4 bits = min intensity.


        /// <summary>
        /// Minimum intensity level for this peep.
        /// </summary>
        public byte MinIntensity => (byte)(intensity & 0b1111);


        /// <summary>
        /// Maximum intensity level for this peep.
        /// </summary>
        public byte MaxIntensity => (byte)(intensity >> 4);
    }
}
