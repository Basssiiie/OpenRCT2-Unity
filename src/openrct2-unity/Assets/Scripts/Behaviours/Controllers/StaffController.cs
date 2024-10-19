using OpenRCT2.Bindings.Entities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours.Controllers
{
    /// <summary>
    /// Controller which moves and updates all the staff in the park.
    /// </summary>
    public class StaffController : PeepController
    {
        /// <inheritdoc/>
        public StaffController(Transform transform, GameObject prefab)
            : base(EntityType.Staff, transform, prefab)
        {
        }

        /// <inheritdoc/>
        protected override int UpdateEntities(PeepEntity[] entities)
            => EntityRegistry.GetAllStaff(entities);
    }
}
