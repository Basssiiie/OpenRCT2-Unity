using Sprites;
using UI;
using UnityEngine;


namespace Tools
{
    public class SelectionTool : MonoBehaviour
    {
        [SerializeField] WindowManager canvasManager;
        [SerializeField] PeepController peepController;


        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

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
                    canvasManager.CreatePeepWindow(peepController.FindPeepIdForGameObject(obj));
                    break;
                default:
                    break;
            }
        }
    }
}
