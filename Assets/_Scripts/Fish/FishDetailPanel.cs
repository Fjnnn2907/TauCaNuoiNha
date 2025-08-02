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

        fishNameText.text = fish.fishName;
        quantityText.text = $"SL: {quantity}";
        rarityText.text = $"PL: {fish.rarity}";
        descriptionText.text = fish.description;
        priceText.text = $"Giá: {fish.sellPrice}";

        // Đổi màu tên cá theo độ hiếm
        rarityText.text = fish.rarity switch
        {
            FishRarity.Common => $"<color=#656565>PL: {fish.rarity}</color>",
            FishRarity.Rare => $"<color=#0069BF>PL: {fish.rarity}</color>",
            FishRarity.Legendary => $"<color=#BF001C>PL: {fish.rarity}</color>",
            _ => $"<color=#656565>PL: {fish.rarity}</color>"
        };

        sellButton.interactable = quantity > 0;
        sellButton.gameObject.SetActive(true);
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

        // Cộng tiền
        CoinManager.Instance.AddCoins(currentFish.sellPrice);

        // Trừ cá khỏi kho
        FishInventory.Instance.RemoveFish(currentFish, 1);

        // Lấy lại số lượng mới
        int newQty = FishInventory.Instance.GetFishQuantity(currentFish);

        if (newQty > 0)
            ShowFishInfo(currentFish, newQty);
        else
            ClearPanel();

        FishInventory.Instance.RefreshUI();
    }
}
