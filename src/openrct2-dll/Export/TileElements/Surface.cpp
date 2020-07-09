#include <openrct2/object/TerrainSurfaceObject.h>
#include <openrct2/object/TerrainEdgeObject.h>
#include <openrct2/object/ObjectManager.h>
#include <openrct2/paint/tile_element/Paint.Surface.h>
#include <openrct2/world/TileElement.h>

#include "..\Openrct2-dll.h"


extern "C"
{    
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
}
