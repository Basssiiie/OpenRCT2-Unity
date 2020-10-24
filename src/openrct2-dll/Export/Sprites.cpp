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


    struct PeepEntity
    {
    public:
        uint16_t idx;
        int32_t x;
        int32_t y;
        int32_t z;

        uint8_t tshirt_colour;
        uint8_t trousers_colour;
    };


    // Loads all the peeps into the specified buffer, returns the total amount of peeps loaded.
    EXPORT int GetAllPeeps(PeepEntity* peeps, int arraySize)
    {
        Peep* peep;
        uint16_t spriteIndex;
        int peepCount = 0;

        FOR_ALL_PEEPS (spriteIndex, peep)
        {
            PeepEntity* data = &peeps[peepCount];
            data->idx = peep->sprite_index;
            data->x = peep->x;
            data->y = peep->y;
            data->z = peep->z;
            data->tshirt_colour = peep->tshirt_colour;
            data->trousers_colour = peep->trousers_colour;

            peepCount++;

            if (peepCount >= arraySize)
                break;
        }
        return peepCount;
    }


    struct PeepStats
    {
    public:
        uint8_t energy;
        uint8_t happiness;
        uint8_t nausea;
        uint8_t hunger;
        uint8_t thirst;
        uint8_t toilet;
        uint8_t intensity;
    };


    EXPORT bool GetPeepStats(uint16_t spriteIndex, PeepStats* peepStats)
    {
        Peep* peep = GET_PEEP(spriteIndex);

        if (peep != nullptr)
        {
            printf("(me) Peep does not exist anymore. ( sprite id: %i )\n", spriteIndex);
            return false;
        }

        peepStats->energy = peep->energy;
        peepStats->happiness = peep->happiness;
        peepStats->nausea = peep->nausea;
        peepStats->hunger = peep->hunger;
        peepStats->thirst = peep->thirst;
        peepStats->toilet = peep->toilet;
        peepStats->intensity = peep->intensity;
        return true;
    }


    struct VehicleEntity
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
    EXPORT int GetAllVehicles(VehicleEntity* vehicles, int arraySize)
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

                VehicleEntity* target = &vehicles[vehicleCount];
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
