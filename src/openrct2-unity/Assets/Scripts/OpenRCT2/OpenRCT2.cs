using UnityEngine;

namespace OpenRCT2.Unity
{
    public partial class OpenRCT2 : MonoBehaviour
    {
        [SerializeField] string park = "My test park with burgers.sv6";
        [SerializeField] string rct2Path;
        [SerializeField] string rct1Path;


        // OpenRCT2 takes the executing directory by default; which is where the Unity Editor is installed here...
        const string rootpath = @"C:\Users\Rolf\Projects\03 - OpenRCT2 Unity";
        const string datapath = rootpath + @"\bin\data";
        const string parkpath = rootpath + @"\src\openrct2-unity\parks";


        void Awake()
        {
            Print("Start OpenRCT2...");

            StartGame(datapath, rct2Path, rct1Path);
            LoadPark($@"{parkpath}\{park}");

            string parkname = GetParkName();
            Print($"Name: {parkname} (pointer: {parkname})");

            Print("OpenRCT2 started.");
        }


        void FixedUpdate()
        {
            PerformGameUpdate();
        }


        void OnDestroy()
        {
            StopGame();
            Print("OpenRCT2 has shutdown.");
        }
    }
}
