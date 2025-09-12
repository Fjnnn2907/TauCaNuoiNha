using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishDetailPanel : Singleton<FishDetailPanel>
{
    [Header("UI")]
    public Image fishIcon;
    public TextMeshProUGUI fishNameText;
    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI rarityText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI priceText;
    public Button sellButton;

    private FishData currentFish;
    private int currentQuantity;

    private void Start()
    {
        sellButton.onClick.AddListener(SellFish);
    }

    private void OnEnable()
    {
        ClearPanel();
    }

    public void ShowFishInfo(FishData fish, int quantity)
    {
        if (fish == null) return;

        currentFish = fish;
        currentQuantity = quantity;

        fishIcon.sprite = fish.sprite;
        fishIcon.gameObject.SetActive(true);

        // 🔑 lấy tên & mô tả từ LanguageManager
        fishNameText.text = LanguageManager.Instance.GetText(fish.nameKey);
        descriptionText.text = LanguageManager.Instance.GetText(fish.descKey);

        // SL: (số lượng)
        quantityText.text = $"{LanguageManager.Instance.GetText("label_quantity")} {quantity}";

        // PL: (độ hiếm)
        string rarityLabel = LanguageManager.Instance.GetText("label_rarity");

        rarityText.text = fish.rarity switch
        {
            FishRarity.Common => $"<color=#656565>{rarityLabel} {fish.rarity}</color>",
            FishRarity.Rare => $"<color=#0069BF>{rarityLabel} {fish.rarity}</color>",
            FishRarity.Legendary => $"<color=#BF001C>{rarityLabel} {fish.rarity}</color>",
            _ => $"<color=#656565>{rarityLabel} {fish.rarity}</color>"
        };

        // Giá hoặc "Không thể bán"
        if (fish.isNotSellable)
        {
            priceText.text = LanguageManager.Instance.GetText("label_not_sellable");
        }
        else
        {
            string priceLabel = LanguageManager.Instance.GetText("label_price");
            priceText.text = $"{priceLabel} {fish.sellPrice}";
        }

        // ✅ Luôn hiện nút Sell, nhưng disable nếu cá không thể bán
        sellButton.gameObject.SetActive(true);
        sellButton.interactable = quantity > 0 && !fish.isNotSellable;
    }

    public void ClearPanel()
    {
        currentFish = null;
        currentQuantity = 0;

        fishIcon.gameObject.SetActive(false);
        fishNameText.text = "";
        quantityText.text = "";
        rarityText.text = "";
        descriptionText.text = "";
        priceText.text = "";
        sellButton.gameObject.SetActive(false);
    }

    private void SellFish()
    {
        if (currentFish == null || currentQuantity <= 0) return;

        CoinManager.Instance.AddCoins(currentFish.sellPrice);
        FishInventory.Instance.RemoveFish(currentFish, 1);

        int newQty = FishInventory.Instance.GetFishQuantity(currentFish);

        if (newQty > 0)
            ShowFishInfo(currentFish, newQty);
        else
            ClearPanel();

        FishInventory.Instance.RefreshUI();
    }
}
