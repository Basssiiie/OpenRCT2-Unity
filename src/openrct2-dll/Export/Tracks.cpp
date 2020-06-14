#include <openrct2/ride/RideData.h>
#include <openrct2/ride/TrackData.h>
#include <openrct2/ride/Vehicle.h>
#include <openrct2/ride/VehicleSubpositionData.h>

#include "Openrct2-dll.h"


// Rounds a number to the nearest multiple of 'multiple'.
int RoundToMultiple(int value, int multiple)
{
    int half = multiple / 2;
    int retval = (value < 0) ? (value - half) : (value + half);

    return (retval) / multiple * multiple;
}


// Hack: manually fix the gaps. 
// (please tell me if you know a better way to fix there gaps, without any bumps!)
void FixTrackPiecePosition(rct_vehicle_info* target, uint32_t trackType, uint8_t slope)
{
    switch (slope)
    {
        case TRACK_SLOPE_UP_90:
        case TRACK_SLOPE_DOWN_90:
            target->z = RoundToMultiple(target->z, 8);
            break;

        default:
            target->x = RoundToMultiple(target->x, 16);
            target->y = RoundToMultiple(target->y, 16);
            target->z = RoundToMultiple(target->z, 8);
            break;
    }

    // Custom hacks for specific track types.
    switch (trackType)
    {
        case TRACK_ELEM_LEFT_CURVED_LIFT_HILL:
        case TRACK_ELEM_RIGHT_CURVED_LIFT_HILL:
            target->vehicle_sprite_type = 0;
            break;
    }
}

    
extern "C"
{
    // Returns the amount of path nodes in the pathing route for the specified track type.
    EXPORT int GetTrackElementRouteSize(int32_t trackVariant, int32_t typeAndDirection)
    {
        return gTrackVehicleInfo[trackVariant][typeAndDirection]->size;
    }


    // Returns the pathing route for the specified track element.
    EXPORT void GetTrackElementRoute(int32_t trackVariant, int32_t typeAndDirection, rct_vehicle_info* nodes, int32_t arraySize)
    {
        const rct_vehicle_info_list* list = gTrackVehicleInfo[trackVariant][typeAndDirection];

        std::memcpy(nodes, list->info, sizeof(rct_vehicle_info) * arraySize);

        int32_t trackType = (typeAndDirection >> 2);
        const rct_trackdefinition definition = TrackDefinitions[trackType];

        FixTrackPiecePosition(&nodes[0], trackType, definition.vangle_start);
        FixTrackPiecePosition(&nodes[arraySize - 1], trackType, definition.vangle_end);
    }


    // (Temporary?) Returns the flags for the specified track type.
    EXPORT uint16_t GetTrackTypeFlags(int32_t trackType)
    {
        return TrackFlags[trackType];
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
