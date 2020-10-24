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
        int peepCount = 0;

        for (Peep* peep : EntityList<Peep>(EntityListId::Peep))
        {
            PeepEntity* data = &peeps[peepCount];
            data->idx = peep->sprite_index;
            data->x = peep->x;
            data->y = peep->y;
            data->z = peep->z;
            data->tshirt_colour = peep->TshirtColour;
            data->trousers_colour = peep->TrousersColour;

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
        Peep* peep = TryGetEntity<Peep>(spriteIndex);

        if (peep == nullptr)
        {
            printf("(me) Peep does not exist anymore. ( sprite id: %i )\n", spriteIndex);
            return false;
        }

        peepStats->energy = peep->Energy;
        peepStats->happiness = peep->Happiness;
        peepStats->nausea = peep->Nausea;
        peepStats->hunger = peep->Hunger;
        peepStats->thirst = peep->Thirst;
        peepStats->toilet = peep->Toilet;
        peepStats->intensity = (uint8_t)peep->Intensity;
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
        int vehicleCount = 0;

        for (Vehicle* vehicle : EntityList<Vehicle>(EntityListId::Vehicle))
        {
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
        return vehicleCount;
    }
}
