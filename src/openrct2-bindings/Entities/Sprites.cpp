#include "../OpenRCT2.Bindings.h"

#include <openrct2/entity/EntityList.h>

extern "C"
{
    // Gets the amount of sprites currently active for the given type.
    EXPORT int GetSpriteCount(int spriteType)
    {
        return GetEntityListCount(EntityType(spriteType));
    }
}
