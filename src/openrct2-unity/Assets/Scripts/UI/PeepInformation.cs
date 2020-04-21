using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PeepInformation : MonoBehaviour
{
    private string name;
    private string hunger;
    [SerializeField] private GameObject peepBox;
    [SerializeField] private GameObject referenceObject;
    private GameObject PeepBox;


    void OnMouseDown()
    {
        if (!PeepBox)
        {
            PeepBox = Instantiate(peepBox, FindObjectOfType<Canvas>().transform);
            PeepBox.name = $"PeepBox: {name}";
            var titleText = PeepBox.transform.Find("Header").transform.Find("TitleText");
            titleText.GetComponent<Text>().text = name;
            var hungerText = PeepBox.transform.Find("Bottom").transform.Find("Page Area").transform.Find("Page 2 - Needs").transform.Find("Hunger");
            hungerText.GetComponent<Text>().text = $"Hunger: {hunger}";
        }

    }

    public void UpdateInformation(OpenRCT2.Unity.Peep peep)
    {
        name = $"{peep.type} {peep.Id}";
        hunger = $"{peep.hunger}";
        /*
        var intensityBinary = System.Convert.ToString(peep.intensity, 2);
        var maxIntensityString = System.Convert.ToString(peep.intensity >> 4);
        var minIntensityString = System.Convert.ToString(peep.intensity & 0b1111);

        minIntensity = $"{minIntensityString}";
        maxIntensity = $"{maxIntensityString}";
        */
    }
}
