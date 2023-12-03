using System;
using System.Collections.Generic;
using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

public class WindowTabGroup : MonoBehaviour
{
    [SerializeField] List<WindowTabButton>? _tabButtons;
    [SerializeField] Texture? _tabIdle;
    [SerializeField] Texture? _tabActive;
    [SerializeField] WindowTabButton? _selectedTab;
    [SerializeField] GameObject[] _objectsToSwap = Array.Empty<GameObject>();


    public void Subscribe(WindowTabButton button)
    {
        if (_tabButtons == null)
        {
            _tabButtons = new List<WindowTabButton>();
        }

        _tabButtons.Add(button);
    }


    public void OnTabSelected(WindowTabButton button)
    {
        Assert.IsNotNull(_tabActive, nameof(_tabActive));

        ResetTabs();
        button.SetBackground(_tabActive);
        _selectedTab = button;

        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < _objectsToSwap.Length; i++)
        {
            if (i == index)
            {
                _objectsToSwap[i].SetActive(true);
            }
            else
            {
                _objectsToSwap[i].SetActive(false);
            }
        }
    }


    public void ResetTabs()
    {
        if (_tabButtons == null)
            return;

        Assert.IsNotNull(_tabIdle, nameof(_tabIdle));

        foreach (WindowTabButton button in _tabButtons)
        {
            button.SetBackground(_tabIdle);
        }
    }
}
