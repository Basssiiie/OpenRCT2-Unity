using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenRCT2.Unity;

public class CanvasManager : MonoBehaviour
{

    [SerializeField] GameObject peepBox;
    [SerializeField] GameObject peepCanvas;
    [SerializeField] PeepController peepController;

    public void CreatePeepWindow(ushort id)
    {
        GameObject obj = Instantiate(peepBox, peepCanvas.transform);
        obj.name = $"PeepBox: {id}";
        obj.GetComponent<PeepWindow>().LoadPeepController(peepController, id);
    }
}
