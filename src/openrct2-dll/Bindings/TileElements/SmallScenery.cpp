#include <openrct2/world/TileElement.h>
#include <openrct2/world/SmallScenery.h>

#include "..\OpenRCT2-DLL.h"


extern "C"
{
    // Adjusts the image index if the scenery element has colours or withering.
    uint32_t GetIndexWithColourAndWither(uint32_t imageIndex, SmallSceneryElement* sceneryElement, rct_scenery_entry* entry)
    {
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

        // Scenery colours
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


    // Returns the sprite image index for a small scenery tile element.
    EXPORT uint32_t GetSmallSceneryImageIndex(const TileElement* tileElement, uint8_t direction)
    {
        SmallSceneryElement* sceneryElement = tileElement->AsSmallScenery();
        rct_scenery_entry* entry = sceneryElement->GetEntry();

        if (entry == nullptr)
        {
            dll_log("Small scenery sprite entry = null");
            return 0;
        }

        uint32_t imageIndex = entry->image + direction;

        return GetIndexWithColourAndWither(imageIndex, sceneryElement, entry);
    }


    // Get all indices of the animation of this small scenery element, returns the amount of animation frames.
    EXPORT int32_t GetSmallSceneryAnimationIndices(const TileElement* tileElement, uint8_t direction, uint32_t* indices, int32_t arraySize)
    {
        SmallSceneryElement* sceneryElement = tileElement->AsSmallScenery();
        rct_scenery_entry* entry = sceneryElement->GetEntry();

        if (entry == nullptr)
        {
            dll_log("Small scenery sprite entry = null");
            return 0;
        }

        if (!scenery_small_entry_has_flag(entry, SMALL_SCENERY_FLAG_ANIMATED))
        {
            dll_log("This small scenery entry is not animated.");
            return 0;
        }

        // Only frame offset animations have been implemented so far
        if (scenery_small_entry_has_flag(entry, SMALL_SCENERY_FLAG_FOUNTAIN_SPRAY_1))
        {
            uint16_t frame = 0;
            const uint8_t max_frames = 0xF;

            for (; frame < max_frames; frame++)
            {
                indices[frame] = (frame + entry->image + 4);
            }
            return frame;
        }
        else if (scenery_small_entry_has_flag(entry, SMALL_SCENERY_FLAG_HAS_FRAME_OFFSETS))
        {
            uint16_t frame = 0;
            uint16_t max_frames = std::min(entry->small_scenery.num_frames, (uint16_t)arraySize);

            for (; frame < max_frames; frame++)
            {
                // 6E0222:
                uint16_t delay = entry->small_scenery.animation_delay & 0xFF;
                frame >>= delay;
                frame &= entry->small_scenery.animation_mask;

                int32_t image_id = entry->small_scenery.frame_offsets[frame];
                image_id = (image_id * 4) + direction + entry->image;

                if (scenery_small_entry_has_flag(entry, SMALL_SCENERY_FLAG_VISIBLE_WHEN_ZOOMED | SMALL_SCENERY_FLAG17))
                {
                    image_id += 4;
                }

                indices[frame] = GetIndexWithColourAndWither(image_id, sceneryElement, entry);
            }

            return frame;
        }

        dll_log("This small scenery entry is animated in a way that is not supported yet (flags = %i).", entry->small_scenery.flags);
        return 0;
    }
}
