#include <openrct2/world/Sprite.h>

#include "OpenRCT2-DLL.h"

extern "C"
{
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


    // Writes statistics about the specified peep to the specified struct, returns true
    // or false depending on whether the peep existed or not.
    EXPORT bool GetPeepStats(uint16_t spriteIndex, PeepStats* peepStats)
    {
        Peep* peep = TryGetEntity<Peep>(spriteIndex);

        if (peep == nullptr)
        {
            dll_log("Peep does not exist anymore. ( sprite id: %i )", spriteIndex);
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
}
