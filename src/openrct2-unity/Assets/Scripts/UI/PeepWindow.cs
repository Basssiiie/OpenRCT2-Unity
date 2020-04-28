using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenRCT2.Unity;

public class PeepWindow : MonoBehaviour
{
    [SerializeField] Text title;
    [SerializeField] Slider happinessBar;
    [SerializeField] Slider energyBar;
    [SerializeField] Slider hungerBar;
    [SerializeField] Slider thirstBar;
    [SerializeField] Slider nauseaBar;
    [SerializeField] Slider toiletBar;
    ushort peepId;
    PeepController peepControllerScript;

    public void LoadPeepController(PeepController peepController, ushort id)
    {
        peepId = id;
        peepControllerScript = peepController;
        title.text = $"Guest {peepId}";
        InvokeRepeating("UpdateData", 0f, 5f);
    }

    void UpdateData()
    {
        Peep? peep = peepControllerScript.GetPeepById(peepId);
        if (peep == null)
        {
            Destroy(gameObject);
        }
        var value = peep.Value;

        happinessBar.value = value.happiness;
        energyBar.value = value.energy;
        hungerBar.value = value.hunger;
        thirstBar.value = value.thirst;
        nauseaBar.value = value.nausea;
        toiletBar.value = value.toilet;

        /*
        var intensityBinary = System.Convert.ToString(peep.intensity, 2);
        var maxIntensityString = System.Convert.ToString(peep.intensity >> 4);
        var minIntensityString = System.Convert.ToString(peep.intensity & 0b1111);
        */
    }
}
