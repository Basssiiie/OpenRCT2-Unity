using OpenRCT2.Bindings.Entities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours.Controllers
{
    /// <summary>
    /// Controller which moves and updates all the guests in the park.
    /// </summary>
    public class GuestController : PeepController
    {
        /// <inheritdoc/>
        public GuestController(Transform transform, GameObject prefab)
            : base(EntityType.Guest, transform, prefab)
        {
        }

        /// <inheritdoc/>
        protected override int UpdateEntities(PeepEntity[] entities)
            => EntityRegistry.GetAllGuests(entities);
    }
}
