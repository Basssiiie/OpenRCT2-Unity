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
        uint16_t trackLength;
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
        uint16_t typeAndDirection = static_cast<uint16_t>((trackType << 2) | (source->GetDirection() & 3));

        element->trackType = trackType;
        element->trackLength = gTrackVehicleInfo[0][typeAndDirection]->size;
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
    int16_t RoundToMultiple(int value, int multiple)
    {
        int half = multiple / 2;
        int retval = (value < 0) ? (value - half) : (value + half);

        return static_cast<int16_t>((retval / multiple) * multiple);
    }

    // Hack: manually fix the gaps.
    // (please tell me if you know a better way to fix there gaps, without any bumps!)
    void FixTrackPiecePosition(TrackSubposition* target, uint32_t trackType, uint8_t slope)
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
                target->pitch = 0;
                break;
        }
    }

    // Returns the pathing route for the specified track element.
    EXPORT void GetTrackSubpositions(uint16_t trackType, uint8_t direction, TrackSubposition* nodes, int32_t arraySize)
    {
        static_assert(sizeof(TrackSubposition) == sizeof(VehicleInfo), "Size is not correct");

        uint16_t typeAndDirection = static_cast<uint16_t>((trackType << 2) | (direction & 3));
        const VehicleInfoList* list = gTrackVehicleInfo[0][typeAndDirection];

        std::memcpy(nodes, list->info, sizeof(VehicleInfo) * arraySize);

        const TrackDefinition definition = TrackMetaData::GetTrackElementDescriptor(trackType).Definition;
        FixTrackPiecePosition(&nodes[0], trackType, definition.vangle_start);
        FixTrackPiecePosition(&nodes[arraySize - 1], trackType, definition.vangle_end);
    }
}
