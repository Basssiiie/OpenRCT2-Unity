#nullable enable

using OpenRCT2.Bindings.Entities;
using OpenRCT2.Generators;
using OpenRCT2.Generators.Tracks;
using UnityEngine;

namespace OpenRCT2.Behaviours.Controllers
{
    /// <summary>
    /// Controller which moves and updates all ride vehicles in the park.
    /// </summary>
    public class VehicleController : SpriteController<Vehicle>
    {
        /// <summary>
        /// Reads all vehicles into the buffer from the dll hook.
        /// </summary>
        protected override int FillSpriteBuffer(Vehicle[] buffer)
            => EntityRegistry.GetAllVehicles(buffer);


        /// <summary>
        /// Returns an id of the vehicle, currently based on the sprite index.
        /// </summary>
        protected override ushort GetId(in Vehicle sprite)
            => sprite.id;


        /// <summary>
        /// Returns the vehicle's position in Unity coordinates.
        /// </summary>
        protected override Vector3 GetPosition(in Vehicle sprite)
            => World.CoordsToVector3(sprite.x, sprite.z, sprite.y);


        /// <summary>
        /// Updates the vehicle with the correct rotation.
        /// </summary>
        protected override SpriteObject UpdateSprite(int index, in Vehicle sprite)
        {
            var obj = base.UpdateSprite(index, sprite);

            obj.gameObject.transform.rotation = GetRotation(sprite);
            return obj;
        }


        /// <summary>
        /// Gets the vehicles rotation as a quaternion.
        /// </summary>
        Quaternion GetRotation(in Vehicle vehicle)
            => TrackFactory
                .GetTrackPiece(vehicle.trackType)
                .GetVehicleRotation(vehicle.trackDirection, vehicle.trackProgress);
    }
}
