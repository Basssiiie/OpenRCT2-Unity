using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenRCT2.Unity;

public class PeepWindow : MonoBehaviour
{
    [SerializeField] private Text title;
    [SerializeField] private Slider happinessBar;
    [SerializeField] private Slider energyBar;
    [SerializeField] private Slider hungerBar;
    [SerializeField] private Slider thirstBar;
    [SerializeField] private Slider nauseaBar;
    [SerializeField] private Slider toiletBar;
    ushort peepId;
    PeepController peepControllerScript;

    public void loadPeepController(PeepController peepController, ushort id)
    {
        peepId = id;
        peepControllerScript = peepController;
        title.text = $"Guest {peepId}";
        InvokeRepeating("UpdateData", 0f, 5f);
    }

    void UpdateData()
    {
        Peep? peep = peepControllerScript.GetPeepById(peepId);
        happinessBar.value = peep.Value.happiness;
        energyBar.value = peep.Value.energy;
        hungerBar.value = peep.Value.hunger;
        thirstBar.value = peep.Value.thirst;
        nauseaBar.value = peep.Value.nausea;
        toiletBar.value = peep.Value.toilet;
    }
}
