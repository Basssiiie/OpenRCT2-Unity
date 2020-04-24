using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{

    [SerializeField] private GameObject peepBox;
    [SerializeField] private GameObject peepCanvas;

    public void RenderUIElement(string action)
    {
        switch (action)
        {
            case "RENDER_PEEP_BOX":
                RenderPeepBox();
                break;
        }
    }

    void RenderPeepBox()
    {
        Debug.Log("Rendered!");
        Instantiate(peepBox, peepCanvas.transform);
    }
}
