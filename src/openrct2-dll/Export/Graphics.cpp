#include <openrct2/drawing/Drawing.h>
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
    EXPORT void GetPalette(rct_palette_entry* entries)
    {
        for (int i = 0; i < 256; i++)
        {
            entries[i] = gPalette[i];
        }
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


    // Returns the image index of the tile element and its texture size.
    EXPORT void GetTextureSize(uint32_t imageIndex, rct_size16* textureSize)
    {
        /*
        uint32_t imageIndex = 0;
        switch (tileElement->GetType())
        {
            case TILE_ELEMENT_TYPE_SURFACE:
                imageIndex = GetSurfaceSprite(tileElement, 0, 0, direction);
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
        }*/

        *textureSize = gfx_get_sprite_size(imageIndex);
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
