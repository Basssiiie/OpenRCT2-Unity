#include "../OpenRCT2.Bindings.h"
#include "../Utilities/Logging.h"

#include <algorithm>
#include <cstdint>
#include <openrct2/drawing/Colour.h>
#include <openrct2/drawing/ColourMap.h>
#include <openrct2/drawing/ColourPalette.h>
#include <openrct2/drawing/Drawing.h>
#include <openrct2/drawing/Drawing.Sprite.h>
#include <openrct2/drawing/G1Element.h>
#include <openrct2/drawing/ImageId.hpp>
#include <openrct2/drawing/PaletteIndex.h>
#include <openrct2/drawing/RenderTarget.h>
#include <openrct2/interface/ZoomLevel.h>
#include <string.h>

using namespace OpenRCT2::Drawing;

extern "C"
{
    // Gets the full colour palette currently in use.
    EXPORT void GetPalette(BGRAColour* entries)
    {
        for (int i = 0; i < 256; i++)
        {
            entries[i] = gPalette[i];
            entries[i].alpha = (i == 0) ? 0 : 255;
        }
    }

    // Converts any of the colour ids (0-31) to the corresponding palette index (0-255).
    EXPORT PaletteIndex GetPaletteIndexForColourId(Colour colourId)
    {
        return getColourMap(colourId).midLight;
    }

    // Struct containing information about the sprite.
    struct sprite_data
    {
        int16_t width;
        int16_t height;
        int16_t x_offset;
        int16_t y_offset;
    };

    // Returns the image index of the tile element and its texture size.
    EXPORT void GetTextureData(uint32_t imageIndex, sprite_data* data)
    {
        const int32_t maskedImageId = imageIndex & 0x7FFFF;
        const G1Element* g1 = GfxGetG1Element(maskedImageId);

        if (g1 == nullptr)
        {
            dll_log("Could not find g1 element data for %i. (Unmasked: %i)", maskedImageId, imageIndex);
            return;
        }

        data->width = g1->width;
        data->height = g1->height;
        data->x_offset = g1->xOffset;
        data->y_offset = g1->yOffset;
    }

    // Returns the actual texture data based on the image index.
    EXPORT void GetTexturePixels(
        uint32_t imageIndex, Colour colour1, Colour colour2, Colour colour3, PaletteIndex* pixels, int length)
    {
        auto imageId = ImageId(imageIndex & 0x7FFFF);
        if (colour1 != kColourNull)
        {
            imageId = imageId.WithPrimary(colour1);
        }
        if (colour2 != kColourNull)
        {
            imageId = imageId.WithSecondary(colour2);
        }
        if (colour3 != kColourNull)
        {
            imageId = imageId.WithTertiary(colour3);
        }
        const G1Element* g1 = GfxGetG1Element(imageId);

        if (g1 == nullptr)
        {
            dll_log("Could not find g1 element pixels for %i.", imageIndex);
            return;
        }

        uint16_t width = g1->width;
        uint16_t height = g1->height;
        size_t numPixels = (size_t)width * height;

        auto bits = new PaletteIndex[numPixels];
        std::fill_n(bits, numPixels, PaletteIndex::transparent);

        RenderTarget dpi;
        dpi.bits = bits;
        dpi.x = g1->xOffset;
        dpi.y = g1->yOffset;
        dpi.width = width;
        dpi.height = height;
        dpi.pitch = 0;
        dpi.zoom_level = ZoomLevel(0);

        GfxDrawSpriteSoftware(dpi, imageId, { 0, 0 });

        // Flip the render up-side-down
        int lastRow = (height - 1);
        for (int y = 0; y <= lastRow; y++)
        {
            memcpy(pixels + (lastRow - y) * width, dpi.bits + y * width, width);
        }

        delete[] dpi.bits;
    }
}
