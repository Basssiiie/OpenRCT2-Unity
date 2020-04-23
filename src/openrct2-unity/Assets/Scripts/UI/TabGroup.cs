using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    [SerializeField] List<TabButton> tabButtons;
    [SerializeField] Texture tabIdle;
    [SerializeField] Texture tabActive;
    [SerializeField] TabButton selectedTab;
    [SerializeField] List<GameObject> objectsToSwap;

    public void Subscribe(TabButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<TabButton>();
        }

        tabButtons.Add(button);
    }

    public void OnTabSelected(TabButton button)
    {
        ResetTabs();
        button.background.texture = tabActive;
        selectedTab = button;
        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                objectsToSwap[i].SetActive(true);
            }
            else
            {
                objectsToSwap[i].SetActive(false);
            }
        }
    }

    public void ResetTabs()
    {
        foreach (TabButton button in tabButtons)
        {
            button.background.texture = tabIdle;
        }
    }
}
