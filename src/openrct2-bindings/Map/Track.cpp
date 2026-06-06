#include "../OpenRCT2.Bindings.h"
#include "../Utilities/Logging.h"
#include "../Utilities/TileElementHelper.h"

#include <cstdint>
#include <openrct2/core/EnumUtils.hpp>
#include <openrct2/drawing/Colour.h>
#include <openrct2/ride/Ride.h>
#include <openrct2/ride/RideColour.h>
#include <openrct2/ride/RideData.h>
#include <openrct2/ride/ted/PitchAndRoll.h>
#include <openrct2/ride/ted/TrackElementDescriptor.h>
#include <openrct2/ride/ted/TrackElemType.h>
#include <openrct2/ride/TrackData.h>
#include <openrct2/ride/Vehicle.h>
#include <openrct2/ride/VehicleSubpositionData.h>
#include <openrct2/world/Location.hpp>
#include <openrct2/world/Map.h>
#include <openrct2/world/tile_element/TileElement.h>
#include <openrct2/world/tile_element/TileElementType.h>
#include <openrct2/world/tile_element/TrackElement.h>
#include <string.h>

using namespace OpenRCT2::Drawing;
using namespace OpenRCT2::TrackMetadata;

extern "C"
{
    struct TrackInfo
    {
        TrackElemType trackType;
        int8_t trackHeight;
        uint8_t sequenceIndex;
        Colour mainColour;
        Colour additionalColour;
        Colour supportsColour;
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
        TrackElemType trackType = track->GetTrackType();

        target->trackType = trackType;
        target->trackHeight = rtd.Heights.VehicleZOffset;
        target->sequenceIndex = track->GetSequenceIndex();
        target->chainlift = track->HasChain();
        target->cablelift = track->HasCableLift();
        target->inverted = track->IsInverted();

        const TrackColour scheme = ride->trackColours[track->GetColourScheme()];
        target->mainColour = scheme.main;
        target->additionalColour = scheme.additional;
        target->supportsColour = scheme.supports;

        const TrackElementDescriptor& ted = GetTrackElementDescriptor(track->GetTrackType());
        target->normalToInverted = (ted.flags.has(TrackElementFlag::normalToInversion));
        target->invertedToNormal = (ted.flags.has(TrackElementFlag::inversionToNormal));
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
    static void FixTrackPiecePosition(TrackSubposition* target, TrackElemType trackType, TrackPitch slope)
    {
        switch (slope)
        {
            case TrackPitch::up90:
            case TrackPitch::down90:
                target->z = RoundToMultiple(target->z, 8);
                break;

            default:
                target->x = RoundToMultiple(target->x, 16);
                target->y = RoundToMultiple(target->y, 16);
                target->z = RoundToMultiple(target->z, 8);
                break;
        }

        // Custom hacks for specific track types.
        if (trackType == TrackElemType::leftCurvedLiftHill || trackType == TrackElemType::rightCurvedLiftHill)
        {
            target->pitch = 0;
        }
    }

    // Returns the upper bound of the track types array.
    EXPORT int32_t GetTrackTypesCount()
    {
        return static_cast<int32_t>(TrackElemType::count);
    }

    // Returns the length of the pathing route for the specified track element.
    EXPORT uint16_t GetTrackSubpositionsLength(VehicleTrackSubposition subposition, TrackElemType trackType, uint8_t direction)
    {
        return VehicleGetMoveInfoSize(subposition, trackType, direction);
    }

    // Returns the pathing route for the specified track element.
    EXPORT void GetTrackSubpositions(
        VehicleTrackSubposition subposition, TrackElemType trackType, uint8_t direction, TrackSubposition* nodes,
        int32_t arraySize)
    {
        static_assert(sizeof(TrackSubposition) == sizeof(VehicleInfo), "Size is not correct");

        auto typeAndDirection = static_cast<uint16_t>((EnumValue(trackType) << 2) | (direction & 3));
        auto trackSubposition = static_cast<uint8_t>(subposition);

        const VehicleInfoList* list = gTrackVehicleInfo[trackSubposition][typeAndDirection];
        std::memcpy(nodes, list->info, sizeof(VehicleInfo) * arraySize);

        const TrackDefinition definition = TrackMetadata::GetTrackElementDescriptor(trackType).definition;
        FixTrackPiecePosition(&nodes[0], trackType, definition.pitchStart);
        FixTrackPiecePosition(&nodes[arraySize - 1], trackType, definition.pitchEnd);
    }
}
