
#include <openrct2/world/Sprite.h>

#include "Openrct2-dll.h"


extern "C"
{
    EXPORT int GetSpriteCount(int spriteType)
    {
        return gSpriteListCount[spriteType];
    }


    EXPORT int GetAllPeeps(Peep* peeps, int arraySize)
    {
        Peep* peep;
        uint16_t spriteIndex;
        int peepCount = 0;

        FOR_ALL_PEEPS (spriteIndex, peep)
        {
            peeps[peepCount] = *peep;
            peepCount++;

            if (peepCount >= arraySize)
                break;
        }
        return peepCount;
    }
}
