#include <iostream>
#include <openrct2/object/ObjectManager.h>
#include <openrct2/object/SmallSceneryEntry.h>
#include <openrct2/world/TileElement.h>
#include <openrct2/world/Scenery.h>
#include <openrct2/world/SmallScenery.h>

#include "../../openrct2-bindings.h"


extern "C"
{
    // Adjusts the image index if the scenery element has colours or withering.
    uint32_t GetIndexWithColourAndWither(uint32_t imageIndex, const SmallSceneryElement* sceneryElement, const SmallSceneryEntry* entry)
    {
        // Wither flowers
        if (entry->HasFlag(SMALL_SCENERY_FLAG_CAN_WITHER))
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
        ImageId imageId = ImageId(imageIndex);
        if (entry->HasFlag(SMALL_SCENERY_FLAG_HAS_PRIMARY_COLOUR))
        {
            imageId = imageId.WithPrimary(sceneryElement->GetPrimaryColour());

            if (entry->HasFlag(SMALL_SCENERY_FLAG_HAS_SECONDARY_COLOUR))
            {
                imageId = imageId.WithSecondary(sceneryElement->GetSecondaryColour());
            }
        }
        if (entry->HasFlag(SMALL_SCENERY_FLAG_HAS_TERTIARY_COLOUR))
        {
            imageId = imageId.WithTertiary(sceneryElement->GetTertiaryColour());
        }
        return imageId.ToUInt32();
    }


    // Returns the sprite image index for a small scenery tile element.
    EXPORT uint32_t GetSmallSceneryImageIndex(const TileElement* tileElement, uint8_t direction)
    {
        const SmallSceneryElement* sceneryElement = tileElement->AsSmallScenery();
        const SmallSceneryEntry* entry = sceneryElement->GetEntry();

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
        const SmallSceneryElement* sceneryElement = tileElement->AsSmallScenery();
        const SmallSceneryEntry* entry = sceneryElement->GetEntry();

        if (entry == nullptr)
        {
            dll_log("Small scenery sprite entry = null");
            return 0;
        }

        if (!entry->HasFlag(SMALL_SCENERY_FLAG_ANIMATED))
        {
            dll_log("This small scenery entry is not animated.");
            return 0;
        }

        // Only frame offset animations have been implemented so far
        if (entry->HasFlag(SMALL_SCENERY_FLAG_FOUNTAIN_SPRAY_1))
        {
            uint16_t frame = 0;
            const uint8_t max_frames = 0xF;

            for (; frame < max_frames; frame++)
            {
                indices[frame] = (frame + entry->image + 4);
            }
            return frame;
        }
        else if (entry->HasFlag(SMALL_SCENERY_FLAG_HAS_FRAME_OFFSETS))
        {
            uint16_t frame = 0;
            uint16_t max_frames = std::min(entry->num_frames, (uint16_t)arraySize);

            for (; frame < max_frames; frame++)
            {
                int32_t image_id = entry->frame_offsets[frame];
                image_id = (image_id * 4) + direction + entry->image;

                if (entry->HasFlag(SMALL_SCENERY_FLAG_VISIBLE_WHEN_ZOOMED | SMALL_SCENERY_FLAG17))
                {
                    image_id += 4;
                }

                indices[frame] = GetIndexWithColourAndWither(image_id, sceneryElement, entry);
            }

            return frame;
        }

        dll_log("This small scenery entry is animated in a way that is not supported yet (flags = %i).", entry->flags);
        return 0;
    }


    const uint8_t IdentifierSize = sizeof(RCTObjectEntry::name);

    struct SmallSceneryEntryInfo
    {
    public:
        char identifier[IdentifierSize + 1]; // extra byte for null terminator
        uint32_t flags;
        uint16_t animationDelay;
        uint16_t animationFrameCount;
    };


    // Returns the RCT object entry for the specified small scenery.
    EXPORT void GetSmallSceneryEntry(uint32_t entryIndex, SmallSceneryEntryInfo* entry)
    {
        IObjectManager& objManager = OpenRCT2::GetContext()->GetObjectManager();
        Object* obj = objManager.GetLoadedObject(ObjectType::SmallScenery, entryIndex);

        if (obj == nullptr)
        {
            dll_log("Small scenery object entry = null");
            return;
        }

        SmallSceneryEntry* sceneryEntry = static_cast<SmallSceneryEntry*>(obj->GetLegacyData());

        entry->flags = sceneryEntry->flags;
        entry->animationDelay = sceneryEntry->animation_delay;
        entry->animationFrameCount = sceneryEntry->num_frames;

        const RCTObjectEntry objectEntry = obj->GetObjectEntry();
        std::memcpy(entry->identifier, objectEntry.name, IdentifierSize);
    }
}
