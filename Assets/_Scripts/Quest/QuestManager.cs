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
    private int zoneQuestLimit = 0;

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
                .Where(f => !f.isUnique || FishInventory.Instance.GetFishQuantity(f) == 0)
                .ToList();

            if (fishesOfRarity.Count > 0)
                zoneLockedQuestCount++;
            else
            {
                fishesOfRarity = FishDatabase.Instance.allFish
                    .Where(f => f.rarity == targetRarity)
                    .Where(f => !f.isUnique || FishInventory.Instance.GetFishQuantity(f) == 0)
                    .ToList();
            }
        }
        else
        {
            fishesOfRarity = FishDatabase.Instance.allFish
                .Where(f => f.rarity == targetRarity)
                .Where(f => !f.isUnique || FishInventory.Instance.GetFishQuantity(f) == 0)
                .ToList();
        }

        if (fishesOfRarity.Count == 0)
        {
            Debug.LogError(string.Format(LanguageManager.Instance.GetText("quest_not_enough_fish"), targetRarity));
            return;
        }

        FishData targetFish = fishesOfRarity[Random.Range(0, fishesOfRarity.Count)];

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

        // ✅ Không hardcode description, QuestUI sẽ tự build bằng ngôn ngữ
        currentQuest.description = string.Format(
            LanguageManager.Instance.GetText("quest_catch"),
            amount, targetFish.fishName, targetRarity);

        currentProgress = 0;
        QuestUI.Instance?.UpdateUI();

        Debug.Log($"📜 {currentQuest.description}");
    }

    private string GetCurrentZoneFromScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        var location = MapManager.Instance.locations.FirstOrDefault(loc => loc.sceneName == sceneName);
        return location != null ? location.nameKey : LanguageManager.Instance.GetText("quest_unknown_zone");
    }

    public void OnFishCaught(FishData fish)
    {
        if (fish.fishName == currentQuest.requiredFishName &&
            fish.rarity == currentQuest.requiredRarity)
        {
            if (fish.isUnique && currentProgress >= 1)
            {
                Debug.Log($"⚠️ {fish.fishName} unique, bỏ qua");
                return;
            }

            currentProgress++;
            QuestUI.Instance?.UpdateUI();

            if (currentProgress >= currentQuest.requiredAmount)
            {
                CompleteQuest();
            }
        }
    }

    private void CompleteQuest()
    {
        Debug.Log($"{LanguageManager.Instance.GetText("quest_complete")} + {currentQuest.rewardGold}");
        questsCompleted++;
        NotificationManager.Instance?.ShowNotification(
            string.Format(LanguageManager.Instance.GetText("quest_reward"), currentQuest.rewardGold));
        CoinManager.Instance?.AddCoins(currentQuest.rewardGold);

        QuestUI.Instance?.ShowCompleteEffect();
        NotificationManager.Instance?.ShowNotification(LanguageManager.Instance.GetText("quest_new"));
        GenerateNewQuest();
    }

    public int GetCurrentProgress() => currentProgress;

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

        currentQuest.description = string.Format(
            LanguageManager.Instance.GetText("quest_catch"),
            data.questAmount, data.questFishName, data.questRarity);

        currentProgress = data.questProgress;
        zoneQuestLimit = data.zoneQuestLimit;
        zoneLockedQuestCount = data.zoneLockedQuestCount;

        QuestUI.Instance?.UpdateUI();
    }
}
