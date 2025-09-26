using System.Collections.Generic;
using UnityEngine;

public class FishCollection : Singleton<FishCollection>, ISaveable
{
    [Header("UI References")]
    public List<FishCollectionSlot> allSlots;
    public List<FishData> allFish;
    public GameObject allFishCollectedPanel;

    private HashSet<string> discoveredFishIDs = new();
    private bool hasShownAllFishPanel = false;

    private void Start()
    {
        if (allFishCollectedPanel != null)
            allFishCollectedPanel.SetActive(false);

        SaveManager.Instance.RegisterSaveable(this);
        RefreshUI();
    }

    public void DiscoverFish(FishData fish)
    {
        if (!discoveredFishIDs.Contains(fish.fishID))
        {
            discoveredFishIDs.Add(fish.fishID);
            RefreshUI();

            if (discoveredFishIDs.Count >= allFish.Count && !hasShownAllFishPanel)
                ShowAllFishCollectedPanel();
        }
    }

    private void ShowAllFishCollectedPanel()
    {
        if (allFishCollectedPanel != null)
        {
            allFishCollectedPanel.SetActive(true);
            Debug.Log("🎉 Đã sưu tầm đủ tất cả cá!");
            hasShownAllFishPanel = true;
        }
    }

    public void RefreshUI()
    {
        for (int i = 0; i < allSlots.Count; i++)
        {
            var fish = allFish[i];
            var isDiscovered = discoveredFishIDs.Contains(fish.fishID);
            allSlots[i].Setup(fish, isDiscovered);
        }
    }

    public bool IsDiscovered(FishData fish) => discoveredFishIDs.Contains(fish.fishID);

    public void SaveData(ref GameData data)
    {
        data.discoveredFishIDs = new List<string>(discoveredFishIDs);
        data.hasShownAllFishPanel = hasShownAllFishPanel;
    }

    public void LoadData(GameData data)
    {
        discoveredFishIDs = new HashSet<string>(data.discoveredFishIDs);
        hasShownAllFishPanel = data.hasShownAllFishPanel;
        RefreshUI();
    }

    [ContextMenu("Test Collect All Fish")]
    public void TestCollectAllFish()
    {
        foreach (var fish in allFish)
        {
            DiscoverFish(fish);
        }
    }
}
