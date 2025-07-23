using UnityEngine;
using TMPro;

public class CoinManager : Singleton<CoinManager>
{
    public int currentCoins = 500; // số tiền ban đầu
    public TextMeshProUGUI coinText;

    private void Start()
    {
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
            coinText.text = $"{currentCoins} 💰";
    }
}
