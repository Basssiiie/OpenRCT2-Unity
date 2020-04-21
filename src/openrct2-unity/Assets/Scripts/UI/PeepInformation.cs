using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PeepInformation : MonoBehaviour
{
    private string name;
    private int happiness;
    private int energy;
    private int hunger;
    private int thirst;
    private int nausea;
    private int toilet;
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
            var needValues = PeepBox.transform.Find("Bottom").transform.Find("Page Area").transform.Find("Page 2 - Needs").transform.Find("NeedValues");
            var happinessBar = needValues.transform.Find("HappinessBar");
            var energyBar = needValues.transform.Find("EnergyBar");
            var hungerBar = needValues.transform.Find("HungerBar");
            var thirstBar = needValues.transform.Find("ThirstBar");
            var nauseaBar = needValues.transform.Find("NauseaBar");
            var toiletBar = needValues.transform.Find("ToiletBar");

            happinessBar.GetComponent<Slider>().value = happiness;
            energyBar.GetComponent<Slider>().value = energy;
            hungerBar.GetComponent<Slider>().value = hunger;
            thirstBar.GetComponent<Slider>().value = thirst;
            nauseaBar.GetComponent<Slider>().value = nausea;
            toiletBar.GetComponent<Slider>().value = toilet;

        }

    }

    public void UpdateInformation(OpenRCT2.Unity.Peep peep)
    {
        name = $"{peep.type} {peep.Id}";

        happiness = peep.happiness;
        energy = peep.energy;
        hunger = peep.hunger;
        thirst = peep.thirst;
        nausea = peep.nausea;
        toilet = peep.toilet;
        /*
        var intensityBinary = System.Convert.ToString(peep.intensity, 2);
        var maxIntensityString = System.Convert.ToString(peep.intensity >> 4);
        var minIntensityString = System.Convert.ToString(peep.intensity & 0b1111);

        minIntensity = $"{minIntensityString}";
        maxIntensity = $"{maxIntensityString}";
        */
    }
}
