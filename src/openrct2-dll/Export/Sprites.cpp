#include <openrct2/ride/Vehicle.h>
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


    struct RideVehicle
    {
    public:
        uint16_t idx;
        int32_t x;
        int32_t y;
        int32_t z;
        uint8_t direction;
        uint8_t bankRotation;
        uint8_t pitchRotation;
        uint8_t trackType;
        uint8_t trackDirection;
        uint16_t trackProgress;
    };

    
    // Loads all the vehicles into the specified buffer, returns the total amount of vehicles loaded.
    EXPORT int GetAllVehicles(RideVehicle* vehicles, int arraySize)
    {
        Vehicle *train, *vehicle;
        int vehicleCount = 0;
        uint16_t train_index, vehicle_index;

        for (train_index = gSpriteListHead[SPRITE_LIST_TRAIN_HEAD]; train_index != SPRITE_INDEX_NULL; train_index = train->next)
        {
            train = GET_VEHICLE(train_index);
            for (vehicle_index = train_index; vehicle_index != SPRITE_INDEX_NULL; vehicle_index = vehicle->next_vehicle_on_train)
            {
                vehicle = GET_VEHICLE(vehicle_index);
                if (vehicle->x == LOCATION_NULL)
                    continue;

                //printf("(me) %i vehicle %i at %i, %i, %i\n", vehicleCount, vehicle->sprite_index, vehicle->x, vehicle->y, vehicle->z);

                RideVehicle* target = &vehicles[vehicleCount];
                target->idx = vehicle->sprite_index;

                target->x = vehicle->x;
                target->y = vehicle->y;
                target->z = vehicle->z;

                target->direction = vehicle->sprite_direction;
                target->bankRotation = vehicle->bank_rotation;
                target->pitchRotation = vehicle->vehicle_sprite_type;

                target->trackType = (vehicle->track_type >> 2);
                target->trackDirection = (vehicle->track_direction & 0b11);
                target->trackProgress = vehicle->track_progress;

                vehicleCount++;

                if (vehicleCount >= arraySize)
                    break;
            }
        }
        return vehicleCount;
    }
}
