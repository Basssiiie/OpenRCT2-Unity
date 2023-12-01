#include "../OpenRCT2.Bindings.h"
#include "../Utilities/Logging.h"

#include <openrct2/world/Park.h>

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
        dll_log("GetMapSize(%d, %d)", gMapSize.x, gMapSize.y);
        size->width = gMapSize.x;
        size->height = gMapSize.y; 
    }
}
