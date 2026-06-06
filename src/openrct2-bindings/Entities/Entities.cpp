#include "../OpenRCT2.Bindings.h"

#include <cstdint>
#include <openrct2/GameState.h>
#include <openrct2/entity/EntityBase.h>
#include <openrct2/entity/EntityRegistry.h>

extern "C"
{
    EXPORT int GetEntityCount(EntityType type)
    {
        return getGameState().entities.GetEntityListCount(type);
    }

    struct EntityCounts
    {
        uint16_t vehicles;
        uint16_t guests;
        uint16_t staff;
    };

    EXPORT void GetEntityCounts(EntityCounts* counts)
    {
        auto& entities = getGameState().entities;
        counts->vehicles = entities.GetEntityListCount(EntityType::vehicle);
        counts->guests = entities.GetEntityListCount(EntityType::guest);
        counts->staff = entities.GetEntityListCount(EntityType::staff);
    }
}
