using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingBaitUI : Singleton<FishingBaitUI>
{
    public GameObject panel;
    public List<FishingBaitButton> baitButtons;
    public List<FishingBaitData> allBaits;
    public Image selectedBaitIcon;

    public void ToggleUI()
    {
        panel.SetActive(!panel.activeSelf);

        if (panel.activeSelf)
        {
            for (int i = 0; i < baitButtons.Count; i++)
            {
                int quantity = BaitInventory.Instance.GetQuantity(allBaits[i]);
                baitButtons[i].Setup(allBaits[i], quantity);
            }
        }
    }

    public void RefreshUI()
    {
        for (int i = 0; i < baitButtons.Count; i++)
        {
            int quantity = BaitInventory.Instance.GetQuantity(allBaits[i]);
            baitButtons[i].Setup(allBaits[i], quantity);
        }
    }

    public void SetCurrentBait(FishingBaitData bait)
    {
        FishingManager.Instance.CurrentBait = bait;
        if (selectedBaitIcon != null)
            selectedBaitIcon.sprite = bait.icon;

        Debug.Log($"🎯 Đã chọn mồi: {bait.baitName}");
    }
}