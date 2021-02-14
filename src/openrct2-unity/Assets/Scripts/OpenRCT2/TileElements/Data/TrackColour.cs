using System.Runtime.InteropServices;

#nullable enable

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
