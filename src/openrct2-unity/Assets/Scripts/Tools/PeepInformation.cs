using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PeepInformation : MonoBehaviour
{
    private string name;
    public Text nameLabel;
    void OnMouseOver()
    {
        nameLabel.GetComponent<Text>().enabled = true;
    }

    void OnMouseExit()
    {
        nameLabel.GetComponent<Text>().enabled = false;
    }

    void Update()
    {
        Vector3 namePose = Camera.main.WorldToScreenPoint(gameObject.transform.Find("Sphere").transform.position);
        nameLabel.transform.position = namePose;
    }

    public void UpdateInformation(OpenRCT2.Unity.Peep peep)
    {
        name = $"{peep.type} {peep.Id}";
        gameObject.transform.Find("Canvas").Find("Text").GetComponent<UnityEngine.UI.Text>().text = name;
        /*
        var intensityBinary = System.Convert.ToString(peep.intensity, 2);
        var maxIntensityString = System.Convert.ToString(peep.intensity >> 4);
        var minIntensityString = System.Convert.ToString(peep.intensity & 0b1111);

        minIntensity = $"{minIntensityString}";
        maxIntensity = $"{maxIntensityString}";
        */
    }
}
