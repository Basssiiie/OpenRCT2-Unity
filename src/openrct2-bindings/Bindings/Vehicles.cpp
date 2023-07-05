#include "../openrct2-bindings.h"

#include <openrct2/entity/EntityList.h>
#include <openrct2/ride/Vehicle.h>

extern "C"
{
    struct VehicleEntity
    {
    public:
        uint16_t idx;
        int32_t x;
        int32_t y;
        int32_t z;
        uint8_t direction;
        uint8_t bankRotation;
        uint8_t pitchRotation;
        uint8_t trackType;
        uint8_t trackDirection;
        uint16_t trackProgress;
    };

    // Loads all the vehicles into the specified buffer, returns the total amount of vehicles loaded.
    EXPORT int32_t GetAllVehicles(VehicleEntity* vehicles, int32_t arraySize)
    {
        int vehicleCount = 0;

        for (Vehicle* vehicle : EntityList<Vehicle>())
        {
            VehicleEntity* target = &vehicles[vehicleCount];
            target->idx = vehicle->Id.ToUnderlying();

            target->x = vehicle->x;
            target->y = vehicle->y;
            target->z = vehicle->z;

            target->direction = vehicle->Orientation;
            target->bankRotation = vehicle->bank_rotation;
            target->pitchRotation = vehicle->Pitch;

            target->trackType = vehicle->GetTrackType();
            target->trackDirection = vehicle->GetTrackDirection();
            target->trackProgress = vehicle->track_progress;

            vehicleCount++;

            if (vehicleCount >= arraySize)
                break;
        }
        return vehicleCount;
    }
}
