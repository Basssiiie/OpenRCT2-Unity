#include "./TileElementHelper.h"

#include "./Logging.h"

#include <openrct2/world/Map.h>

// Get a tile element at a specified x, y, and index.
const TileElement* GetTileElementAt(int x, int y, int index)
{
    const CoordsXY coords = CoordsXY(x * kCoordsXYStep, y * kCoordsXYStep).ToTileStart();
    const TileElement* target = MapGetFirstElementAt(coords);

    return &target[index];
}

// Get a tile element at a specified x, y, and index of the given type.
const TileElement* GetTileElementAt(int x, int y, int index, TileElementType type)
{
    const TileElement* source = GetTileElementAt(x, y, index);
    TileElementType actual = source->GetType();

    if (actual != type)
    {
        dll_log("Tile element at %i, %i, index %i is not of required type %i, but of type %i!", x, y, index, type, actual);
    }

    return source;
}
