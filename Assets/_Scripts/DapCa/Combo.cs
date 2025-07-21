using TMPro;
using UnityEngine;

public class Combo : MonoBehaviour
{
    public int combo = 0;
    public int maxCombo = 0;

    public TextMeshProUGUI comboText;

    public void AddCombo()
    {
        combo++;
        if (combo > maxCombo)
            maxCombo = combo;

        UpdateUI();
    }

    public void ResetCombo()
    {
        combo = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        comboText.text = $"Combo: x{combo}";
    }
}
