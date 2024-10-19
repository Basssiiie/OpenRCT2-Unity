using OpenRCT2.Bindings.Entities;
using OpenRCT2.Generators;
using OpenRCT2.Generators.Tracks;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours.Controllers
{
    /// <summary>
    /// Controller which moves and updates all ride vehicles in the park.
    /// </summary>
    public class VehicleController : EntityController<VehicleEntity>
    {
        readonly GameObject _prefab;

        /// <inheritdoc/>
        public VehicleController(Transform parent, GameObject prefab)
            : base(EntityType.Vehicle, parent)
        {
            _prefab = prefab;
        }

        /// <inheritdoc/>
        protected override int UpdateEntities(VehicleEntity[] entities)
            => EntityRegistry.GetAllVehicles(entities);

        /// <inheritdoc/>
        protected override bool IsActive(in VehicleEntity entity)
        {
            return entity.x > 0;
        }

        /// <inheritdoc/>
        protected override GameObject CreateEntity(int index)
        {
            var obj = Object.Instantiate(_prefab);
            obj.name = $"Vehicle {index}";
            return obj;
        }

        /// <inheritdoc/>
        protected override void UpdateEntity(int index, in VehicleEntity entity, GameObject gameObject)
        {
            var position = World.CoordsToVector3(entity.x, entity.z, entity.y);
            var rotation = TrackDataFactory
                .GetTrackPiece(entity.trackType)
                .GetVehicleRotation(entity.trackDirection, entity.trackProgress);

            gameObject.transform.SetLocalPositionAndRotation(position, rotation);
        }
    }
}
