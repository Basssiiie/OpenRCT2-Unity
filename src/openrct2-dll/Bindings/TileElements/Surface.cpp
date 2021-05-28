#include <openrct2/object/TerrainSurfaceObject.h>
#include <openrct2/object/TerrainEdgeObject.h>
#include <openrct2/object/ObjectManager.h>
#include <openrct2/paint/tile_element/Paint.Surface.h>
#include <openrct2/world/TileElement.h>

#include "..\OpenRCT2-DLL.h"


extern "C"
{    
    // Returns the sprite image index for a surface sprite.
    //  Inspired by: get_surface_object(), get_surface_image()
    EXPORT uint32_t GetSurfaceImageIndex(const TileElement* tileElement, int32_t tileX, int32_t tileY, uint8_t direction)
    {
        const SurfaceElement* surface = tileElement->AsSurface();
        uint32_t surfaceIndex = surface->GetSurfaceStyle();
        uint8_t grassLength = surface->GetGrassLength();

        uint32_t imageId = (uint32_t)SPR_NONE;

        IObjectManager& objMgr = OpenRCT2::GetContext()->GetObjectManager();
        Object* obj = objMgr.GetLoadedObject(ObjectType::TerrainSurface, surfaceIndex);

        if (obj == nullptr)
        {
            dll_log("Could not find surface object: %i", surfaceIndex);
        }
        else
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
        const SurfaceElement* surface = tileElement->AsSurface();
        uint32_t edgeIndex = surface->GetEdgeStyle();

        IObjectManager& objMgr = OpenRCT2::GetContext()->GetObjectManager();
        Object* obj = objMgr.GetLoadedObject(ObjectType::TerrainEdge, edgeIndex);

        if (obj == nullptr)
        {
            dll_log("Could not find surface edge object: %i", edgeIndex);
        }
        else
        {
            auto tobj = static_cast<TerrainEdgeObject*>(obj);
            return tobj->BaseImageId + 5; // EDGE_BOTTOMRIGHT = +5
        }
        return 0;
    }


    // Returns the sprite image for a regular water tile.
    //  Inspired by: surface_paint()
    EXPORT uint32_t GetWaterImageIndex()
    {
        // SPR_WATER_OVERLAY = overlay for water
        const int32_t imageId
            = (SPR_WATER_MASK | IMAGE_TYPE_REMAP | IMAGE_TYPE_TRANSPARENT | EnumValue(FilterPaletteID::PaletteWater) << 19);
        return imageId;
    }
}
