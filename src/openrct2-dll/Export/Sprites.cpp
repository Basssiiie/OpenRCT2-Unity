#include <openrct2/world/Sprite.h>

#include "Openrct2-dll.h"


extern "C"
{
    // Gets the amount of sprites currently active for the given type.
    // All type possibilities are found in the 'SPRITE_LIST'-enum in 'world/Sprite.h'.
    EXPORT int GetSpriteCount(int spriteType)
    {
        return gSpriteListCount[spriteType];
    }


    // Loads all the peeps into the specified buffer, returns the total amount of peeps loaded.
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

    
    // Loads all the vehicles into the specified buffer, returns the total amount of vehicles loaded.
    EXPORT int GetAllVehicles(Vehicle* vehicles, int arraySize)
    {
        Vehicle* vehicle;
        int vehicleCount = 0;

        for (uint16_t i = gSpriteListHead[SPRITE_LIST_VEHICLE]; i != SPRITE_INDEX_NULL; i = vehicle->next)
        {
            vehicle = &get_sprite(i)->vehicle;

            vehicles[vehicleCount] = *vehicle;
            vehicleCount++;

            if (vehicleCount >= arraySize)
                break;
        }
        return vehicleCount;
    }
}
