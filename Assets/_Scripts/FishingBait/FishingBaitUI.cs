using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishingBaitUI : Singleton<FishingBaitUI>
{
    public GameObject panel;
    public List<FishingBaitButton> baitButtons;
    public List<FishingBaitData> allBaits;
    public Image selectedBaitIcon;
    public TextMeshProUGUI quantityText;

    private FishingBaitData currentBait;

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
        for (int i = 0; i < baitButtons.Count; i++)
        {
            int quantity = BaitInventory.Instance.GetQuantity(allBaits[i]);
            baitButtons[i].Setup(allBaits[i], quantity);
        }

        UpdateSelectedBaitQuantity();
    }
    public bool IsCurrentBait(FishingBaitData bait)
    {
        return FishingManager.Instance.CurrentBait == bait;
    }
    public void SetCurrentBait(FishingBaitData bait)
    {
        currentBait = bait;
        FishingManager.Instance.CurrentBait = bait;

        if (selectedBaitIcon != null)
        {
            selectedBaitIcon.sprite = bait.icon;
        }

        UpdateSelectedBaitQuantity();
        RefreshUI();
        Debug.Log($"🎯 Đã chọn mồi: {bait.baitName}");
    }

    private void UpdateSelectedBaitQuantity()
    {
        if (currentBait == null || quantityText == null)
            return;

        int qty = BaitInventory.Instance.GetQuantity(currentBait);
        if (qty < 1)
            quantityText.text = "";
        else
            quantityText.text = qty.ToString();
    }
}
