#include <openrct2/sprites.h>
#include <openrct2/world/SmallScenery.h>

#include "OpenRCT2-DLL.h"


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
        const rct_g1_element* g1 = gfx_get_g1_element(imageIndex & 0x7FFFF);

        if (g1 == nullptr)
        {
            dll_log("Could not find g1 element for %i.", imageIndex);
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
        const rct_g1_element* g1 = gfx_get_g1_element(maskedImageId);

        if (g1 == nullptr)
        {
            dll_log("Could not find g1 element for %i.", maskedImageId);
            return;
        }

        uint16_t width = g1->width;
        uint16_t height = g1->height;
        size_t numPixels = (size_t)width * height;

        auto bits = new uint8_t[numPixels];
        std::fill_n(bits, numPixels, 0);

        rct_drawpixelinfo dpi;
        dpi.bits = bits;
        dpi.x = g1->x_offset;
        dpi.y = g1->y_offset;
        dpi.width = width;
        dpi.height = height;
        dpi.pitch = 0;
        dpi.zoom_level = 0;

        gfx_draw_sprite_software(&dpi, ImageId::FromUInt32(imageIndex), { 0, 0 });

        for (int i = 0; i < numPixels; i++)
        {
            pixels[i] = dpi.bits[i];
        }

        delete[] dpi.bits;
    }


    // Returns the RCT Scenery entry for the specified path, smallscenery, wall, largescenery or banner.
    EXPORT void GetSceneryEntry(uint8_t type, uint32_t entryIndex, rct_scenery_entry* entry)
    {
        switch (type)
        {
            case TILE_ELEMENT_TYPE_PATH:
                break;

            case TILE_ELEMENT_TYPE_SMALL_SCENERY:
                *entry = *get_small_scenery_entry(entryIndex);
                break;

            case TILE_ELEMENT_TYPE_WALL:
                *entry = *get_wall_entry(entryIndex);
                break;

            case TILE_ELEMENT_TYPE_LARGE_SCENERY:
                break;

            case TILE_ELEMENT_TYPE_BANNER:
                break;
        }
    }
}
