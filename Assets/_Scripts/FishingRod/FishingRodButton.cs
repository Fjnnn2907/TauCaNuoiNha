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
                    FishingManager.Instance.SetRodBonus(rodData);
                    FishingRodUI.Instance.UpdateSelectedRodIcon(rodData.icon);
                    FishingRodUI.Instance.RefreshUI();
                });
            }
        }
        else
        {
            useButtonText.text = $"Mua ({rodData.price} VND)";
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
