using System;
using System.Runtime.InteropServices;

namespace Lib
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
