using System.Runtime.InteropServices;
using UnityEngine;

namespace Lib
{
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public struct PaletteEntry
    {
        public byte blue;
        public byte green;
        public byte red;
        public byte alpha;


        const float ByteToUnityColorValue = 255f;


        public Color Color => new Color(
            red / ByteToUnityColorValue,
            green / ByteToUnityColorValue,
            blue / ByteToUnityColorValue,
            alpha / ByteToUnityColorValue
        );
    }
}
