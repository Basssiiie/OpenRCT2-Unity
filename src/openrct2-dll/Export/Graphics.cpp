#include <openrct2/drawing/Drawing.h>
#include <openrct2/interface/Colour.h>
#include <openrct2/object/TerrainSurfaceObject.h>
#include <openrct2/object/TerrainEdgeObject.h>
#include <openrct2/object/ObjectManager.h>
#include <openrct2/paint/tile_element/Paint.Surface.h>
#include <openrct2/sprites.h>
#include <openrct2/world/TileElement.h>
#include <openrct2/world/SmallScenery.h>

#include "Openrct2-dll.h"


extern "C"
{
    // Gets the full colour palette currently in use.
    EXPORT void GetPalette(rct_palette_entry* entries)
    {
        for (int i = 0; i < 256; i++)
        {
            entries[i] = gPalette[i];
            entries[i].alpha = (i == 0) ? 0 : 255;
        }
    }


    // Converts any of the colour ids (0-31) to the corresponding palette index (0-255).
    EXPORT uint8_t GetPaletteIndexForColourId(uint8_t colourId)
    {
        return ColourMapA[colourId].mid_light;
    }


    // Returns the sprite image index for a surface sprite.
    //  Inspired by: get_surface_object(), get_surface_image()
    EXPORT uint32_t GetSurfaceImageIndex(const TileElement* tileElement, int32_t tileX, int32_t tileY, uint8_t direction)
    {
        SurfaceElement* surface = tileElement->AsSurface();
        auto surfaceIndex = surface->GetSurfaceStyle();
        auto grassLength = surface->GetGrassLength();

        auto imageId = (uint32_t)SPR_NONE;

        auto& objMgr = OpenRCT2::GetContext()->GetObjectManager();
        auto obj = objMgr.GetLoadedObject(OBJECT_TYPE_TERRAIN_SURFACE, surfaceIndex);

        if (obj != nullptr)
        {
            TerrainSurfaceObject* result = static_cast<TerrainSurfaceObject*>(obj);

            imageId = result->GetImageId({ tileX, tileY }, grassLength, direction, 0, false, false);
            if (result->Colour != 255)
            {
                imageId |= SPRITE_ID_PALETTE_COLOUR_1(result->Colour);
            }
        }
        return imageId;
    }


    // Returns the sprite image for a surface edge sprite.
    //  Inspired by: get_edge_image_with_offset()
    EXPORT uint32_t GetSurfaceEdgeImageIndex(const TileElement* tileElement)
    {
        SurfaceElement* surface = tileElement->AsSurface();
        auto edgeIndex = surface->GetEdgeStyle();

        auto& objMgr = OpenRCT2::GetContext()->GetObjectManager();
        auto obj = objMgr.GetLoadedObject(OBJECT_TYPE_TERRAIN_EDGE, edgeIndex);
        if (obj != nullptr)
        {
            auto tobj = static_cast<TerrainEdgeObject*>(obj);
            return tobj->BaseImageId;
        }
        return 0;
    }


    // Returns the sprite image for a regular water tile.
    //  Inspired by: surface_paint()
    EXPORT uint32_t GetWaterImageIndex()
    {
        // SPR_WATER_OVERLAY = overlay for water
        const int32_t imageId = (SPR_WATER_MASK | IMAGE_TYPE_REMAP | IMAGE_TYPE_TRANSPARENT | PALETTE_WATER << 19);
        return imageId;
    }


    // Flat path table, from Paint.Path.cpp
    static constexpr const uint8_t byte_98D6E0[] = {
        0, 1, 2, 3, 4, 5, 6,  7,  8, 9,  10, 11, 12, 13, 14, 15, 0, 1, 2, 20, 4, 5, 6, 22, 8, 9, 10, 26, 12, 13, 14, 36,
        0, 1, 2, 3, 4, 5, 21, 23, 8, 9,  10, 11, 12, 13, 33, 37, 0, 1, 2, 3,  4, 5, 6, 24, 8, 9, 10, 11, 12, 13, 14, 38,
        0, 1, 2, 3, 4, 5, 6,  7,  8, 9,  10, 11, 29, 30, 34, 39, 0, 1, 2, 3,  4, 5, 6, 7,  8, 9, 10, 11, 12, 13, 14, 40,
        0, 1, 2, 3, 4, 5, 6,  7,  8, 9,  10, 11, 12, 13, 35, 41, 0, 1, 2, 3,  4, 5, 6, 7,  8, 9, 10, 11, 12, 13, 14, 42,
        0, 1, 2, 3, 4, 5, 6,  7,  8, 25, 10, 27, 12, 31, 14, 43, 0, 1, 2, 3,  4, 5, 6, 7,  8, 9, 10, 28, 12, 13, 14, 44,
        0, 1, 2, 3, 4, 5, 6,  7,  8, 9,  10, 11, 12, 13, 14, 45, 0, 1, 2, 3,  4, 5, 6, 7,  8, 9, 10, 11, 12, 13, 14, 46,
        0, 1, 2, 3, 4, 5, 6,  7,  8, 9,  10, 11, 12, 32, 14, 47, 0, 1, 2, 3,  4, 5, 6, 7,  8, 9, 10, 11, 12, 13, 14, 48,
        0, 1, 2, 3, 4, 5, 6,  7,  8, 9,  10, 11, 12, 13, 14, 49, 0, 1, 2, 3,  4, 5, 6, 7,  8, 9, 10, 11, 12, 13, 14, 50
    };


    // Returns the sprite image index for a small scenery tile element.
    //  Inspired by: path_paint()
    EXPORT uint32_t GetPathSurfaceImageIndex(const TileElement* tileElement)
    {
        PathElement* pathElement = tileElement->AsPath();
        PathSurfaceEntry* footpathEntry = pathElement->GetSurfaceEntry();

        uint32_t imageId;
        if (tileElement->AsPath()->IsSloped())
        {
            imageId = 16; // We just take rotation 0. Always.
        }
        else
        {
            uint8_t edges = (pathElement->GetEdgesAndCorners());
            imageId = byte_98D6E0[edges];
        }

        return (imageId + footpathEntry->image);
    }


    // Returns the sprite image index for a small scenery tile element.
    //  Inspired by: path_paint()
    EXPORT uint32_t GetPathRailingImageIndex(const TileElement* tileElement)
    {
        return 0;
    }


    // Returns the sprite image index for a small scenery tile element.
    EXPORT uint32_t GetSmallSceneryImageIndex(const TileElement* tileElement, uint8_t direction)
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
                imageIndex |= SPRITE_ID_PALETTE_COLOUR_2(
                    sceneryElement->GetPrimaryColour(), sceneryElement->GetSecondaryColour());
            }
            else
            {
                imageIndex |= SPRITE_ID_PALETTE_COLOUR_1(sceneryElement->GetPrimaryColour());
            }
        }
        return imageIndex;
    }

    
    // Returns the sprite image index for a small scenery tile element.
    //  Inspired by: fence_paint
    EXPORT uint32_t GetWallImageIndex(const TileElement* tileElement, uint8_t direction)
    {
        WallElement* wallElement = tileElement->AsWall();
        rct_scenery_entry* sceneryEntry = wallElement->GetEntry();

        if (sceneryEntry == nullptr)
        {
            printf("(me) Wall sprite entry = null\n");
            return 0;
        }

        // Colours
        int32_t primaryColour = wallElement->GetPrimaryColour();
        uint32_t imageColourFlags = SPRITE_ID_PALETTE_COLOUR_1(primaryColour);
        //uint32_t dword_141F718 = imageColourFlags + 0x23800006;

        if (sceneryEntry->wall.flags & WALL_SCENERY_HAS_SECONDARY_COLOUR)
        {
            uint8_t secondaryColour = wallElement->GetSecondaryColour();
            imageColourFlags |= secondaryColour << 24 | IMAGE_TYPE_REMAP_2_PLUS;
        }

        uint32_t tertiaryColour = 0;
        if (sceneryEntry->wall.flags & WALL_SCENERY_HAS_TERNARY_COLOUR)
        {
            tertiaryColour = wallElement->GetTertiaryColour();
            imageColourFlags &= 0x0DFFFFFFF;
        }

        // Slopes
        uint8_t slope = wallElement->GetSlope();
        uint32_t imageOffset;

        if (slope == 2)
        {
            imageOffset = 2;
        }
        else if (slope == 1)
        {
            imageOffset = 4;
        }
        else
        {
            imageOffset = 0;
        }

        uint32_t imageId = sceneryEntry->image + imageOffset;

        if (sceneryEntry->wall.flags & WALL_SCENERY_HAS_GLASS)
        {
            if (sceneryEntry->wall.flags & WALL_SCENERY_HAS_PRIMARY_COLOUR)
            {
                imageId |= imageColourFlags;
            }
        }
        else
        {
            if (sceneryEntry->wall.flags & WALL_SCENERY_HAS_PRIMARY_COLOUR)
            {
                imageId |= imageColourFlags;
            }
        }
        return imageId;
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
            printf("(me) Could not find g1 element for %i.\n", imageIndex);
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

        gfx_draw_sprite_software(&dpi, ImageId::FromUInt32(imageIndex), 0, 0);

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
