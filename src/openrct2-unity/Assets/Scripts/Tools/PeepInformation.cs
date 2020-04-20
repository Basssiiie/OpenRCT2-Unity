using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeepInformation : MonoBehaviour
{
    private string name;
    void OnMouseDown()
    {
        Debug.Log("Clicked: " + name);
    }

    public void UpdateInformation(OpenRCT2.Unity.Peep peep)
    {
        name = $"{peep.type} {peep.Id}";
        /*
        var intensityBinary = System.Convert.ToString(peep.intensity, 2);
        var maxIntensityString = System.Convert.ToString(peep.intensity >> 4);
        var minIntensityString = System.Convert.ToString(peep.intensity & 0b1111);

        minIntensity = $"{minIntensityString}";
        maxIntensity = $"{maxIntensityString}";
        */
    }
}
