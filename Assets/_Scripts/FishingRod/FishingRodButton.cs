using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishingRodButton : MonoBehaviour
{
    [Header("UI")]
    public Image icon;
    public TextMeshProUGUI nameText;
    public Button button;
    public TextMeshProUGUI useButtonText;
    public TextMeshProUGUI rareRateText;
    public TextMeshProUGUI legendaryRateText;

    private FishingRodData rodData;
    private bool isOwned;

    public void Setup(FishingRodData data, bool owned)
    {
        rodData = data;
        isOwned = owned;

        icon.sprite = rodData.icon;
        nameText.text = rodData.GetRodName();
        rareRateText.text = $"R {data.bonusRareRate}";
        legendaryRateText.text = $"L {data.bonusLegendaryRate}";
        button.onClick.RemoveAllListeners();

        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        if (isOwned)
        {
            if (FishingManager.Instance.CurrentRod == rodData)
            {
                // ✅ "Đang dùng"
                useButtonText.text = LanguageManager.Instance.GetText("dang_dung");
            }
            else
            {
                // ✅ "Sử dụng"
                useButtonText.text = LanguageManager.Instance.GetText("su_dung");

                button.onClick.AddListener(() =>
                {
                    FishingManager.Instance.CurrentRod = rodData;
                    FishingManager.Instance.SetRodBonus(rodData);
                    FishingRodUI.Instance.UpdateSelectedRodIcon(rodData.icon);
                    FishingRodUI.Instance.RefreshUI();
                });
            }
        }
        else
        {
            // ✅ "Mua (giá)"
            string buyText = LanguageManager.Instance.GetText("mua");
            useButtonText.text = $"{buyText}\n({rodData.price})";

            button.onClick.AddListener(() =>
            {
                if (CoinManager.Instance.SpendCoins(rodData.price))
                {
                    FishingInventory.Instance.AddRod(rodData);
                    isOwned = true;

                    FishingManager.Instance.CurrentRod = rodData;
                    FishingManager.Instance.SetRodBonus(rodData);
                    FishingRodUI.Instance.UpdateSelectedRodIcon(rodData.icon);

                    FishingRodUI.Instance.RefreshUI();
                }
                else
                {
                    Debug.Log("💸 Không đủ tiền!");
                }
            });
        }
    }
}
