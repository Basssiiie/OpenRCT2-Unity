#include <openrct2/ride/RideData.h>
#include <openrct2/ride/TrackData.h>
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


    // (Temporary?) Returns the track height offset for this ride index.
    EXPORT int8_t GetTrackHeightOffset(uint8_t rideIndex)
    {
        auto ride = get_ride(rideIndex);
        if (ride == nullptr)
        {
            printf("(me) GetTrackHeightOffset: ride %i not found.\n", rideIndex);
            return 0;
        }

        return RideData5[ride->type].z_offset;
    }


    // (Temporary?) Returns the ride colours for this ride.
    EXPORT void GetRideTrackColours(uint8_t rideIndex, TrackColour* colours)
    {
        auto ride = get_ride(rideIndex);
        if (ride == nullptr)
        {
            printf("(me) GetRideTrackColours: ride %i not found.\n", rideIndex);
            return;
        }

        std::memcpy(colours, ride->track_colour, sizeof(TrackColour) * NUM_COLOUR_SCHEMES);
    }
}
