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
        uint8_t colour1;
        uint8_t colour2;
        uint8_t colour3;
        uint16_t animationFrameCount;
        uint16_t animationFrameDelay;
        bool animated;
    };

    // Returns the sprite image index for a small scenery tile element.
    //  Inspired by: PaintWall(), PaintWallWall()
    uint32_t GetWallImageIndex(const WallElement* element, const WallSceneryEntry* entry)
    {
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

        uint8_t wallFlags = entry->flags;
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
        return entry->image + imageOffset;
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
        target->colour1 = wall->GetPrimaryColour();
        target->colour2 = wall->GetSecondaryColour();
        target->colour3 = wall->GetTertiaryColour();
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
