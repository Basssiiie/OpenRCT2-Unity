using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours.Controllers
{
    /// <summary>
    /// Controller which moves and updates all guests and staff in the park.
    /// </summary>
    public class PeepControllerScript : MonoBehaviour
    {
        [SerializeField, Required] GameObject _prefab = null!;

        GuestController? _guests;
        StaffController? _staff;


        void Start()
        {
            _guests = new GuestController(transform, _prefab);
            _staff = new StaffController(transform, _prefab);
        }

        void Update()
        {
            _guests?.Update();
            _staff?.Update();
        }

        void OnDestroy()
        {
            _guests = null;
            _staff = null;
        }
    }
}
