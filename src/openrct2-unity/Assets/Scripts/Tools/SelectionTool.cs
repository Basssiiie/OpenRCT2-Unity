using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionTool : MonoBehaviour
{

    [SerializeField] private GameObject canvasManager;
    CanvasManager canvasScript;


    void Start()
    {
        canvasScript = canvasManager.gameObject.GetComponent<CanvasManager>();
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
                CheckHit(hit.collider.gameObject.name);
            }
        }
    }

    void CheckHit(string name)
    {
        switch (name)
        {
            case string a when a.Contains("Guest"):
                canvasScript.RenderUIElement("RENDER_PEEP_BOX");
                break;
            default:
                break;
        }
    }

}
