using TMPro;
using UnityEngine;

public class QuestUI : Singleton<QuestUI>
{
    public GameObject panel;

    [Header("UI References")]
    public TextMeshProUGUI questDescriptionText;
    public TextMeshProUGUI questProgressText;
    public TextMeshProUGUI questZoneText;
    public TextMeshProUGUI questRewardText;

    private void Start()
    {
        UpdateUI();
    }

    public void ToggleUI()
    {
        panel.SetActive(!panel.activeSelf);

        if (panel.activeSelf)
        {
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        var quest = QuestManager.Instance.currentQuest;
        if (quest == null) return;

        // ✅ Lấy fish theo tên
        var fish = FishDatabase.Instance.allFish.Find(f => f.fishName == quest.requiredFishName);

        // ✅ Lấy tên hiển thị qua nameKey
        string fishDisplayName = fish != null && !string.IsNullOrEmpty(fish.nameKey)
            ? LanguageManager.Instance.GetText(fish.nameKey)
            : quest.requiredFishName;

        // ✅ Lấy zone hiển thị qua dịch nếu muốn
        string zone = GetFishZone(fish);

        // ✅ Format quest description đa ngôn ngữ
        // Trong file Excel bạn tạo key: quest_catch = "Câu {0} con {1} ({2})"
        string descriptionFormat = LanguageManager.Instance.GetText("quest_catch");
        questDescriptionText.text = string.Format(descriptionFormat, quest.requiredAmount, fishDisplayName, quest.requiredRarity);

        // ✅ Tiến độ
        questProgressText.text = $"{QuestManager.Instance.GetCurrentProgress()}/{quest.requiredAmount}";

        // ✅ Zone
        questZoneText.text = zone;

        // ✅ Reward
        string rewardFormat = LanguageManager.Instance.GetText("quest_reward");
        // ví dụ trong Excel: quest_reward = "{0} vàng"
        questRewardText.text = string.Format(rewardFormat, quest.rewardGold);
    }

    private string GetFishZone(FishData fish)
    {
        if (fish == null) return LanguageManager.Instance.GetText("zone_unknown"); // fallback

        // Nếu bạn muốn zone cũng dịch được thì thêm key vào Excel
        return !string.IsNullOrEmpty(fish.zone)
            ? LanguageManager.Instance.GetText(fish.zone)
            : LanguageManager.Instance.GetText("zone_unknown");
    }

    public void ShowCompleteEffect()
    {
        Debug.Log("🎉 Đã hoàn thành nhiệm vụ!");
        // TODO: thêm animation hoặc sound effect
    }
}
