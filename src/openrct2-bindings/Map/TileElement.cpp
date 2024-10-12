#include "../OpenRCT2.Bindings.h"
#include "../Utilities/Logging.h"
#include "../Utilities/TileElementHelper.h"

#include <openrct2/world/Park.h>

extern "C"
{
    struct TileElementInfo
    {
        TileElementType type;
        uint8_t rotation;
        uint8_t baseHeight;
        bool invisible;
    };

    void SetTileElementInfo(TileElementInfo* info, const TileElement* element)
    {
        info->type = element->GetType();
        info->rotation = element->GetDirection();
        info->baseHeight = element->BaseHeight;
        info->invisible = element->IsInvisible();
    }

    // Gets the specified tile-element at the the specified coordinates.
    EXPORT void GetMapElementAt(int x, int y, int index, TileElementInfo* element)
    {
        const TileElement* source = GetTileElementAt(x, y, index);
        SetTileElementInfo(element, source);
    }

    // Writes all tile-elements at the the specified coordinates to the specified buffer.
    EXPORT int GetMapElementsAt(int x, int y, TileElementInfo* elements, int length)
    {
        const TileElement* source = GetTileElementAt(x, y, 0);
        int elementCount = 0;

        for (int i = 0; i < length; i++)
        {
            SetTileElementInfo(&elements[i], source);
            elementCount++;

            if (source->IsLastForTile())
                break;

            source++;
        }

        return elementCount;
    }
}
