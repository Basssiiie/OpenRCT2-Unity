using OpenRCT2.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable

public class DragWindow : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField] RectTransform? _dragRectTransform;
    [SerializeField] GameObject? _peepWindow;
    [SerializeField] Canvas? _canvas;


    public void Awake()
    {
        if (_dragRectTransform == null)
        {
            _dragRectTransform = transform.parent.GetComponent<RectTransform>();
        }

        if (_canvas == null)
        {
            _canvas = FindObjectOfType<Canvas>().transform.GetComponent<Canvas>();
        }
    }


    public void CloseWindow()
    {
        if (_peepWindow == null)
            return;

        Destroy(_peepWindow);
    }


    public void OnDrag(PointerEventData eventData)
    {
        Assert.IsNotNull(_dragRectTransform, nameof(_dragRectTransform));
        Assert.IsNotNull(_canvas, nameof(_canvas));

        _dragRectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        Assert.IsNotNull(_dragRectTransform, nameof(_dragRectTransform));
        
        _dragRectTransform.SetAsLastSibling();
    }
}
