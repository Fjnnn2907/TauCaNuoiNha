using UnityEngine;
using TMPro;

public class CoinManager : Singleton<CoinManager>, ISaveable
{
    public int currentCoins = 500; // Số tiền ban đầu
    public TextMeshProUGUI coinText;

    private void Start()
    {
        SaveManager.Instance.RegisterSaveable(this);
        UpdateUI();
    }

    public bool SpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (coinText != null)
            coinText.text = $"{currentCoins}";
    }

    // ========== SAVE / LOAD ==========

    public void SaveData(ref GameData data)
    {
        data.currentCoins = currentCoins;
    }

    public void LoadData(GameData data)
    {
        currentCoins = data.currentCoins;
        UpdateUI();
    }
}
