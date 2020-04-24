using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PeepInformation : MonoBehaviour
{
    string name;
    int happiness;
    int energy;
    int hunger;
    int thirst;
    int nausea;
    int toilet;
    [SerializeField] private GameObject peepBox;
    GameObject PeepBox;


    void OnMouseDown()
    {
        if (!PeepBox)
        {
            PeepBox = Instantiate(peepBox, FindObjectOfType<Canvas>().transform);
            PeepBox.name = $"PeepBox: {name}";
            var titleText = PeepBox.transform.GetChild(0).transform.GetChild(0);
            titleText.GetComponent<Text>().text = name;
            var needValues = PeepBox.transform.GetChild(1).transform.GetChild(1).transform.GetChild(1).transform.GetChild(1);
            var happinessBar = needValues.transform.GetChild(0);
            var energyBar = needValues.transform.GetChild(1);
            var hungerBar = needValues.transform.GetChild(2);
            var thirstBar = needValues.transform.GetChild(3);
            var nauseaBar = needValues.transform.GetChild(4);
            var toiletBar = needValues.transform.GetChild(5);

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
