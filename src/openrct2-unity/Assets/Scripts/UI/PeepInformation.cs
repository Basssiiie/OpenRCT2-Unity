using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PeepInformation : MonoBehaviour
{
    private string name;
    [SerializeField] private Text nameLabel;
    [SerializeField] private GameObject referenceObject;
    private Text UI;
    private bool active;
    private bool destroyed;

    void OnMouseOver()
    {
        if (!active) {
            UI = Instantiate(nameLabel, FindObjectOfType<Canvas>().transform);
            UI.name = $"HoverLabel: {name}";
            UI.GetComponent<Text>().text = name;
            active = true;
            destroyed = false;
        }
    }

    void OnMouseExit()
    {
        if (active && !destroyed) {
            StartCoroutine("Fade");
            destroyed = true;
        }
    }

    IEnumerator Fade() {
        if (!destroyed) {
            yield return new WaitForSeconds(1f);
            Destroy(UI.gameObject);
            active = false;
        }
      
        
    }

    void Update()
    {
        if (UI) {
            Vector3 namePose = Camera.main.WorldToScreenPoint(referenceObject.transform.position);
            UI.transform.position = namePose; 
        }
 
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
