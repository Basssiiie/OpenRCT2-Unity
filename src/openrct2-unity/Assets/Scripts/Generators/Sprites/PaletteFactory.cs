using OpenRCT2.Bindings.Graphics;
using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Sprites
{
    /// <summary>
    /// Factory for managing the game's palette.
    /// </summary>
    public static class PaletteFactory
    {
        static PaletteEntry[]? _palette = null;
        static Texture2D? _texture = null;

        /// <summary>
        /// Returns a Unity color for the specified RCT2 color index on the palette.
        /// </summary>
        public static Color32 IndexToColor(ushort colorIndex)
        {
            Assert.IsNotNull(_palette);

            return _palette[colorIndex].ToColor32();
        }

        /// <summary>
        /// Reloads the game's palette, for example after a new park has been loaded.
        /// </summary>
        public static Texture2D GetPalette()
        {
            Assert.IsNotNull(_texture);

            return _texture;
        }

        /// <summary>
        /// Reloads the game's palette, for example after a new park has been loaded.
        /// </summary>
        public static Texture2D ReloadPalette()
        {
            _palette = GraphicsDataFactory.GetPalette();
            return _texture = CreatePalette();
        }

        /// <summary>
        /// Reloads the game's palette, for example after a new park has been loaded.
        /// </summary>
        public static Texture2D CreatePalette()
        {
            Assert.IsNotNull(_palette);

            var size = _palette.Length;
            var colours = new Color32[size];

            for (ushort i = 0; i < size; i++)
            {
                colours[i] = IndexToColor(i);
            }

            // Export as Texture2D image.
            var texture = new Texture2D(size, 1, TextureFormat.RGBA32, mipChain: false, linear: true, createUninitialized: true)
            {
                name = "Palette",
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            texture.SetPixels32(colours);
            texture.Apply(updateMipmaps: false, makeNoLongerReadable: true);
            return texture;
        }
    }
}
