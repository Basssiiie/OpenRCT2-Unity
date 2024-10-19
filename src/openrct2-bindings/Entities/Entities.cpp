#include "../OpenRCT2.Bindings.h"
#include "../Utilities/Logging.h"

#include <openrct2/entity/EntityList.h>
#include <openrct2/entity/Guest.h>
#include <openrct2/entity/Staff.h>
#include <openrct2/ride/Vehicle.h>

extern "C"
{
    EXPORT int GetEntityCount(EntityType type)
    {
        return GetEntityListCount(type);
    }

    struct EntityCounts
    {
        uint16_t vehicles;
        uint16_t guests;
        uint16_t staff;
    };

    EXPORT void GetEntityCounts(EntityCounts* counts)
    {
        counts->vehicles = GetEntityListCount(EntityType::Vehicle);
        counts->guests = GetEntityListCount(EntityType::Guest);
        counts->staff = GetEntityListCount(EntityType::Staff);
    }
}
