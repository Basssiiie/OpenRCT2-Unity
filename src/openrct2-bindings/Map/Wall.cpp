#include "../OpenRCT2.Bindings.h"
#include "../Utilities/Logging.h"
#include "../Utilities/TileElementHelper.h"

#include <openrct2/object/WallSceneryEntry.h>
#include <openrct2/world/SmallScenery.h>
#include <openrct2/world/TileElement.h>

extern "C"
{
    struct WallInfo
    {
        uint32_t imageIndex;
        uint8_t slope;
        bool animated;
        uint16_t animationFrameCount;
        uint16_t animationFrameDelay;
    };

    // Returns the sprite image index for a small scenery tile element.
    //  Inspired by: PaintWall(), PaintWallWall()
    uint32_t GetWallImageIndex(const WallElement* element, const WallSceneryEntry* entry)
    {
        // Colours
        uint8_t wallFlags = entry->flags;
        ImageId imageId;
        if (wallFlags & WALL_SCENERY_HAS_PRIMARY_COLOUR)
        {
            imageId = imageId.WithPrimary(element->GetPrimaryColour());
        }
        if (wallFlags & WALL_SCENERY_HAS_SECONDARY_COLOUR)
        {
            imageId = imageId.WithSecondary(element->GetSecondaryColour());
        }
        if (wallFlags & WALL_SCENERY_HAS_TERTIARY_COLOUR)
        {
            imageId = imageId.WithTertiary(element->GetTertiaryColour());
        }

        // Slopes
        uint8_t slope = element->GetSlope();
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
        return imageId.WithIndex(entry->image + imageOffset).ToUInt32();
    }

    // Writes the wall element details to the specified buffer.
    EXPORT void GetWallElementAt(int x, int y, int index, WallInfo* element)
    {
        const TileElement* source = GetTileElementAt(x, y, index, TileElementType::Wall);
        const WallElement* wall = source->AsWall();
        const WallSceneryEntry* entry = wall->GetEntry();

        element->imageIndex = GetWallImageIndex(wall, entry);
        element->slope = wall->GetSlope();
        element->animated = (entry->flags2 & WALL_SCENERY_2_ANIMATED);
    }
}
