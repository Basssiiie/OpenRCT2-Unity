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
        uint8_t clearanceHeight;
        bool invisible;

        uint8_t padding[3]; // todo: test this?
    };


    void SetTileElementInfo(TileElementInfo* info, const TileElement* element)
    {
        info->type = element->GetType();
        info->rotation = element->GetDirection();
        info->baseHeight = element->BaseHeight;
        info->clearanceHeight = element->ClearanceHeight;
        info->invisible = element->IsInvisible();
    }

    // Gets the specified tile-element at the the specified coordinates.
    EXPORT void GetMapElementAt(int x, int y, int index, TileElementInfo* element)
    {
        const TileElement* source = GetTileElementAt(x, y, index);
        SetTileElementInfo(element, source);
    }

    // Writes all tile-elements at the the specified coordinates to the specified buffer.
    EXPORT int GetMapElementsAt(int x, int y, TileElementInfo* elements, int arraySize)
    {
        dll_log("Cpp TileElementInfo size: %li", sizeof(TileElementInfo));

        const TileElement* source = GetTileElementAt(x, y, 0);
        int elementCount = 0;

        for (int i = 0; i < arraySize; i++)
        {
            SetTileElementInfo(&elements[i], source);
            elementCount++;

            const uint32_t* ptr = (uint32_t*)&elements[i];
            dll_log("%i -> %08x %08x\n", 1, ptr[1], ptr[0]);

            if (source->IsLastForTile())
                break;

            source++;
        }
        return elementCount;
    }
}
