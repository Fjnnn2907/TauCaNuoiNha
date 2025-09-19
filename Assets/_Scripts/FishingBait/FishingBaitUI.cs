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

        // Cập nhật icon (kiểm tra null an toàn)
        if (selectedBaitIcon != null)
        {
            if (bait != null && bait.icon != null)
                selectedBaitIcon.sprite = bait.icon;
        }

        UpdateSelectedBaitQuantity();
        RefreshUI();
    }

    private void UpdateSelectedBaitQuantity()
    {
        if (quantityText == null) return;

        if (currentBait == null)
        {
            // nếu không còn mồi thì xóa text hiển thị
            quantityText.text = "";
            return;
        }

        int qty = BaitInventory.Instance.GetQuantity(currentBait);
        quantityText.text = qty > 0 ? qty.ToString() : "";
    }

}
