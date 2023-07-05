#include "../openrct2-bindings.h"

#include <openrct2/entity/EntityList.h>
#include <openrct2/entity/Guest.h>
#include <openrct2/entity/Peep.h>
#include <openrct2/entity/Staff.h>

extern "C"
{
    struct PeepEntity
    {
    public:
        uint16_t idx;
        int32_t x;
        int32_t y;
        int32_t z;
        uint8_t direction;
        uint32_t imageId;

        // uint8_t tshirt_colour;
        // uint8_t trousers_colour;
    };

    // Gets the current sprite image id of this peep, excluding rotation.
    uint32_t GetCurrentImageId(Peep* peep)
    {
        // Borrowed from Paint.Peep.cpp/peep_paint()
        PeepAnimationEntry sprite = g_peep_animation_entries[EnumValue(peep->SpriteType)];

        PeepSpriteType spriteType = peep->SpriteType;
        PeepActionSpriteType actionSpriteType = peep->ActionSpriteType;
        uint8_t imageOffset = peep->ActionSpriteImageOffset;

        if (peep->Action == PeepActionType::Idle)
        {
            actionSpriteType = peep->NextActionSpriteType;
            imageOffset = 0;
        }

        uint32_t baseImageId = (GetPeepAnimation(spriteType, actionSpriteType).base_image + imageOffset * 4); // +
                                                                                                              // (imageDirection
                                                                                                              // >> 3)
        ImageId imageId = ImageId(baseImageId, peep->TshirtColour, peep->TrousersColour);

        return imageId.ToUInt32();
    }

    void SetPeepInfo(PeepEntity* entity, Peep* peep)
    {
        entity->idx = peep->Id.ToUnderlying();
        entity->x = peep->x;
        entity->y = peep->y;
        entity->z = peep->z;
        entity->direction = peep->PeepDirection;
        entity->imageId = GetCurrentImageId(peep);

        // entity->tshirt_colour = peep->TshirtColour;
        // entity->trousers_colour = peep->TrousersColour;
    }

    // Loads all the peeps into the specified buffer, returns the total amount of peeps loaded.
    EXPORT int32_t GetAllPeeps(PeepEntity* peeps, int32_t arraySize)
    {
        int32_t peepCount = 0;

        for (Guest* guest : EntityList<Guest>())
        {
            if (peepCount >= arraySize)
                break;

            SetPeepInfo(&peeps[peepCount], guest);
            peepCount++;
        }
        for (Staff* staff : EntityList<Staff>())
        {
            if (peepCount >= arraySize)
                break;

            SetPeepInfo(&peeps[peepCount], staff);
            peepCount++;
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
        Guest* guest = TryGetEntity<Guest>(EntityId::FromUnderlying(spriteIndex));

        if (guest == nullptr)
        {
            dll_log("Peep does not exist anymore. ( sprite id: %i )", spriteIndex);
            return false;
        }

        peepStats->energy = guest->Energy;
        peepStats->happiness = guest->Happiness;
        peepStats->nausea = guest->Nausea;
        peepStats->hunger = guest->Hunger;
        peepStats->thirst = guest->Thirst;
        peepStats->toilet = guest->Toilet;
        peepStats->intensity = (uint8_t)guest->Intensity;
        return true;
    }
}
