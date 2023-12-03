#include "../OpenRCT2.Bindings.h"
#include "../Utilities/Logging.h"
#include "../Utilities/TileElementHelper.h"

#include <openrct2/object/ObjectManager.h>
#include <openrct2/object/TerrainEdgeObject.h>
#include <openrct2/object/TerrainSurfaceObject.h>
#include <openrct2/paint/tile_element/Paint.Surface.h>
#include <openrct2/world/TileElement.h>

extern "C"
{
    struct SurfaceInfo
    {
        uint32_t surfaceIndex;
        uint32_t edgeIndex;
        int32_t waterHeight;
        uint8_t slope;
    };

    // Returns the sprite image index for a surface sprite.
    //  Inspired by: GetSurfaceObject(), GetSurfaceImage()
    uint32_t GetSurfaceImageIndex(const TileElement* element, const SurfaceElement* surface, int32_t x, int32_t y)
    {
        uint32_t surfaceIndex = surface->GetSurfaceStyle();
        uint8_t grassLength = surface->GetGrassLength();
        ImageId imageId;

        IObjectManager& objMgr = OpenRCT2::GetContext()->GetObjectManager();
        Object* obj = objMgr.GetLoadedObject(ObjectType::TerrainSurface, surfaceIndex);

        if (obj == nullptr)
        {
            dll_log("Could not find surface object: %i", surfaceIndex);
        }
        else
        {
            const TerrainSurfaceObject* result = static_cast<TerrainSurfaceObject*>(obj);

            imageId = ImageId(result->GetImageId({ x, y }, grassLength, element->GetDirection(), 0, false, false));
            if (result->Colour != 255)
            {
                imageId = imageId.WithPrimary(result->Colour);
            }
        }
        return imageId.ToUInt32();
    }

    // Returns the sprite image for a surface edge sprite.
    //  Inspired by: GetEdgeImageWithOffset()
    uint32_t GetSurfaceEdgeImageIndex(const SurfaceElement* surface)
    {
        uint32_t edgeIndex = surface->GetEdgeStyle();

        IObjectManager& objMgr = OpenRCT2::GetContext()->GetObjectManager();
        Object* obj = objMgr.GetLoadedObject(ObjectType::TerrainEdge, edgeIndex);

        if (obj == nullptr)
        {
            dll_log("Could not find surface edge object: %i", edgeIndex);
            return 0;
        }

        auto tobj = static_cast<TerrainEdgeObject*>(obj);
        return tobj->BaseImageId + 5; // EDGE_BOTTOMRIGHT = +5
    }

    // Writes the surface element details to the specified buffer.
    EXPORT void GetSurfaceElementAt(int x, int y, int index, SurfaceInfo* element)
    {
        const TileElement* source = GetTileElementAt(x, y, index, TileElementType::Surface);
        const SurfaceElement* surface = source->AsSurface();

        if (surface == nullptr)
        {
            dll_log("Could not find surface element at %i, %i, index %i", x, y, index);
            return;
        }

        element->surfaceIndex = GetSurfaceImageIndex(source, surface, x, y);
        element->edgeIndex = GetSurfaceEdgeImageIndex(surface);
        element->slope = surface->GetSlope();
        element->waterHeight = surface->GetWaterHeight();
    }

    // Returns the sprite image for a regular water tile.
    //  Inspired by: PaintSurface()
    EXPORT uint32_t GetWaterImageIndex()
    {
        // SPR_WATER_OVERLAY = overlay for water
        const auto imageId = ImageId(SPR_WATER_MASK, FilterPaletteID::PaletteWater).WithBlended(true);
        return imageId.ToUInt32();
    }
}
