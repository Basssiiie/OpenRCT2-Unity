#include "../openrct2-bindings.h"

#include <openrct2/ride/RideData.h>
#include <openrct2/ride/Track.h>
#include <openrct2/ride/TrackData.h>
#include <openrct2/ride/Vehicle.h>
#include <openrct2/ride/VehicleSubpositionData.h>

// Rounds a number to the nearest multiple of 'multiple'.
int RoundToMultiple(int value, int multiple)
{
    int half = multiple / 2;
    int retval = (value < 0) ? (value - half) : (value + half);

    return (retval) / multiple * multiple;
}

// Hack: manually fix the gaps.
// (please tell me if you know a better way to fix there gaps, without any bumps!)
void FixTrackPiecePosition(VehicleInfo* target, uint32_t trackType, uint8_t slope)
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
        case TrackElemType::LeftCurvedLiftHill:
        case TrackElemType::RightCurvedLiftHill:
            target->Pitch = 0;
            break;
    }
}

extern "C"
{
    // Returns the amount of path nodes in the pathing route for the specified track type.
    EXPORT int GetTrackElementRouteSize(uint8_t trackVariant, int32_t typeAndDirection)
    {
        return gTrackVehicleInfo[trackVariant][typeAndDirection]->size;
    }

    // Returns the pathing route for the specified track element.
    EXPORT void GetTrackElementRoute(uint8_t trackVariant, int32_t typeAndDirection, VehicleInfo* nodes, int32_t arraySize)
    {
        const VehicleInfoList* list = gTrackVehicleInfo[trackVariant][typeAndDirection];

        std::memcpy(nodes, list->info, sizeof(VehicleInfo) * arraySize);

        const int32_t trackType = (typeAndDirection >> 2);
        const TrackDefinition definition = TrackMetaData::GetTrackElementDescriptor(trackType).Definition;

        FixTrackPiecePosition(&nodes[0], trackType, definition.vangle_start);
        FixTrackPiecePosition(&nodes[arraySize - 1], trackType, definition.vangle_end);
    }

    // (Temporary?) Returns the flags for the specified track type.
    EXPORT uint16_t GetTrackTypeFlags(int32_t trackType)
    {
        return TrackMetaData::GetTrackElementDescriptor(trackType).Flags;
    }

    // (Temporary?) Returns the track height offset for this ride index.
    EXPORT int8_t GetTrackHeightOffset(uint16_t rideIndex)
    {
        auto ride = GetRide(RideId::FromUnderlying(rideIndex));
        if (ride == nullptr)
        {
            dll_log("GetTrackHeightOffset: ride %i not found.", rideIndex);
            return 0;
        }

        return RideTypeDescriptors[ride->type].Heights.VehicleZOffset;
    }

    // (Temporary?) Returns the ride colours for this ride.
    EXPORT void GetRideTrackColours(uint16_t rideIndex, TrackColour* colours)
    {
        auto ride = GetRide(RideId::FromUnderlying(rideIndex));
        if (ride == nullptr)
        {
            dll_log("GetRideTrackColours: ride %i not found.", rideIndex);
            return;
        }

        std::memcpy(colours, ride->track_colour, sizeof(TrackColour) * Limits::NumColourSchemes);
    }
}
