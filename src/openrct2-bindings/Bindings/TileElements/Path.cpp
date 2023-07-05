#include "../../openrct2-bindings.h"

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

    // Returns the sprite image index for a small scenery tile element.
    //  Inspired by: path_paint()
    EXPORT uint32_t GetPathSurfaceImageIndex(const TileElement* tileElement)
    {
        const PathElement* pathElement = tileElement->AsPath();
        const FootpathSurfaceObject* footpathEntry = pathElement->GetSurfaceEntry();

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

        return (imageId + footpathEntry->BaseImageId);
    }

    // Returns the sprite image index for a small scenery tile element.
    //  Inspired by: path_paint()
    EXPORT uint32_t GetPathRailingImageIndex(const TileElement* tileElement)
    {
        return 0;
    }
}
