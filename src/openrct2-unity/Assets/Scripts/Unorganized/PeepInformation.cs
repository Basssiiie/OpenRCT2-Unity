using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeepInformation : MonoBehaviour
{
    /* 
     public int value; // will show
     [SerializeField] private int serPrivValue; // will show
     [SerializeField] private int serProtValue; // will show      
     private int privValue; // will not show
     protected int protValue; // will not show
    [Header("Button Settings")]
        [Tooltip("Arbitary text message")]
  */



    public string type;
    public string state;
    public string insideThePark;
    public string subState;
    public string ridesDone;
    public string TshirtColour;
    public string TrouserColour;
    public string TryingToGetTo;
    public string DestinationTolerance;
    public string intensity;
    public string nauseaTolerance;

    [Header("Peep Needs")]
    [Range(0, 256)] public int energy;
    [Range(0, 256)] public int happiness;
    [Range(0, 256)] public int hunger;
    [Range(0, 256)] public int thirst;
    [Range(0, 256)] public int toilet;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    Color decodeColour(byte colour)
    {
        var colourRGB = new Color32(0, 0, 0, 1);
        switch (colour)
        {
            case 0:
                colourRGB = new Color32(0, 0, 0, 1);
                break;
            case 1:
                colourRGB = new Color32(128, 128, 128, 1);
                break;
            case 2:
                colourRGB = new Color32(255, 255, 255, 1);
                break;
            case 3:
                colourRGB = new Color32(85, 26, 139, 1);
                break;
            case 4:
                colourRGB = new Color32(171, 130, 255, 1);
                break;
            case 5:
                colourRGB = new Color32(160, 32, 240, 1);
                break;
            case 6:
                colourRGB = new Color32(0, 0, 139, 1);
                break;
            case 7:
                colourRGB = new Color32(102, 102, 255, 1);
                break;
            case 8:
                colourRGB = new Color32(135, 206, 235, 1);
                break;
            case 9:
                colourRGB = new Color32(0, 128, 128, 1);
                break;
            case 10:
                colourRGB = new Color32(127, 255, 212, 1);
                break;
            case 11:
                colourRGB = new Color32(124, 205, 124, 1);
                break;
            case 12:
                colourRGB = new Color32(0, 100, 0, 1);
                break;
            case 13:
                colourRGB = new Color32(110, 139, 61, 1);
                break;
            case 14:
                colourRGB = new Color32(0, 255, 0, 1);
                break;
            case 15:
                colourRGB = new Color32(192, 255, 62, 1);
                break;
            case 16:
                colourRGB = new Color32(85, 107, 47, 1);
                break;
            case 17:
                colourRGB = new Color32(255, 255, 0, 1);
                break;
            case 18:
                colourRGB = new Color32(139, 139, 0, 1);
                break;
            case 19:
                colourRGB = new Color32(255, 165, 0, 1);
                break;
            case 20:
                colourRGB = new Color32(255, 127, 0, 1);
                break;
            case 21:
                colourRGB = new Color32(244, 164, 96, 1);
                break;
            case 22:
                colourRGB = new Color32(165, 42, 42, 1);
                break;
            case 23:
                colourRGB = new Color32(205, 170, 125, 1);
                break;
            case 24:
                colourRGB = new Color32(61, 51, 37, 1);
                break;
            case 25:
                colourRGB = new Color32(255, 160, 122, 1);
                break;
            case 26:
                colourRGB = new Color32(205, 55, 0, 1);
                break;
            case 27:
                colourRGB = new Color32(200, 0, 0, 1);
                break;
            case 28:
                colourRGB = new Color32(255, 0, 0, 1);
                break;
            case 29:
                colourRGB = new Color32(205, 16, 118, 1);
                break;
            case 30:
                colourRGB = new Color32(255, 105, 180, 1);
                break;
            case 31:
                colourRGB = new Color32(255, 174, 185, 1);
                break;
            default:
                break;
        }
        return colourRGB;
    }

    public void UpdateColours(byte tshirtColour, byte trousersColour)
    {

        GameObject tshirt = gameObject.transform.Find("Tshirt").gameObject;
        GameObject trousers = gameObject.transform.Find("Trousers").gameObject;

        var tshirtRenderer = tshirt.GetComponent<Renderer>();
        var trousersRenderer = trousers.GetComponent<Renderer>();

        tshirtRenderer.material.color = decodeColour(tshirtColour);
        trousersRenderer.material.color = decodeColour(trousersColour);

    }

    public void UpdateInformation(OpenRCT2.Unity.Peep peep)
    {
        type = $"{peep.type}";
        state = $"{peep.state}";
        insideThePark = $"{(peep.outsideOfPark == 0 ? "Yes" : "No")}";
        subState = $"{peep.substate}";
        ridesDone = $"{peep.staffTypeOrNoOfRides}";
        TshirtColour = $"{peep.tshirtColour}";
        TrouserColour = $"{peep.trousersColour}";
        TryingToGetTo = $"{(peep.destinationX + peep.destinationY)}";
        DestinationTolerance = $"{peep.destinationTolerance}";

        energy = peep.energy;
        happiness = peep.happiness;
        hunger = peep.hunger;
        thirst = peep.thirst;
        toilet = peep.toilet;

        intensity = $"{peep.intensity}";
        nauseaTolerance = $"{peep.nauseaTolerance}";

    }
}
