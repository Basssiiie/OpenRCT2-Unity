using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using OpenRCT2.Unity;

public class SelectionTool : MonoBehaviour
{

    [SerializeField] WindowManager canvasManager;
    [SerializeField] PeepController peepController;


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
