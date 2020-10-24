using Sprites;
using UnityEngine;


namespace UI
{
    public class WindowManager : MonoBehaviour
    {
        [SerializeField] GameObject peepWindowPrefab;
        [SerializeField] GameObject parentCanvas;


        public void CreatePeepWindow(ushort peepId)
        {
            GameObject obj = Instantiate(peepWindowPrefab, parentCanvas.transform);
            obj.name = $"PeepWindow: {peepId}";

            var window = obj.GetComponent<PeepWindow>();
            window.SetPeep(peepId);
        }
    }
}
