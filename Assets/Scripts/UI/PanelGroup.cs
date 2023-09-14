using UnityEngine;

public class PanelGroup : MonoBehaviour
{
    public GameObject[] panels;

    public int panelIndex;

    private void Awake()
    {
        ShowCurrentPanel();
    }

    private void ShowCurrentPanel()
    {
        bool toogle = default;
        for (int i = 0; i < panels.Length; i++)
        {
            toogle = panelIndex == i;
            panels[i].gameObject.SetActive(toogle);
        }
    }

    public void SetPageIndex(int index)
    {
        panelIndex = index;
        ShowCurrentPanel();
    }
}
