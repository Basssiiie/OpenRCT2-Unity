#include <openrct2/ride/Vehicle.h>
#include <openrct2/ride/VehicleSubpositionData.h>

#include "Openrct2-dll.h"

extern "C"
{
    // Returns the amount of path nodes in the pathing route for the specified track type.
    EXPORT int GetTrackElementRouteSize(int32_t trackVariant, int32_t typeAndDirection)
    {
        return gTrackVehicleInfo[trackVariant][typeAndDirection]->size;
    }


    // Returns the pathing route for the specified track element.
    EXPORT void GetTrackElementRoute(int32_t trackVariant, int32_t typeAndDirection, rct_vehicle_info* nodes, int arraySize)
    {
        const rct_vehicle_info_list* list = gTrackVehicleInfo[trackVariant][typeAndDirection];

        std::memcpy(nodes, list->info, sizeof(rct_vehicle_info) * arraySize);
    }
}
