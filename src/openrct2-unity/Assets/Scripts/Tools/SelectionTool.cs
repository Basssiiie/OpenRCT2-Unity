using Sprites;
using UI;
using UnityEngine;


namespace Tools
{
    public class SelectionTool : MonoBehaviour
    {
        [SerializeField] WindowManager windowManager;
        [SerializeField] PeepController peepController;


        Camera mainCamera;

        void Start()
        {
            mainCamera = Camera.main;
        }


        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

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
                    ushort peepId = peepController.FindPeepIdForGameObject(obj);
                    windowManager.CreatePeepWindow(peepId);
                    break;
            }
        }
    }
}
