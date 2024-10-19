#include "../OpenRCT2.Bindings.h"
#include "../Utilities/Logging.h"

#include <openrct2/entity/EntityList.h>
#include <openrct2/entity/Guest.h>
#include <openrct2/entity/Peep.h>
#include <openrct2/entity/Staff.h>
#include <openrct2/peep/PeepAnimationData.h>
#include <openrct2/peep/PeepSpriteIds.h>

extern "C"
{
    EXPORT 

    struct PeepEntity
    {
        int32_t x;
        int32_t y;
        int32_t z;
        uint8_t direction;
        uint8_t tshirtColour;
        uint8_t trousersColour;
        uint8_t accessoryColour;
        PeepAnimationGroup animationGroup;
        PeepAnimationType animationType;
        uint8_t animationOffset;
    };

    static void SetPeepInfo(PeepEntity* entity, const Peep* peep)
    {
        entity->x = peep->x;
        entity->y = peep->y;
        entity->z = peep->z;
        entity->direction = peep->PeepDirection;
        entity->tshirtColour = peep->TshirtColour;
        entity->trousersColour = peep->TrousersColour;

        const auto group = peep->AnimationGroup;
        entity->animationGroup = group;

        if (peep->Action == PeepActionType::Idle)
        {
            entity->animationType = peep->NextAnimationType;
            entity->animationOffset = 0;
        }
        else
        {
            entity->animationType = peep->AnimationType;
            entity->animationOffset = peep->AnimationImageIdOffset;
        }

        auto* guest = peep->As<Guest>();
        if (guest == nullptr)
        {
            return;
        }

        switch (group) 
        {
            case PeepAnimationGroup::Umbrella:
                entity->accessoryColour = guest->UmbrellaColour;
                return;

            case PeepAnimationGroup::Balloon:
                entity->accessoryColour = guest->BalloonColour;
                return;

            case PeepAnimationGroup::Hat:
                entity->accessoryColour = guest->HatColour;
                return;
        }
    }

    // Loads all the guests into the specified buffer, returns the total amount of guests loaded.
    EXPORT int32_t GetAllGuests(PeepEntity* peeps, int32_t length)
    {
        int32_t peepCount = 0;

        for (const Guest* guest : EntityList<Guest>())
        {
            if (peepCount >= length)
                break;

            SetPeepInfo(&peeps[peepCount], guest);
            peepCount++;
        }

        return peepCount;
    }
            
    // Loads all the staff into the specified buffer, returns the total amount of staff loaded.
    EXPORT int32_t GetAllStaff(PeepEntity* peeps, int32_t length)
    {
        int32_t peepCount = 0;

        for (const Staff* staff : EntityList<Staff>())
        {
            if (peepCount >= length)
                break;

            SetPeepInfo(&peeps[peepCount], staff);
            peepCount++;
        }
        return peepCount;
    }

    struct PeepAnimationData
    {
        uint32_t baseImageId;
        uint8_t accessoryImageOffset;
        uint8_t length;
        uint8_t rotations;
    };

    EXPORT void GetPeepAnimationData(PeepAnimationGroup group, PeepAnimationType type, PeepAnimationData* out)
    {
        const auto& animation = GetPeepAnimation(group, type);
        const auto& frames = animation.frame_offsets;
        const auto baseImageId = animation.base_image;

        out->baseImageId = baseImageId;
        out->length = (*std::max_element(frames.begin(), frames.end())) + 1;
        out->rotations = (type == PeepAnimationType::Hanging) ? 1 : 4;

        if ((baseImageId >= kPeepSpriteHatStateWatchRideId && baseImageId < (kPeepSpriteHatStateSittingIdleId + 4))
            || (baseImageId >= kPeepSpriteBalloonStateWatchRideId && baseImageId < (kPeepSpriteBalloonStateSittingIdleId + 4))
            || (baseImageId >= kPeepSpriteUmbrellaStateNoneId && baseImageId < (kPeepSpriteUmbrellaStateSittingIdleId + 4)))
        {
            out->accessoryImageOffset = 32;
        }
        else
        {
            out->accessoryImageOffset = 0;
        }
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
