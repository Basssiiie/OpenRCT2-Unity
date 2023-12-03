using UnityEngine;

#nullable enable

namespace OpenRCT2.Bindings.Graphics
{
    public readonly struct PaletteEntry
    {
        public readonly byte blue;
        public readonly byte green;
        public readonly byte red;
        public readonly byte alpha;


        /// <summary>
        /// Gets the palette entry as a Unity Color32. (more efficient than Color)
        /// </summary>
        public Color32 ToColor32() => new Color32(red, green, blue, alpha);


        /// <summary>
        /// Gets the palette entry as a Unity Color. (less efficient than Color32)
        /// </summary>
        public Color ToColor() => ToColor32(); // use the implicit cast
    }
}
