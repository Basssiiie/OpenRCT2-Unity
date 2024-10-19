using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours.Controllers
{
    /// <summary>
    /// Controller which moves and updates all ride vehicles in the park.
    /// </summary>
    public class VehicleControllerScript : MonoBehaviour
    {
        [SerializeField, Required] GameObject _prefab = null!;

        VehicleController? _controller;

        void Start()
        {
            _controller = new VehicleController(transform, _prefab);
        }

        void Update()
        {
            _controller?.Update();
        }

        void OnDestroy()
        {
            _controller = null;
        }
    }
}
