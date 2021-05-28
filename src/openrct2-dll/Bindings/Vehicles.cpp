#include <openrct2/ride/Vehicle.h>
#include <openrct2/world/EntityList.h>
#include <openrct2/world/Sprite.h>

#include "OpenRCT2-DLL.h"


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

        for (Vehicle* vehicle : EntityList<Vehicle>(EntityListId::Vehicle))
        {
            VehicleEntity* target = &vehicles[vehicleCount];
            target->idx = vehicle->sprite_index;

            target->x = vehicle->x;
            target->y = vehicle->y;
            target->z = vehicle->z;

            target->direction = vehicle->sprite_direction;
            target->bankRotation = vehicle->bank_rotation;
            target->pitchRotation = vehicle->vehicle_sprite_type;

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
