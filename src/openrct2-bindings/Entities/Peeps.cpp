#include "../OpenRCT2.Bindings.h"
#include "../Utilities/Logging.h"

#include <openrct2/entity/EntityList.h>
#include <openrct2/entity/Guest.h>
#include <openrct2/entity/Peep.h>
#include <openrct2/entity/Staff.h>
#include <openrct2/peep/PeepAnimationData.h>

extern "C"
{
    struct PeepEntity
    {
        uint16_t id;
        int32_t x;
        int32_t y;
        int32_t z;
        uint8_t direction;
        uint32_t imageId;
    };

    // Gets the current sprite image id of this peep, excluding rotation.
    //  Inspired by: Peep.cpp/Peep::Paint()
    uint32_t GetCurrentImageId(const Peep* peep)
    {
        PeepAnimationType actionAnimationGroup = peep->AnimationType;
        uint8_t imageOffset = peep->AnimationImageIdOffset;

        if (peep->Action == PeepActionType::Idle)
        {
            actionAnimationGroup = peep->NextAnimationType;
            imageOffset = 0;
        }

        PeepAnimationGroup animationGroup = peep->AnimationGroup;
        uint32_t baseImageId = GetPeepAnimation(animationGroup, actionAnimationGroup).base_image;
        // + (imageDirection >> 3)

        if (actionAnimationGroup != PeepAnimationType::Hanging)
            baseImageId += imageOffset * 4;
        else
            baseImageId += imageOffset;

        auto imageId = ImageId(baseImageId, peep->TshirtColour, peep->TrousersColour);
        return imageId.ToUInt32();
    }

    void SetPeepInfo(PeepEntity* entity, const Peep* peep)
    {
        entity->id = peep->Id.ToUnderlying();
        entity->x = peep->x;
        entity->y = peep->y;
        entity->z = peep->z;
        entity->direction = peep->PeepDirection;
        entity->imageId = GetCurrentImageId(peep);
    }

    // Loads all the peeps into the specified buffer, returns the total amount of peeps loaded.
    EXPORT int32_t GetAllPeeps(PeepEntity* peeps, int32_t arraySize)
    {
        int32_t peepCount = 0;

        for (const Guest* guest : EntityList<Guest>())
        {
            if (peepCount >= arraySize)
                break;

            SetPeepInfo(&peeps[peepCount], guest);
            peepCount++;
        }
        for (const Staff* staff : EntityList<Staff>())
        {
            if (peepCount >= arraySize)
                break;

            SetPeepInfo(&peeps[peepCount], staff);
            peepCount++;
        }
        return peepCount;
    }

    struct GuestStats
    {
        uint8_t energy;
        uint8_t happiness;
        uint8_t nausea;
        uint8_t hunger;
        uint8_t thirst;
        uint8_t toilet;
        uint8_t minimumIntensity;
        uint8_t maximumIntensity;
    };

    // Writes statistics about the specified peep to the specified struct, returns true
    // or false depending on whether the peep existed or not.
    EXPORT bool GetGuestStats(uint16_t spriteIndex, GuestStats* stats)
    {
        const Guest* guest = TryGetEntity<Guest>(EntityId::FromUnderlying(spriteIndex));

        if (guest == nullptr)
        {
            dll_log("Peep does not exist anymore. ( sprite id: %i )", spriteIndex);
            return false;
        }

        stats->energy = guest->Energy;
        stats->happiness = guest->Happiness;
        stats->nausea = guest->Nausea;
        stats->hunger = guest->Hunger;
        stats->thirst = guest->Thirst;
        stats->toilet = guest->Toilet;
        stats->minimumIntensity = guest->Intensity.GetMinimum();
        stats->maximumIntensity = guest->Intensity.GetMaximum();
        return true;
    }
}
