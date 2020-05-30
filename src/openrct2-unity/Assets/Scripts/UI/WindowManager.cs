using Sprites;
using UnityEngine;


namespace UI
{
    public class WindowManager : MonoBehaviour
    {
        [SerializeField] GameObject peepBox;
        [SerializeField] GameObject peepCanvas;
        [SerializeField] PeepController peepController;


        public void CreatePeepWindow(ushort id)
        {
            GameObject obj = Instantiate(peepBox, peepCanvas.transform);
            obj.name = $"PeepBox: {id}";
            obj.GetComponent<PeepWindow>().LoadPeepController(peepController, id);
        }
    }
}
