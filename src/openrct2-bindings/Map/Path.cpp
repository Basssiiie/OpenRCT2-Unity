#include "../OpenRCT2.Bindings.h"
#include "../Utilities/Logging.h"
#include "../Utilities/TileElementHelper.h"

#include <openrct2/object/FootpathSurfaceObject.h>
#include <openrct2/world/TileElement.h>

extern "C"
{
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

    struct PathInfo
    {
        uint32_t surfaceIndex;
        uint32_t railingIndex;
        bool sloped;
        uint8_t slopeDirection;
    };


    // Returns the sprite image index for a small scenery tile element.
    //  Inspired by: path_paint()
    uint32_t GetPathSurfaceImageIndex(const PathElement* path)
    {
        const FootpathSurfaceObject* footpathEntry = path->GetSurfaceEntry();

        uint32_t imageId;
        if (path->IsSloped())
        {
            imageId = 16; // We just take rotation 0. Always.
        }
        else
        {
            uint8_t edges = (path->GetEdgesAndCorners());
            imageId = byte_98D6E0[edges];
        }

        return (imageId + footpathEntry->BaseImageId);
    }

    // Returns the sprite image index for a small scenery tile element.
    //  Inspired by: path_paint()
    uint32_t GetPathRailingImageIndex(const TileElement* tileElement)
    {
        return 0;
    }

    // Writes the path element details to the specified buffer.
    EXPORT void GetPathElementAt(int x, int y, int index, PathInfo* element)
    {
        const TileElement* source = GetTileElementAt(x, y, index, TileElementType::Path);
        const PathElement* path = source->AsPath();

        if (path == nullptr)
        {
            dll_log("Could not find path element at %i, %i, index %i", x, y, index);
            return;
        }

        element->surfaceIndex = GetPathSurfaceImageIndex(path);
        element->sloped = path->IsSloped();
        element->slopeDirection = path->GetSlopeDirection();
    }
}
