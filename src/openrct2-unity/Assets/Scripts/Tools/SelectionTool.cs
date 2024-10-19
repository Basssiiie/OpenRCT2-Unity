using OpenRCT2.Behaviours.Controllers;
using OpenRCT2.Utilities;
using UI;
using UnityEngine;

#nullable enable

namespace Tools
{
    public class SelectionTool : MonoBehaviour
    {
        [SerializeField] WindowManager? _windowManager;
        [SerializeField] PeepController? _peepController;


        Camera? _mainCamera;


        void Start()
        {
            _mainCamera = Camera.main;
        }


        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Assert.IsNotNull(_mainCamera, nameof(_mainCamera));

                Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    CheckHit(hit.collider.gameObject);
                }
            }
        }


        void CheckHit(GameObject obj)
        {
            switch (obj.tag)
            {
                case "Peep":
                    Assert.IsNotNull(_peepController, nameof(_peepController));
                    Assert.IsNotNull(_windowManager, nameof(_windowManager));

                    //ushort peepId = _peepController.FindPeepIdForGameObject(obj);
                    //_windowManager.CreatePeepWindow(peepId);
                    break;
            }
        }
    }
}
