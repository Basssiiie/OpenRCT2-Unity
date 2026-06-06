#include "../OpenRCT2.Bindings.h"
#include "../Utilities/Logging.h"

#include <algorithm>
#include <cstdint>
#include <openrct2/Context.h>
#include <openrct2/entity/EntityList.h>
#include <openrct2/entity/Guest.h>
#include <openrct2/entity/Peep.h>
#include <openrct2/entity/Staff.h>
#include <openrct2/GameState.h>
#include <openrct2/Identifiers.h>
#include <openrct2/object/ObjectManager.h>
#include <openrct2/object/PeepAnimationsObject.h>
#include <openrct2/peep/PeepSpriteIds.h>
#include <openrct2/ride/RideColour.h>

using namespace OpenRCT2::Drawing;

extern "C"
{
    EXPORT 

    struct PeepEntity
    {
        int32_t x;
        int32_t y;
        int32_t z;
        uint8_t direction;
        Colour tshirtColour;
        Colour trousersColour;
        Colour accessoryColour;
        ObjectEntryIndex animationObjectId;
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
        entity->animationObjectId = peep->AnimationObjectIndex;

        const auto group = peep->AnimationGroup;
        entity->animationGroup = group;

        if (peep->Action == PeepActionType::idle)
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
            case PeepAnimationGroup::umbrella:
                entity->accessoryColour = guest->UmbrellaColour;
                return;

            case PeepAnimationGroup::balloon:
                entity->accessoryColour = guest->BalloonColour;
                return;

            case PeepAnimationGroup::hat:
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

    // Inspired by PaintPeepGetBaseImageAndOffset in Paint.Peep.cpp
    EXPORT void GetPeepAnimationData(ObjectEntryIndex animationObjectId, PeepAnimationGroup group, PeepAnimationType type, PeepAnimationData* out)
    {
        auto& objManager = GetContext()->GetObjectManager();
        auto* animObj = objManager.GetLoadedObject<PeepAnimationsObject>(animationObjectId);
        auto& animation = animObj->GetPeepAnimation(group, type);

        auto baseImageId = animation.baseImage;
        auto& frames = animation.frameOffsets;

        out->baseImageId = baseImageId;
        out->length = (*std::max_element(frames.begin(), frames.end())) + 1;
        out->rotations = (type == PeepAnimationType::hanging) ? 1 : 4;

        if ((baseImageId >= kPeepSpriteHatStateWatchRideId && baseImageId < (kPeepSpriteHatStateSittingIdleId + 4))
            || (baseImageId >= kPeepSpriteBalloonStateWatchRideId && baseImageId < (kPeepSpriteBalloonStateSittingIdleId + 4))
            || (baseImageId >= kPeepSpriteUmbrellaStateWalkingId && baseImageId < (kPeepSpriteUmbrellaStateSittingIdleId + 4)))
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
    EXPORT bool GetGuestStats(uint16_t entityIndex, GuestStats* stats)
    {
        auto& entities = getGameState().entities;
        const auto entityId = EntityId::FromUnderlying(entityIndex);
        const Guest* guest = entities.TryGetEntity<Guest>(entityId);

        if (guest == nullptr)
        {
            dll_log("Peep does not exist anymore. ( entity id: %i )", entityIndex);
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
