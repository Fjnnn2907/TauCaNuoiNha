﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishingBaitButton : MonoBehaviour
{
    [Header("UI")]
    public Image icon;
    public TextMeshProUGUI nameText;

    public Button buyButton;
    public Button actionButton;
    public TextMeshProUGUI actionButtonText;
    public TextMeshProUGUI rareRateText;
    public TextMeshProUGUI legendaryRateText;

    private FishingBaitData baitData;

    public void Setup(FishingBaitData bait, int currentQuantity)
    {
        baitData = bait;

        icon.sprite = bait.icon;
        nameText.text = bait.baitName;

        rareRateText.text = $"R {bait.bonusRareRate}";
        legendaryRateText.text = $"L {bait.bonusLegendaryRate}";

        SetupUseButton(currentQuantity);
        SetupBuyButton();
    }

    private void SetupUseButton(int quantity)
    {
        actionButton.onClick.RemoveAllListeners();

        bool isSelected = FishingBaitUI.Instance.IsCurrentBait(baitData);

        if (isSelected)
        {
            actionButtonText.text = "Đã sử dụng";
            actionButton.interactable = false;
        }
        else
        {
            actionButtonText.text = "Sử dụng";
            actionButton.interactable = quantity > 0;
            actionButton.onClick.AddListener(() =>
            {
                if (quantity > 0)
                {
                    UseBait();
                }
            });
        }
    }

    private void SetupBuyButton()
    {
        buyButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Mua ({baitData.price})";
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => BuyBait());
    }

    private void UseBait()
    {
        FishingBaitUI.Instance.SetCurrentBait(baitData);
        FishingManager.Instance.SetBaitBonus(baitData);
    }

    private void BuyBait()
    {
        if (CoinManager.Instance.SpendCoins(baitData.price))
        {
            BaitInventory.Instance.AddBait(baitData, 1);
            FishingBaitUI.Instance?.RefreshUI();
        }
        else
        {
            Debug.Log("❌ Không đủ coin để mua mồi!");
        }
    }
}
