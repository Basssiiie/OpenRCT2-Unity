#include "../OpenRCT2.Bindings.h"
#include "../Utilities/Logging.h"

#include <openrct2/GameState.h>

extern "C"
{
    struct MapSize
    {
        int32_t width;
        int32_t height;
    };

    // Gets the amount of tiles on both edges of the map.
    EXPORT void GetMapSize(MapSize* size)
    {
        const auto& mapSize = GetGameState().MapSize;
        dll_log("GetMapSize(%d, %d)", mapSize.x, mapSize.y);
        size->width = mapSize.x;
        size->height = mapSize.y;
    }
}
