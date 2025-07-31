using TMPro;
using UnityEngine;

public class QuestUI : Singleton<QuestUI>
{
    [Header("UI References")]
    public TextMeshProUGUI questDescriptionText;
    public TextMeshProUGUI questProgressText;
    public TextMeshProUGUI questZoneText;
    public TextMeshProUGUI questRewardText;

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        var quest = QuestManager.Instance.currentQuest;
        if (quest == null) return;

        string fishName = quest.requiredFishName;
        string zone = GetFishZone(fishName);

        questDescriptionText.text = $"Câu {quest.requiredAmount} con {fishName} ({quest.requiredRarity})";
        questProgressText.text = $"{QuestManager.Instance.GetCurrentProgress()}/{quest.requiredAmount}";
        questZoneText.text = $"{zone}";
        questRewardText.text = $"{quest.rewardGold} vàng";
    }

    private string GetFishZone(string fishName)
    {
        var fish = FishDatabase.Instance.allFish.Find(f => f.fishName == fishName);
        return fish != null ? fish.zone : "Không xác định";
    }

    public void ShowCompleteEffect()
    {
        Debug.Log("🎉 Đã hoàn thành nhiệm vụ!");
        // TODO: Thêm animation UI hoặc âm thanh ở đây nếu muốn
    }
}
