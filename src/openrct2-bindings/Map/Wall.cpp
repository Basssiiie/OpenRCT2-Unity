#include "../OpenRCT2.Bindings.h"
#include "../Utilities/Logging.h"
#include "../Utilities/TileElementHelper.h"

#include <openrct2/object/WallSceneryEntry.h>
#include <openrct2/world/Map.h>

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

    static void SetWallInfo(int x, int y, int index, const TileElement* source, WallInfo* target)
    {
        const WallElement* wall = source->AsWall();

        if (wall == nullptr)
        {
            dll_log("Could not find wall element at %i, %i, index %i", x, y, index);
            return;
        }

        const WallSceneryEntry* entry = wall->GetEntry();

        target->imageIndex = GetWallImageIndex(wall, entry);
        target->slope = wall->GetSlope();
        target->animated = (entry->flags2 & WALL_SCENERY_2_ANIMATED);
    }

    // Writes the wall element details to the specified buffer.
    EXPORT void GetWallElementAt(int x, int y, int index, WallInfo* element)
    {
        const TileElement* source = GetTileElementAt(x, y, index, TileElementType::Wall);
        SetWallInfo(x, y, index, source, element);
    }
        
    // Writes all the wall element details to the specified buffer.
    EXPORT int GetAllWallElementsAt(int x, int y, WallInfo* elements, int length)
    {
        const TileElement* source = MapGetFirstElementAt(TileCoordsXY{ x, y });
        auto index = 0;

        do
        {
            if (source == nullptr)
                break;

            const TileElementType type = source->GetType();
            if (type != TileElementType::Wall)
                continue;

            SetWallInfo(x, y, index, source, elements);
            index++;
            elements++;

        } while (!(source++)->IsLastForTile() && index < length);

        return index;
    }
}
