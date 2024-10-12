#include "../OpenRCT2.Bindings.h"
#include "../Utilities/Logging.h"
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

    static void SetTrackInfo(int x, int y, int index, const TileElement* source, TrackInfo* target)
    {
        const TrackElement* track = source->AsTrack();

        if (track == nullptr)
        {
            dll_log("Could not find track element at %i, %i, index %i", x, y, index);
            return;
        }

        const Ride* ride = GetRide(track->GetRideIndex());
        const RideTypeDescriptor& rtd = GetRideTypeDescriptor(track->GetRideType());
        uint16_t trackType = track->GetTrackType();

        target->trackType = trackType;
        target->trackHeight = rtd.Heights.VehicleZOffset;
        target->sequenceIndex = track->GetSequenceIndex();
        target->chainlift = track->HasChain();
        target->cablelift = track->HasCableLift();
        target->inverted = track->IsInverted();

        const TrackColour scheme = ride->track_colour[track->GetColourScheme()];
        target->mainColour = scheme.main;
        target->additionalColour = scheme.additional;
        target->supportsColour = scheme.supports;

        const TrackElementDescriptor& ted = GetTrackElementDescriptor(track->GetTrackType());
        target->normalToInverted = (ted.flags & TRACK_ELEM_FLAG_NORMAL_TO_INVERSION);
        target->invertedToNormal = (ted.flags & TRACK_ELEM_FLAG_INVERSION_TO_NORMAL);
    }

    // Writes the track element details to the specified buffer.
    EXPORT void GetTrackElementAt(int x, int y, int index, TrackInfo* element)
    {
        const TileElement* source = GetTileElementAt(x, y, index, TileElementType::Track);
        SetTrackInfo(x, y, index, source, element);
    }

    // Writes all the track element details to the specified buffer.
    EXPORT int GetAllTrackElementsAt(int x, int y, TrackInfo* elements, int length)
    {
        const TileElement* source = MapGetFirstElementAt(TileCoordsXY{ x, y });
        auto index = 0;

        do
        {
            if (source == nullptr)
                break;

            const TileElementType type = source->GetType();
            if (type != TileElementType::Track)
                continue;

            SetTrackInfo(x, y, index, source, elements);
            index++;
            elements++;

        } while (!(source++)->IsLastForTile() && index < length);

        return index;
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
    static void FixTrackPiecePosition(TrackSubposition* target, uint32_t trackType, TrackPitch slope)
    {
        switch (slope)
        {
            case TrackPitch::Up90:
            case TrackPitch::Down90:
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

        const TrackDefinition definition = TrackMetaData::GetTrackElementDescriptor(trackType).definition;
        FixTrackPiecePosition(&nodes[0], trackType, definition.pitchStart);
        FixTrackPiecePosition(&nodes[arraySize - 1], trackType, definition.pitchEnd);
    }
}
