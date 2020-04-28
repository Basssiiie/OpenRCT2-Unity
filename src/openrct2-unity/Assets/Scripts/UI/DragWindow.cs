using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragWindow : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField] RectTransform dragRectTransform;
    [SerializeField] GameObject peepWindow;
    [SerializeField] Canvas canvas;

    public void Awake()
    {
        if (dragRectTransform == null)
        {
            dragRectTransform = dragRectTransform.parent.GetComponent<RectTransform>();
        }

        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>().transform.GetComponent<Canvas>();
        }
    }


    public void CloseWindow()
    {
        Destroy(peepWindow);
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        dragRectTransform.SetAsLastSibling();
    }
}
