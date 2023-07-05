#include "../openrct2-bindings.h"

#include <openrct2/world/Park.h>

extern "C"
{
    // Gets the amount of tiles on a single edge of the map.
    EXPORT int GetMapSize()
    {
        return gMapSize.x; // TODO: make it work with a different y as well.
    }

    // Gets the first tile-element at the the specified coordinates.
    EXPORT void GetMapElementAt(int x, int y, TileElement* element)
    {
        CoordsXY coords = CoordsXY(x * COORDS_XY_STEP, y * COORDS_XY_STEP).ToTileStart();
        *element = *MapGetFirstElementAt(coords);
    }

    // Writes all tile-elements at the the specified coordinates to the specified buffer.
    EXPORT int GetMapElementsAt(int x, int y, TileElement* elements, int arraySize)
    {
        int elementCount = 0;

        CoordsXY coords = CoordsXY(x * COORDS_XY_STEP, y * COORDS_XY_STEP).ToTileStart();
        TileElement* element = MapGetFirstElementAt(coords);

        for (int i = 0; i < arraySize; i++)
        {
            elements[i] = *element;
            elementCount++;

            if (element->IsLastForTile())
                break;

            element++;
        }
        return elementCount;
    }
}
