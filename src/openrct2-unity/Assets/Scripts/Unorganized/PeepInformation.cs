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
        public string name = "PLACEHOLDER";
        public string type = "PLACEHOLDER";
        public string state = "PLACEHOLDER";

        
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateInformation (OpenRCT2.Unity.Peep peep) {
        name = $"{peep.Name}";
        type = $"{peep.type}";
        state = $"{peep.state}";
    }
}
