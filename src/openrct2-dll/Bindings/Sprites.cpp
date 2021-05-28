#include <openrct2/world/EntityList.h>
#include <openrct2/world/Sprite.h>

#include "OpenRCT2-DLL.h"


extern "C"
{
    // Gets the amount of sprites currently active for the given type.
    // All type possibilities are found in the 'SPRITE_LIST'-enum in 'world/Sprite.h'.
    EXPORT int GetSpriteCount(int spriteType)
    {
        return GetEntityListCount(EntityListId(spriteType));
    }
}
