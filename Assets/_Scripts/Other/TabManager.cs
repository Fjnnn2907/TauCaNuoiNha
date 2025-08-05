using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TabManager : MonoBehaviour
{
    [System.Serializable]
    public class TabPair
    {
        public Button tabButton;
        public GameObject tabPanel;
    }

    public List<TabPair> tabs;
    private Color activeTabColor = Color.white;
    private Color inactiveTabColor = Color.white;

    private int currentTabIndex = 0;

    void Start()
    {
        SwitchTab(0);
        for (int i = 0; i < tabs.Count; i++)
        {
            int index = i; // Local copy for closure
            tabs[i].tabButton.onClick.AddListener(() => SwitchTab(index));
        }
    }

    public void SwitchTab(int newIndex)
    {
        // Bỏ chọn tab hiện tại
        tabs[currentTabIndex].tabPanel.SetActive(false);
        tabs[currentTabIndex].tabButton.interactable = true;

        // Chọn tab mới
        currentTabIndex = newIndex;
        tabs[currentTabIndex].tabPanel.SetActive(true);
        tabs[currentTabIndex].tabButton.interactable = false;

        // Cập nhật màu (tuỳ chọn)
        UpdateTabColors();
    }

    private void UpdateTabColors()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            var colors = tabs[i].tabButton.colors;
            colors.normalColor = (i == currentTabIndex) ? activeTabColor : inactiveTabColor;
            tabs[i].tabButton.colors = colors;
        }
    }
}