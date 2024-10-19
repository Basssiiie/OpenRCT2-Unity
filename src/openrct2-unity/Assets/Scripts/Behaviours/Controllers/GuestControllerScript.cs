using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours.Controllers
{
    /// <summary>
    /// Controller which moves and updates all guests in the park.
    /// </summary>
    public class GuestControllerScript : MonoBehaviour
    {
        [SerializeField, Required] GameObject _prefab = null!;

        GuestController? _controller;


        void Start()
        {
            _controller = new GuestController(transform, _prefab);
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
