using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RawImage))]
public class TabButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] TabGroup tabGroup;
    public RawImage background;


    void Start()
    {
        background = GetComponent<RawImage>();
        tabGroup.Subscribe(this);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }
}
