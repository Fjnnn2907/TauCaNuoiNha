using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsGUI : Singleton<StatsGUI>
{
    public GameObject panel;
    public Button button;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI rodNameText;
    [SerializeField] private TextMeshProUGUI baitNameText;
    [SerializeField] private TextMeshProUGUI statsText;

    private void Start()
    {
        button.onClick.AddListener(ToggleUI);
    }

    public void ToggleUI()
    {
        panel.SetActive(!panel.activeSelf);

        if (panel.activeSelf)
        {
            RefreshUI();
        }
    }

    public void RefreshUI()
    {
        var manager = FishingManager.Instance;

        // ======== Rod ========
        if (manager.CurrentRod != null)
        {
            string rodLabel = LanguageManager.Instance.GetText("stats_rod");         // "Cần"
            string rareLabel = LanguageManager.Instance.GetText("stats_rare");       // "Rare"
            string legLabel = LanguageManager.Instance.GetText("stats_legendary");   // "Legendary"

            rodNameText.text = $"{rodLabel}: {manager.CurrentRod.GetRodName()}: {rareLabel} {manager.CurrentRod.bonusRareRate}, {legLabel} {manager.CurrentRod.bonusLegendaryRate}";
        }
        else
        {
            string rodLabel = LanguageManager.Instance.GetText("stats_rod");
            string noneText = LanguageManager.Instance.GetText("stats_none");        // "(Chưa chọn)"
            rodNameText.text = $"{rodLabel}: {noneText}";
        }

        // ======== Bait ========
        if (manager.CurrentBait != null)
        {
            string baitLabel = LanguageManager.Instance.GetText("stats_bait");       // "Mồi"
            string rareLabel = LanguageManager.Instance.GetText("stats_rare");
            string legLabel = LanguageManager.Instance.GetText("stats_legendary");

            baitNameText.text = $"{baitLabel}: {manager.CurrentBait.GetBaitName()}: {rareLabel} {manager.CurrentBait.bonusRareRate}, {legLabel} {manager.CurrentBait.bonusLegendaryRate}";
        }
        else
        {
            string baitLabel = LanguageManager.Instance.GetText("stats_bait");
            string noneText = LanguageManager.Instance.GetText("stats_none");
            baitNameText.text = $"{baitLabel}: {noneText}";
        }

        // ======== Tỉ lệ hiển thị ========
        float rarePercent = GetRareRatePercent(manager.TotalBonusRareRate);
        float legPercent = GetLegendaryRatePercent(manager.TotalBonusLegendaryRate);

        string currentRateLabel = LanguageManager.Instance.GetText("stats_current_rate"); // "Tỉ lệ hiện tại"
        string rareLabel2 = LanguageManager.Instance.GetText("stats_rare");
        string legLabel2 = LanguageManager.Instance.GetText("stats_legendary");

        statsText.text = $"{currentRateLabel}:\n" +
                         $"- {rareLabel2}: {rarePercent:F1}%\n" +
                         $"- {legLabel2}: {legPercent:F1}%";
    }

    // ========== Helper để quy đổi ==========
    private float GetRareRatePercent(float rareRate)
    {
        // Rare: 9 -> 0%, 92 -> +25%
        float scale = Mathf.Clamp01((rareRate - 9f) / (92f - 9f));
        return scale * 25f;
    }

    private float GetLegendaryRatePercent(float legRate)
    {
        // Legendary: 4 -> 0%, 83 -> +15%
        float scale = Mathf.Clamp01((legRate - 4f) / (83f - 4f));
        return scale * 15f;
    }
}
