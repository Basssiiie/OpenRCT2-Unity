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
        [Range(0, 256)]public int energy;
        [Range(0, 256)]public int happiness;
        [Range(0, 256)]public int hunger;
        [Range(0, 256)]public int thirst;
        [Range(0, 256)]public int toilet;

        
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateInformation (OpenRCT2.Unity.Peep peep) {
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
