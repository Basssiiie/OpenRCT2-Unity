#include <openrct2/world/TileElement.h>
#include <openrct2/world/SmallScenery.h>

#include "..\openrct2-bindings.h"


extern "C"
{
    // Returns the sprite image index for a small scenery tile element.
    //  Inspired by: fence_paint
    EXPORT uint32_t GetWallImageIndex(const TileElement* tileElement, uint8_t direction)
    {
        const WallElement* wallElement = tileElement->AsWall();
        const WallSceneryEntry* sceneryEntry = wallElement->GetEntry();

        if (sceneryEntry == nullptr)
        {
            dll_log("Wall sprite entry = null");
            return 0;
        }

        // Colours
        int32_t primaryColour = wallElement->GetPrimaryColour();
        uint32_t imageColourFlags = SPRITE_ID_PALETTE_COLOUR_1(primaryColour);
        //uint32_t dword_141F718 = imageColourFlags + 0x23800006;

        if (sceneryEntry->flags & WALL_SCENERY_HAS_SECONDARY_COLOUR)
        {
            uint8_t secondaryColour = wallElement->GetSecondaryColour();
            imageColourFlags |= secondaryColour << 24 | IMAGE_TYPE_REMAP_2_PLUS;
        }

        uint32_t tertiaryColour = 0;
        if (sceneryEntry->flags & WALL_SCENERY_HAS_TERTIARY_COLOUR)
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

        if (sceneryEntry->flags & WALL_SCENERY_HAS_GLASS)
        {
            if (sceneryEntry->flags & WALL_SCENERY_HAS_PRIMARY_COLOUR)
            {
                imageId |= imageColourFlags;
            }
        }
        else
        {
            if (sceneryEntry->flags & WALL_SCENERY_HAS_PRIMARY_COLOUR)
            {
                imageId |= imageColourFlags;
            }
        }
        return imageId;
    }
}
