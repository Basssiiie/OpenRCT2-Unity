using System;
using System.Runtime.InteropServices;

namespace OpenRCT2.Unity
{
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public struct SpriteSize
    {
        public short width;
        public short height;

        public int Total
            => width * height;
    }
}
