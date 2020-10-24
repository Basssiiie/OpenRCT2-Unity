using System.Runtime.InteropServices;
using UnityEngine;

namespace Lib
{
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public readonly struct PaletteEntry
    {
        public readonly byte blue;
        public readonly byte green;
        public readonly byte red;
        public readonly byte alpha;


        // Converts the 0-255 color byte to a float between 0 and 1.
        const float ByteToUnityColorValue = 255f;


        /// <summary>
        /// Gets the palette entry as a Unity color.
        /// </summary>
        public Color Color => new Color(
            red / ByteToUnityColorValue,
            green / ByteToUnityColorValue,
            blue / ByteToUnityColorValue,
            alpha / ByteToUnityColorValue
        );
    }
}
