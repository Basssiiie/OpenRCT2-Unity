#include "../OpenRCT2.Bindings.h"
#include "../Utilities/Logging.h"
#include "../Utilities/TileElementHelper.h"

#include <iostream>
#include <openrct2/object/ObjectManager.h>
#include <openrct2/object/ObjectTypes.h>
#include <openrct2/object/SmallSceneryEntry.h>
#include <openrct2/world/Map.h>
#include <openrct2/world/Scenery.h>

extern "C"
{
    const uint8_t IdentifierSize = 50;

    struct SmallSceneryInfo
    {
        uint32_t imageIndex;
        ObjectEntryIndex objectIndex;
        uint8_t quadrant;
        bool fullTile;
        uint8_t colour1;
        uint8_t colour2;
        uint8_t colour3;
        bool animated;
        uint16_t animationFrameCount;
        uint16_t animationFrameDelay;
        char identifier[IdentifierSize];
    };

    // Adjusts the image index if the scenery element has colours or withering.
    uint32_t GetIndexWithWither(const SmallSceneryElement* element, const SmallSceneryEntry* entry)
    {
        uint32_t imageIndex = entry->image + element->GetDirection();

        // Wither flowers
        if (entry->HasFlag(SMALL_SCENERY_FLAG_CAN_WITHER))
        {
            uint8_t age = element->GetAge();

            if (age >= kSceneryWitherAgeThreshold1)
            {
                imageIndex += 4;
            }
            if (age >= kSceneryWitherAgeThreshold2)
            {
                imageIndex += 4;
            }
        }

        return imageIndex;
    }

    static void SetSmallSceneryInfo(int x, int y, int index, const TileElement* source, SmallSceneryInfo* target)
    {
        const SmallSceneryElement* scenery = source->AsSmallScenery();

        if (scenery == nullptr)
        {
            dll_log("Could not find scenery element at %i, %i, index %i", x, y, index);
            return;
        }

        const SmallSceneryEntry* entry = scenery->GetEntry();

        target->objectIndex = scenery->GetEntryIndex();
        target->imageIndex = GetIndexWithWither(scenery, entry);
        target->quadrant = scenery->GetSceneryQuadrant();
        target->fullTile = entry->HasFlag(SMALL_SCENERY_FLAG_FULL_TILE);
        target->colour1 = scenery->GetPrimaryColour();
        target->colour2 = scenery->GetSecondaryColour();
        target->colour3 = scenery->GetTertiaryColour();
        target->animated = entry->HasFlag(SMALL_SCENERY_FLAG_ANIMATED);

        if (entry->HasFlag(SMALL_SCENERY_FLAG_HAS_FRAME_OFFSETS))
        {
            target->animationFrameCount = entry->FrameOffsetCount;
            target->animationFrameDelay = entry->animation_delay & 0xFF;
        }
        else
        {
            target->animationFrameCount = 0xF;
            target->animationFrameDelay = 1;
        }

        const Object* object = ObjectEntryGetObject(ObjectType::SmallScenery, scenery->GetEntryIndex());
        object->GetIdentifier().copy(target->identifier, IdentifierSize);
    }

    // Writes the small scenery element details to the specified buffer.
    EXPORT void GetSmallSceneryElementAt(int x, int y, int index, SmallSceneryInfo* element)
    {
        const TileElement* source = GetTileElementAt(x, y, index, TileElementType::SmallScenery);
        SetSmallSceneryInfo(x, y, index, source, element);
    }

    // Writes all the small scenery element details to the specified buffer.
    EXPORT int GetAllSmallSceneryElementsAt(int x, int y, SmallSceneryInfo* elements, int length)
    {
        const TileElement* source = MapGetFirstElementAt(TileCoordsXY{ x, y });
        auto index = 0;

        do
        {
            if (source == nullptr)
                break;

            const TileElementType type = source->GetType();
            if (type != TileElementType::SmallScenery)
                continue;

            SetSmallSceneryInfo(x, y, index, source, elements);
            index++;
            elements++;

        } while (!(source++)->IsLastForTile() && index < length);

        return index;
    }

    // Get all indices of the animation of this small scenery element, returns the amount of animation frames.
    EXPORT int32_t GetSmallSceneryAnimationIndices(int x, int y, int index, uint32_t* indices, int32_t length)
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
            uint16_t max_frames = std::min(entry->FrameOffsetCount, (uint16_t)length);

            for (; frame < max_frames; frame++)
            {
                int32_t image_offset = entry->frame_offsets[frame];
                image_offset = (image_offset * 4);

                if (entry->HasFlag(SMALL_SCENERY_FLAG_VISIBLE_WHEN_ZOOMED | SMALL_SCENERY_FLAG17))
                {
                    image_offset += 4;
                }

                indices[frame] = (image_offset + GetIndexWithWither(sceneryElement, entry));
            }

            return frame;
        }

        dll_log("This small scenery entry is animated in a way that is not supported yet (flags = %i).", entry->flags);
        return 0;
    }
}
