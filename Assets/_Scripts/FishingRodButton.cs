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

    private FishingRodData rodData;
    private bool isOwned;

    public void Setup(FishingRodData data, bool owned)
    {
        rodData = data;
        isOwned = owned;

        icon.sprite = rodData.icon;
        nameText.text = rodData.rodName;
        button.onClick.RemoveAllListeners();

        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        if (isOwned)
        {
            if (FishingManager.Instance.CurrentRod == rodData)
            {
                useButtonText.text = "Đang dùng";
            }
            else
            {
                useButtonText.text = "Sử dụng";
                button.onClick.AddListener(() =>
                {
                    FishingManager.Instance.CurrentRod = rodData;
                    FishingManager.Instance.SetBonusRate(rodData.bonusRareRate, rodData.bonusLegendaryRate);
                    FindObjectOfType<FishingRodUI>().UpdateSelectedRodIcon(rodData.icon);
                    FindObjectOfType<FishingRodUI>().RefreshUI();
                });
            }
        }
        else
        {
            useButtonText.text = $"Mua ({rodData.price}💰)";
            button.onClick.AddListener(() =>
            {
                if (CoinManager.Instance.SpendCoins(rodData.price))
                {
                    FishingInventory.Instance.AddRod(rodData);
                    isOwned = true;

                    FishingManager.Instance.CurrentRod = rodData;
                    FishingManager.Instance.SetBonusRate(rodData.bonusRareRate, rodData.bonusLegendaryRate);
                    FindObjectOfType<FishingRodUI>().UpdateSelectedRodIcon(rodData.icon);

                    FindObjectOfType<FishingRodUI>().RefreshUI();
                }
                else
                {
                    Debug.Log("💸 Không đủ tiền!");
                }
            });
        }
    }
}
