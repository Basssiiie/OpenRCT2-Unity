#include <openrct2/entity/EntityList.h>

#include "OpenRCT2-DLL.h"


extern "C"
{
    // Gets the amount of sprites currently active for the given type.
    EXPORT int GetSpriteCount(int spriteType)
    {
        return GetEntityListCount(EntityType(spriteType));
    }
}
