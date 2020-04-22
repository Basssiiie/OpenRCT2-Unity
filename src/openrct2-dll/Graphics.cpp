#include <openrct2/drawing/Drawing.h>
#include <openrct2/world/TileElement.h>
#include <openrct2/world/SmallScenery.h>

#include "Openrct2-dll.h"


// Returns the sprite image index for a small scenery tile element.
uint32_t GetSmallScenerySprite(const TileElement* tileElement, uint8_t direction)
{
    SmallSceneryElement* sceneryElement = tileElement->AsSmallScenery();
    rct_scenery_entry* entry = sceneryElement->GetEntry();

    if (entry == nullptr)
    {
        printf("(me) Small scenery sprite entry = null\n");
        return 0;
    }

    uint32_t imageIndex = entry->image + direction;

    // Wither flowers
    if (scenery_small_entry_has_flag(entry, SMALL_SCENERY_FLAG_CAN_WITHER))
    {
        uint8_t age = sceneryElement->GetAge();

        if (age >= SCENERY_WITHER_AGE_THRESHOLD_1)
        {
            imageIndex += 4;
        }
        if (age >= SCENERY_WITHER_AGE_THRESHOLD_2)
        {
            imageIndex += 4;
        }
    }

    // Scenery colors
    if (scenery_small_entry_has_flag(entry, SMALL_SCENERY_FLAG_HAS_PRIMARY_COLOUR))
    {
        if (scenery_small_entry_has_flag(entry, SMALL_SCENERY_FLAG_HAS_SECONDARY_COLOUR))
        {
            imageIndex |= SPRITE_ID_PALETTE_COLOUR_2(sceneryElement->GetPrimaryColour(), sceneryElement->GetSecondaryColour());
        }
        else
        {
            imageIndex |= SPRITE_ID_PALETTE_COLOUR_1(sceneryElement->GetPrimaryColour());
        }
    }

    return imageIndex;
}


extern "C"
{
    EXPORT void GetPalette(rct_palette_entry* entries)
    {
        for (int i = 0; i < 256; i++)
        {
            entries[i] = gPalette[i];
        }
    }


    // Returns the image index of the tile element and its texture size.
    EXPORT uint32_t GetTileElementTextureInfo(const TileElement* tileElement, uint8_t direction, rct_size16* textureSize)
    {
        uint32_t imageIndex = 0;
        switch (tileElement->GetType())
        {
            case TILE_ELEMENT_TYPE_SURFACE:
                // surface_paint(session, direction, baseZ, tile_element);
                break;
            case TILE_ELEMENT_TYPE_PATH:
                // path_paint(session, baseZ, tile_element);
                break;
            case TILE_ELEMENT_TYPE_TRACK:
                // track_paint(session, direction, baseZ, tile_element);
                break;
            case TILE_ELEMENT_TYPE_SMALL_SCENERY:
                imageIndex = GetSmallScenerySprite(tileElement, direction);
                break;
            case TILE_ELEMENT_TYPE_ENTRANCE:
                // entrance_paint(session, direction, baseZ, tile_element);
                break;
            case TILE_ELEMENT_TYPE_WALL:
                // fence_paint(session, direction, baseZ, tile_element);
                break;
            case TILE_ELEMENT_TYPE_LARGE_SCENERY:
                // large_scenery_paint(session, direction, baseZ, tile_element);
                break;
            case TILE_ELEMENT_TYPE_BANNER:
                // banner_paint(session, direction, baseZ, tile_element);
                break;
        }

        if (imageIndex == 0)
        {
            printf("(me) GetTileElementTextureInfo: image index = 0\n");
            return 0;
        }

        *textureSize = gfx_get_sprite_size(imageIndex);
        return imageIndex;
    }


    // Returns the actual texture data based on the image index.
    EXPORT void GetTexture(uint32_t imageIndex, uint8_t* pixels, int arraySize)
    {
        const int32_t maskedImageId = imageIndex & 0x7FFFF;
        const rct_g1_element* g1 = gfx_get_g1_element(maskedImageId);

        if (g1 == nullptr)
        {
            printf("(me) Could not find g1 element for %i.\n", maskedImageId);
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

        printf("(me) g1 = %i, %i, %i x %i\n", g1->x_offset, g1->y_offset, width, height);
        gfx_draw_sprite_software(&dpi, ImageId::FromUInt32(imageIndex), 0, 0);

        for (int i = 0; i < numPixels; i++)
        {
            pixels[i] = dpi.bits[i];
        }

        delete[] dpi.bits;
    }
}
