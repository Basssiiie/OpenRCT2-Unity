using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

namespace UI
{
    public class WindowManager : MonoBehaviour
    {
        [SerializeField] GameObject? _peepWindowPrefab;
        [SerializeField] GameObject? _parentCanvas;


        public void CreatePeepWindow(ushort peepId)
        {
            Assert.IsNotNull(_peepWindowPrefab, nameof(_peepWindowPrefab));
            Assert.IsNotNull(_parentCanvas, nameof(_parentCanvas));

            GameObject obj = Instantiate(_peepWindowPrefab, _parentCanvas.transform);
            obj.name = $"PeepWindow: {peepId}";

            var window = obj.GetComponent<PeepWindow>();
            window.SetPeep(peepId);
        }
    }
}
