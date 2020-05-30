using System.Runtime.InteropServices;

namespace Lib
{
    [StructLayout(LayoutKind.Sequential, Size = 3)]
    public struct TrackColour
    {
        public byte main;
        public byte additional;
        public byte supports;
    }
}
