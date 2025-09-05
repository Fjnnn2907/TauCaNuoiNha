using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsGUI : MonoBehaviour
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
            rodNameText.text = $"Cần: {manager.CurrentRod.rodName} : R {manager.CurrentRod.bonusRareRate} | L {manager.CurrentRod.bonusLegendaryRate}";
        else
            rodNameText.text = "Cần: (Chưa chọn)";

        // ======== Bait ========
        if (manager.CurrentBait != null)
            baitNameText.text = $"Mồi: {manager.CurrentBait.baitName} : R {manager.CurrentBait.bonusRareRate} | L {manager.CurrentBait.bonusLegendaryRate}";
        else
            baitNameText.text = "Mồi: (Chưa chọn)";


        // ======== Tỉ lệ hiển thị ========
        float rarePercent = GetRareRatePercent(manager.TotalBonusRareRate);
        float legPercent = GetLegendaryRatePercent(manager.TotalBonusLegendaryRate);

        statsText.text = $"Tỉ lệ hiện tại:\n" +
                         $"- Rare: {rarePercent:F1}%\n" +
                         $"- Legendary: {legPercent:F1}%";
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
