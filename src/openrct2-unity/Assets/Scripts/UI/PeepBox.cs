using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PeepBox : MonoBehaviour
{
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject happinessBar;
    [SerializeField] private GameObject energyBar;
    [SerializeField] private GameObject hungerBar;
    [SerializeField] private GameObject thirstBar;
    [SerializeField] private GameObject nauseaBar;
    [SerializeField] private GameObject toiletBar;
    Text titleText;
    Slider happinessSlider;
    Slider energySlider;
    Slider hungerSlider;
    Slider thirstSlider;
    Slider nauseaSlider;
    Slider toiletSlider;

    void Start()
    {
        titleText = title.GetComponent<Text>();

        happinessSlider = happinessBar.GetComponent<Slider>();
        energySlider = energyBar.GetComponent<Slider>();
        hungerSlider = hungerBar.GetComponent<Slider>();
        thirstSlider = thirstBar.GetComponent<Slider>();
        nauseaSlider = nauseaBar.GetComponent<Slider>();
        toiletSlider = toiletBar.GetComponent<Slider>();
    }


    void Update()
    {

    }
}
