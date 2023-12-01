#include "../OpenRCT2.Bindings.h"
#include "../Utilities/Logging.h"

#include <openrct2/drawing/Drawing.h>


extern "C"
{
    // Gets the full colour palette currently in use.
    EXPORT void GetPalette(PaletteBGRA* entries)
    {
        for (int i = 0; i < 256; i++)
        {
            entries[i] = gPalette.Colour[i];
            entries[i].Alpha = (i == 0) ? 0 : 255;
        }
    }

    // Converts any of the colour ids (0-31) to the corresponding palette index (0-255).
    EXPORT uint8_t GetPaletteIndexForColourId(uint8_t colourId)
    {
        return ColourMapA[colourId].mid_light;
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
        data->x_offset = g1->x_offset;
        data->y_offset = g1->y_offset;
    }

    // Returns the actual texture data based on the image index.
    EXPORT void GetTexturePixels(uint32_t imageIndex, uint8_t* pixels, int arraySize)
    {
        const int32_t maskedImageId = imageIndex & 0x7FFFF;
        const G1Element* g1 = GfxGetG1Element(maskedImageId);

        if (g1 == nullptr)
        {
            dll_log("Could not find g1 element pixels for %i. (Unmasked: %i)", maskedImageId, imageIndex);
            return;
        }

        uint16_t width = g1->width;
        uint16_t height = g1->height;
        size_t numPixels = (size_t)width * height;

        auto bits = new uint8_t[numPixels];
        std::fill_n(bits, numPixels, 0);

        DrawPixelInfo dpi;
        dpi.bits = bits;
        dpi.x = g1->x_offset;
        dpi.y = g1->y_offset;
        dpi.width = width;
        dpi.height = height;
        dpi.pitch = 0;
        dpi.zoom_level = ZoomLevel(0);

        GfxDrawSpriteSoftware(dpi, ImageId::FromUInt32(imageIndex), { 0, 0 });

        for (int i = 0; i < numPixels; i++)
        {
            pixels[i] = dpi.bits[i];
        }

        delete[] dpi.bits;
    }
}
