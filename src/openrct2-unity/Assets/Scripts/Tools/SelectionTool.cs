using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using OpenRCT2.Unity;

public class SelectionTool : MonoBehaviour
{

    [SerializeField] private CanvasManager canvasManager;
    [SerializeField] private PeepController peepController;


    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                CheckHit(hit.collider.gameObject);
            }
        }
    }

    void CheckHit(GameObject obj)
    {
        switch (obj.name)
        {
            case string a when a.Contains("Guest"):
                dynamic args = new System.Dynamic.ExpandoObject();
                args.id = peepController.FindPeepIdForGameObject(obj);
                canvasManager.RenderUIElement("RENDER_PEEP_BOX", args);
                break;
            default:
                break;
        }
    }

}
