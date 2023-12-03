#include "../OpenRCT2.Bindings.h"
#include "../Utilities/TileElementHelper.h"

#include <openrct2/ride/Ride.h>
#include <openrct2/ride/RideData.h>
#include <openrct2/ride/TrackData.h>

using namespace OpenRCT2::TrackMetaData;

extern "C"
{
    struct TrackInfo
    {
        uint16_t trackType;
        int8_t trackHeight;
        uint8_t sequenceIndex;
        uint8_t mainColour;
        uint8_t additionalColour;
        uint8_t supportsColour;
        bool chainlift;
        bool cablelift;
        bool inverted;
        bool normalToInverted;
        bool invertedToNormal;
    };

    // Writes the track element details to the specified buffer.
    EXPORT void GetTrackElementAt(int x, int y, int index, TrackInfo* element)
    {
        const TileElement* source = GetTileElementAt(x, y, index, TileElementType::Track);
        const TrackElement* track = source->AsTrack();
        const Ride* ride = GetRide(track->GetRideIndex());
        const RideTypeDescriptor& rtd = GetRideTypeDescriptor(track->GetRideType());
        uint16_t trackType = track->GetTrackType();

        element->trackType = trackType;
        element->trackHeight = rtd.Heights.VehicleZOffset;
        element->sequenceIndex = track->GetSequenceIndex();
        element->chainlift = track->HasChain();
        element->cablelift = track->HasCableLift();
        element->inverted = track->IsInverted();

        const TrackColour scheme = ride->track_colour[track->GetColourScheme()];
        element->mainColour = scheme.main;
        element->additionalColour = scheme.additional;
        element->supportsColour = scheme.supports;

        const TrackElementDescriptor& ted = GetTrackElementDescriptor(track->GetTrackType());
        element->normalToInverted = (ted.Flags & TRACK_ELEM_FLAG_NORMAL_TO_INVERSION);
        element->invertedToNormal = (ted.Flags & TRACK_ELEM_FLAG_INVERSION_TO_NORMAL);
    }

    struct TrackSubposition
    {
        int16_t x;
        int16_t y;
        int16_t z;
        uint8_t direction;
        uint8_t pitch;
        uint8_t banking;
    };

    // Rounds a number to the nearest multiple of 'multiple'.
    static int16_t RoundToMultiple(int value, int multiple)
    {
        int half = multiple / 2;
        int retval = (value < 0) ? (value - half) : (value + half);

        return static_cast<int16_t>((retval / multiple) * multiple);
    }

    // Hack: manually fix the gaps.
    // (please tell me if you know a better way to fix there gaps, without any bumps!)
    static void FixTrackPiecePosition(TrackSubposition* target, uint32_t trackType, uint8_t slope)
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
        if (trackType == TrackElemType::LeftCurvedLiftHill || trackType == TrackElemType::RightCurvedLiftHill)
        {
            target->pitch = 0;
        }
    }

    // Returns the upper bound of the track types array.
    EXPORT int32_t GetTrackTypesCount()
    {
        return static_cast<int32_t>(TrackElemType::Count);
    }

    // Returns the length of the pathing route for the specified track element.
    EXPORT uint16_t GetTrackSubpositionsLength(VehicleTrackSubposition subposition, uint16_t trackType, uint8_t direction)
    {
        return VehicleGetMoveInfoSize(subposition, trackType, direction);
    }

    // Returns the pathing route for the specified track element.
    EXPORT void GetTrackSubpositions(
        VehicleTrackSubposition subposition, uint16_t trackType, uint8_t direction, TrackSubposition* nodes, int32_t arraySize)
    {
        static_assert(sizeof(TrackSubposition) == sizeof(VehicleInfo), "Size is not correct");

        auto typeAndDirection = static_cast<uint16_t>((trackType << 2) | (direction & 3));
        auto trackSubposition = static_cast<uint8_t>(subposition);

        const VehicleInfoList* list = gTrackVehicleInfo[trackSubposition][typeAndDirection];
        std::memcpy(nodes, list->info, sizeof(VehicleInfo) * arraySize);

        const TrackDefinition definition = TrackMetaData::GetTrackElementDescriptor(trackType).Definition;
        FixTrackPiecePosition(&nodes[0], trackType, definition.vangle_start);
        FixTrackPiecePosition(&nodes[arraySize - 1], trackType, definition.vangle_end);
    }
}
