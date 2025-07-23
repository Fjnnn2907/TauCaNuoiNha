using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingRodUI : Singleton<FishingRodUI>
{
    public GameObject panel;
    public List<FishingRodButton> rodButtons;
    public List<FishingRodData> allRods;

    public Image iconButton;

    public void ToggleUI()
    {
        panel.SetActive(!panel.activeSelf);

        if (panel.activeSelf)
        {
            RefreshUI();
        }
    }

    public void RefreshUI()
    {
        for (int i = 0; i < rodButtons.Count; i++)
        {
            bool owned = FishingInventory.Instance.HasRod(allRods[i]);
            rodButtons[i].Setup(allRods[i], owned);
        }
    }

    public void UpdateSelectedRodIcon(Sprite newIcon)
    {
        iconButton.sprite = newIcon;
    }
}
