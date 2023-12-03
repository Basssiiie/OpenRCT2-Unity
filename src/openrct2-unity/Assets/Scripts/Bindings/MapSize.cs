using System.Runtime.InteropServices;

#nullable enable

namespace OpenRCT2.Bindings
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct MapSize
    {
        public readonly int width;
        public readonly int height;
    };
}
