
#include <openrct2/world/Park.h>

#include "Openrct2-dll.h"


extern "C"
{
    EXPORT int GetMapSize()
    {
        return gMapSize;
    }


    EXPORT void GetMapElementAt(int x, int y, TileElement* element)
    {
        CoordsXY coords = CoordsXY(x * COORDS_XY_STEP, y * COORDS_XY_STEP).ToTileStart();
        *element = *map_get_first_element_at(coords);
        /*
        printf("(me) GetMapElementAt( %i, %i ) = ( %i, %i )\n", x, y, coords.x, coords.y);
        printf("(me)   -> %i \t(type)\n", element->type);
        printf("(me)   -> %i \t(flags)\n", element->Flags);
        printf("(me)   -> %i \t(base_height)\n", element->base_height);
        printf("(me)   -> %i \t(clearance_height)\n", element->clearance_height);
        printf("(me)   -> %i \t(pad 0x1)\n", element->pad_04[0]);
        printf("(me)   -> %i \t(pad 0x2)\n", element->pad_04[1]);
        printf("(me)   -> %i \t(pad 0x3)\n", element->pad_04[2]);
        printf("(me)   -> %i \t(pad 0x4)\n", element->pad_04[3]);
        printf("(me)   -> %i \t(pad 0x5)\n", element->pad_08[0]);
        printf("(me)   -> %i \t(pad 0x6)\n", element->pad_08[1]);
        printf("(me)   -> %i \t(pad 0x7)\n", element->pad_08[2]);
        printf("(me)   -> %i \t(pad 0x8)\n", element->pad_08[3]);
        printf("(me)   -> %i \t(pad 0x9)\n", element->pad_08[4]);
        printf("(me)   -> %i \t(pad 0xA)\n", element->pad_08[5]);
        printf("(me)   -> %i \t(pad 0xB)\n", element->pad_08[6]);
        printf("(me)   -> %i \t(pad 0xC)\n", element->pad_08[7]);
        */
    }


    EXPORT int GetMapElementsAt(int x, int y, TileElement* elements, int arraySize)
    {
        int elementCount = 0;

        CoordsXY coords = CoordsXY(x * COORDS_XY_STEP, y * COORDS_XY_STEP).ToTileStart();
        TileElement* element = map_get_first_element_at(coords);

        for (int i = 0; i < arraySize; i++)
        {
            elements[i] = *element;
            elementCount++;

            if (element->IsLastForTile())
                break;

            element++;
        }

        // printf("(me) GetMapElementsAt( %i, %i ) = %i\n", x, y, elementCount);
        return elementCount;
    }
}
