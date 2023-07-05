#include "../../openrct2-bindings.h"

#include <openrct2/object/WallSceneryEntry.h>
#include <openrct2/world/SmallScenery.h>
#include <openrct2/world/TileElement.h>

extern "C"
{
    // Returns the sprite image index for a small scenery tile element.
    //  Inspired by: PaintWall(), PaintWallWall()
    EXPORT uint32_t GetWallImageIndex(const TileElement* tileElement, uint8_t direction)
    {
        const WallElement* wallElement = tileElement->AsWall();
        const WallSceneryEntry* wallEntry = wallElement->GetEntry();

        if (wallEntry == nullptr)
        {
            dll_log("Wall sprite entry = null");
            return 0;
        }

        // Colours
        uint8_t wallFlags = wallEntry->flags;
        ImageId imageId;
        if (wallFlags & WALL_SCENERY_HAS_PRIMARY_COLOUR)
        {
            imageId = imageId.WithPrimary(wallElement->GetPrimaryColour());
        }
        if (wallFlags & WALL_SCENERY_HAS_SECONDARY_COLOUR)
        {
            imageId = imageId.WithSecondary(wallElement->GetSecondaryColour());
        }
        if (wallFlags & WALL_SCENERY_HAS_TERTIARY_COLOUR)
        {
            imageId = imageId.WithTertiary(wallElement->GetTertiaryColour());
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

        if (wallFlags & WALL_SCENERY_HAS_GLASS)
        {
            if (wallFlags & WALL_SCENERY_IS_DOUBLE_SIDED)
            {
                imageOffset += 12;
            }
        }
        else
        {
            if (wallFlags & WALL_SCENERY_IS_DOUBLE_SIDED)
            {
                imageOffset += 6;
            }
        }
        return imageId.WithIndex(wallEntry->image + imageOffset).ToUInt32();
    }
}
