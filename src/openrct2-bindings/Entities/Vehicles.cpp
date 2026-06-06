#include "../OpenRCT2.Bindings.h"

#include <cstdint>
#include <openrct2/entity/EntityList.h>
#include <openrct2/ride/Angles.h>
#include <openrct2/ride/ted/TrackElemType.h>
#include <openrct2/ride/Vehicle.h>

extern "C"
{
    struct VehicleEntity
    {
        int32_t x;
        int32_t y;
        int32_t z;
        uint8_t direction;
        VehicleRoll banking;
        VehiclePitch pitch;
        TrackElemType trackType;
        uint8_t trackDirection;
        uint16_t trackProgress;
    };

    // Loads all the vehicles into the specified buffer, returns the total amount of vehicles loaded.
    EXPORT int32_t GetAllVehicles(VehicleEntity* vehicles, int32_t length)
    {
        int vehicleCount = 0;

        for (const Vehicle* vehicle : EntityList<Vehicle>())
        {
            VehicleEntity* target = &vehicles[vehicleCount];

            target->x = vehicle->x;
            target->y = vehicle->y;
            target->z = vehicle->z;

            target->direction = vehicle->Orientation;
            target->banking = vehicle->roll;
            target->pitch = vehicle->pitch;

            target->trackType = vehicle->GetTrackType();
            target->trackDirection = vehicle->GetTrackDirection();
            target->trackProgress = vehicle->track_progress;

            vehicleCount++;

            if (vehicleCount >= length)
                break;
        }

        return vehicleCount;
    }
}
