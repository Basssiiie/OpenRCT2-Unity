using System.Runtime.InteropServices;

namespace Lib
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct TrackColour
    {
        public readonly byte main;
        public readonly byte additional;
        public readonly byte supports;
    }
}
