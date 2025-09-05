using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class QuestManager : Singleton<QuestManager>, ISaveable
{
    public QuestData currentQuest;
    private int currentProgress = 0;
    private int questsCompleted = 0;

    private string currentZone;
    private int zoneLockedQuestCount = 0;
    private int zoneQuestLimit = 0; // sẽ random trong Start

    private void Start()
    {
        SaveManager.Instance.RegisterSaveable(this);
        zoneQuestLimit = Random.Range(3, 6);
    }

    public void GenerateNewQuest()
    {
        FishRarity targetRarity = GetRarityByProgress(questsCompleted);

        currentZone = GetCurrentZoneFromScene();
        List<FishData> fishesOfRarity;

        if (zoneLockedQuestCount < zoneQuestLimit)
        {
            fishesOfRarity = FishDatabase.Instance.allFish
                .Where(f => f.rarity == targetRarity && f.zone == currentZone)
                .Where(f => !f.isUnique || FishInventory.Instance.GetFishQuantity(f) == 0) // ✅ Không chọn cá unique đã bắt
                .ToList();

            if (fishesOfRarity.Count > 0)
                zoneLockedQuestCount++;
            else
            {
                fishesOfRarity = FishDatabase.Instance.allFish
                    .Where(f => f.rarity == targetRarity)
                    .Where(f => !f.isUnique || FishInventory.Instance.GetFishQuantity(f) == 0) // ✅ Không chọn cá unique đã bắt
                    .ToList();
            }
        }
        else
        {
            fishesOfRarity = FishDatabase.Instance.allFish
                .Where(f => f.rarity == targetRarity)
                .Where(f => !f.isUnique || FishInventory.Instance.GetFishQuantity(f) == 0) // ✅ Không chọn cá unique đã bắt
                .ToList();
        }

        if (fishesOfRarity.Count == 0)
        {
            Debug.LogError($"❌ Không có cá nào thuộc loại {targetRarity} để tạo nhiệm vụ.");
            return;
        }

        FishData targetFish = fishesOfRarity[Random.Range(0, fishesOfRarity.Count)];

        // ✅ Cá unique chỉ yêu cầu 1 con
        int amount = targetFish.isUnique ? 1 : Random.Range(2, 6);

        int baseReward = amount * (int)targetRarity * 15 + 30;
        float rewardMultiplier = 1f + questsCompleted * 0.1f;
        int finalReward = Mathf.RoundToInt(baseReward * rewardMultiplier);

        currentQuest = ScriptableObject.CreateInstance<QuestData>();
        currentQuest.questID = "quest_" + questsCompleted;
        currentQuest.requiredRarity = targetRarity;
        currentQuest.requiredFishName = targetFish.fishName;
        currentQuest.requiredAmount = amount;
        currentQuest.rewardGold = finalReward;
        currentQuest.description = $"Câu {amount} con {targetFish.fishName} ({targetRarity})\n" +
                                   $"📍 Xuất hiện ở vùng: {targetFish.zone}";

        currentProgress = 0;
        QuestUI.Instance?.UpdateUI();

        Debug.Log($"📜 Nhiệm vụ mới: {currentQuest.description}");
    }

    private string GetCurrentZoneFromScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        var location = MapManager.Instance.locations.FirstOrDefault(loc => loc.sceneName == sceneName);
        return location != null ? location.locationName : "Không xác định";
    }

    public void OnFishCaught(FishData fish)
    {
        if (fish.fishName == currentQuest.requiredFishName &&
            fish.rarity == currentQuest.requiredRarity)
        {
            // ✅ Nếu cá là unique và đã câu trong nhiệm vụ, bỏ qua
            if (fish.isUnique && currentProgress >= 1)
            {
                Debug.Log($"⚠️ {fish.fishName} là cá unique, đã bắt rồi.");
                return;
            }

            currentProgress++;
            QuestUI.Instance?.UpdateUI();
            Debug.Log($"🐟 Đã câu được {currentProgress}/{currentQuest.requiredAmount} {fish.fishName}");

            if (currentProgress >= currentQuest.requiredAmount)
            {
                CompleteQuest();
            }
        }
    }

    private void CompleteQuest()
    {
        Debug.Log($"✅ Đã hoàn thành nhiệm vụ! Nhận {currentQuest.rewardGold} vàng");
        questsCompleted++;
        NotificationManager.Instance?.ShowNotification($"Bạn hoàn thành nhiệm vụ nhận {currentQuest.rewardGold} vàng");
        CoinManager.Instance?.AddCoins(currentQuest.rewardGold);

        QuestUI.Instance?.ShowCompleteEffect();
        NotificationManager.Instance?.ShowNotification("Bạn có nhiệm vụ mới");
        GenerateNewQuest();
    }

    public int GetCurrentProgress()
    {
        return currentProgress;
    }

    private FishRarity GetRarityByProgress(int progress)
    {
        if (progress < 3) return FishRarity.Common;
        if (progress < 6) return FishRarity.Rare;
        return FishRarity.Legendary;
    }

    public void SaveData(ref GameData data)
    {
        if (currentQuest == null) return;

        data.questFishName = currentQuest.requiredFishName;
        data.questRarity = currentQuest.requiredRarity;
        data.questAmount = currentQuest.requiredAmount;
        data.questProgress = currentProgress;
        data.questReward = currentQuest.rewardGold;
        data.questsCompleted = questsCompleted;

        data.zoneQuestLimit = zoneQuestLimit;
        data.zoneLockedQuestCount = zoneLockedQuestCount;
    }

    public void LoadData(GameData data)
    {
        questsCompleted = data.questsCompleted;

        if (string.IsNullOrEmpty(data.questFishName))
        {
            GenerateNewQuest();
            return;
        }

        currentQuest = ScriptableObject.CreateInstance<QuestData>();
        currentQuest.questID = "quest_" + data.questsCompleted;
        currentQuest.requiredFishName = data.questFishName;
        currentQuest.requiredRarity = data.questRarity;
        currentQuest.requiredAmount = data.questAmount;
        currentQuest.rewardGold = data.questReward;
        currentQuest.description = $"Câu {data.questAmount} con {data.questFishName} ({data.questRarity})";

        currentProgress = data.questProgress;

        zoneQuestLimit = data.zoneQuestLimit;
        zoneLockedQuestCount = data.zoneLockedQuestCount;

        Debug.Log($"🔁 Đã load nhiệm vụ: {currentQuest.description}, Tiến độ: {currentProgress}/{currentQuest.requiredAmount}");

        QuestUI.Instance?.UpdateUI();
    }
}
