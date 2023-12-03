using OpenRCT2.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable enable

[RequireComponent(typeof(RawImage))]
public class WindowTabButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField, Required] WindowTabGroup _tabGroup = null!;


    RawImage? _background;


    void Start()
    {
        _background = GetComponent<RawImage>();
        _tabGroup.Subscribe(this);
    }


    public void SetBackground(Texture image)
    {
        Assert.IsNotNull(_background, nameof(_background));

        _background.texture = image;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        _tabGroup.OnTabSelected(this);
    }
}
