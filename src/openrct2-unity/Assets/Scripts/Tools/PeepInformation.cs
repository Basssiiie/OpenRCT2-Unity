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

    [Header("Peep General Information")]
    public string name;
    public string type;
    public string insideThePark;

    [Header("Peep Activity")]
    public string state;
    public string subState;
    public string ridesDone;
    public string TryingToGetTo;
    public string DestinationTolerance;
    public string intensity;
    public string nauseaTolerance;
    public string actionSprite;

    [Header("Peep Needs")]
    [Range(0, 256)] public int energy;
    [Range(0, 256)] public int happiness;
    [Range(0, 256)] public int hunger;
    [Range(0, 256)] public int thirst;
    [Range(0, 256)] public int toilet;


    public void UpdateInformation(OpenRCT2.Unity.Peep peep)
    {
        name = $"{(peep.Name != null ? peep.Name : "null")}";
        type = $"{peep.type}";
        state = $"{peep.state}";
        insideThePark = $"{(peep.outsideOfPark == 0 ? "Yes" : "No")}";
        subState = $"{peep.substate}";
        ridesDone = $"{peep.staffTypeOrNoOfRides}";
        TryingToGetTo = $"{(peep.destinationX + peep.destinationY)}";
        DestinationTolerance = $"{peep.destinationTolerance}";

        energy = peep.energy;
        happiness = peep.happiness;
        hunger = peep.hunger;
        thirst = peep.thirst;
        toilet = peep.toilet;

        intensity = $"{peep.intensity}";
        nauseaTolerance = $"{peep.nauseaTolerance}";
        actionSprite = $"{peep.actionSpriteType}";

    }
}
