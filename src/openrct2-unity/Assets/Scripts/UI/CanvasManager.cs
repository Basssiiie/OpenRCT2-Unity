using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenRCT2.Unity;

public class CanvasManager : MonoBehaviour
{

    [SerializeField] private GameObject peepBox;
    [SerializeField] private GameObject peepCanvas;
    [SerializeField] private PeepController peepController;

    public void RenderUIElement(string action, dynamic args)
    {
        switch (action)
        {
            case "RENDER_PEEP_BOX":
                RenderPeepBox(args.id);
                break;
        }
    }

    void RenderPeepBox(ushort id)
    {
        GameObject obj = Instantiate(peepBox, peepCanvas.transform);
        obj.name = $"PeepBox: {id}";
        obj.gameObject.GetComponent<PeepWindow>().loadPeepController(peepController, id);
    }
}
