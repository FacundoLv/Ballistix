using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public Color tabNormal;
    public Color tabHighlighted;
    public Color tabSelected;

    public TabButton selectedTab;
    public PanelGroup panelGroup;

    private List<TabButton> tabButtons;

    public void Subscribe(TabButton button)
    {
        if (tabButtons == null) tabButtons = new List<TabButton>();

        tabButtons.Add(button);
    }

    public void OnTabEnter(TabButton button)
    {
        if (selectedTab != null && selectedTab == button) return;
        ResetTabs();
        button.Background.color = tabHighlighted;
    }

    public void OnTabExit(TabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton button)
    {
        if (selectedTab != null)
            selectedTab.Deselect();

        selectedTab = button;
        selectedTab.Select();

        ResetTabs();
        button.Background.color = tabSelected;

        int index = button.transform.GetSiblingIndex();
        panelGroup.SetPageIndex(index);
    }

    public void ResetTabs()
    {
        for (int i = 0; i < tabButtons.Count; i++)
        {
            if (selectedTab != null && tabButtons[i] == selectedTab) continue;
            tabButtons[i].Background.color = tabNormal;
        }
    }
}
