using TMPro;
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

    private FishingBaitData baitData;
    private int quantity;

    public void Setup(FishingBaitData bait, int currentQuantity)
    {
        baitData = bait;
        quantity = currentQuantity;

        icon.sprite = bait.icon;
        nameText.text = $"{bait.baitName} x{quantity}";

        SetupUseButton();
        SetupBuyButton();
    }

    private void SetupUseButton()
    {
        actionButton.interactable = quantity > 0;
        actionButtonText.text = "Sử dụng";

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() =>
        {
            if (quantity > 0)
            {
                UseBait();
            }
        });
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
            FishingBaitUI.Instance.RefreshUI();
        }
        else
        {
            Debug.Log("❌ Không đủ coin để mua mồi!");
        }
    }
}
