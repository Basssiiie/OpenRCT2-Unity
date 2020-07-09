#include <openrct2/world/TileElement.h>
#include <openrct2/world/SmallScenery.h>

#include "..\Openrct2-dll.h"


extern "C"
{    
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
}
