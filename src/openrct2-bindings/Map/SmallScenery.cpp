#include "../OpenRCT2.Bindings.h"
#include "../Utilities/Logging.h"
#include "../Utilities/TileElementHelper.h"

#include <iostream>
#include <openrct2/object/ObjectManager.h>
#include <openrct2/object/ObjectTypes.h>
#include <openrct2/object/SmallSceneryEntry.h>
#include <openrct2/world/Map.h>
#include <openrct2/world/Scenery.h>
#include <openrct2/world/SmallScenery.h>
#include <openrct2/world/TileElement.h>

extern "C"
{
    const uint8_t IdentifierSize = 50;

    struct SmallSceneryInfo
    {
        uint32_t imageIndex;
        ObjectEntryIndex objectIndex;
        uint8_t quadrant;
        bool fullTile;
        uint16_t animationFrameCount;
        uint16_t animationFrameDelay;
        bool animated;
        char identifier[IdentifierSize];
    };

    // Adjusts the image index if the scenery element has colours or withering.
    uint32_t GetIndexWithColourAndWither(const SmallSceneryElement* element, const SmallSceneryEntry* entry)
    {
        uint32_t imageIndex = entry->image + element->GetDirection();

        // Wither flowers
        if (entry->HasFlag(SMALL_SCENERY_FLAG_CAN_WITHER))
        {
            uint8_t age = element->GetAge();

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
            imageId = imageId.WithPrimary(element->GetPrimaryColour());

            if (entry->HasFlag(SMALL_SCENERY_FLAG_HAS_SECONDARY_COLOUR))
            {
                imageId = imageId.WithSecondary(element->GetSecondaryColour());
            }
        }
        if (entry->HasFlag(SMALL_SCENERY_FLAG_HAS_TERTIARY_COLOUR))
        {
            imageId = imageId.WithTertiary(element->GetTertiaryColour());
        }
        return imageId.ToUInt32();
    }

    // Writes the small scenery element details to the specified buffer.
    EXPORT void GetSmallSceneryElementAt(int x, int y, int index, SmallSceneryInfo* element)
    {
        const TileElement* source = GetTileElementAt(x, y, index, TileElementType::SmallScenery);
        const SmallSceneryElement* scenery = source->AsSmallScenery();
        const SmallSceneryEntry* entry = scenery->GetEntry();

        element->objectIndex = scenery->GetEntryIndex();
        element->imageIndex = GetIndexWithColourAndWither(scenery, entry);
        element->quadrant = scenery->GetSceneryQuadrant();
        element->fullTile = entry->HasFlag(SMALL_SCENERY_FLAG_FULL_TILE);
        element->animated = entry->HasFlag(SMALL_SCENERY_FLAG_ANIMATED);
        element->animationFrameCount = (entry->num_frames != 0) ? entry->num_frames : 0xF;
        element->animationFrameDelay = entry->animation_delay;

        const Object* object = ObjectEntryGetObject(ObjectType::SmallScenery, scenery->GetEntryIndex());
        object->GetIdentifier().copy(element->identifier, IdentifierSize);
    }

    // Get all indices of the animation of this small scenery element, returns the amount of animation frames.
    EXPORT int32_t GetSmallSceneryAnimationIndices(int x, int y, int index, uint32_t* indices, int32_t arraySize)
    {
        const TileElement* source = GetTileElementAt(x, y, index, TileElementType::SmallScenery);
        const SmallSceneryElement* sceneryElement = source->AsSmallScenery();
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
                int32_t image_offset = entry->frame_offsets[frame];
                image_offset = (image_offset * 4);

                if (entry->HasFlag(SMALL_SCENERY_FLAG_VISIBLE_WHEN_ZOOMED | SMALL_SCENERY_FLAG17))
                {
                    image_offset += 4;
                }

                indices[frame] = (image_offset + GetIndexWithColourAndWither(sceneryElement, entry));
            }

            return frame;
        }

        dll_log("This small scenery entry is animated in a way that is not supported yet (flags = %i).", entry->flags);
        return 0;
    }

    /*
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
    */
}
