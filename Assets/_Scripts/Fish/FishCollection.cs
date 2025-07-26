using System.Collections.Generic;
using UnityEngine;

public class FishCollection : Singleton<FishCollection>, ISaveable
{
    public List<FishCollectionSlot> allSlots;
    public List<FishData> allFish;

    private HashSet<string> discoveredFishIDs = new();

    private void Start()
    {
        SaveManager.Instance.RegisterSaveable(this);
        RefreshUI();
    }

    public void DiscoverFish(FishData fish)
    {
        if (!discoveredFishIDs.Contains(fish.fishID))
        {
            discoveredFishIDs.Add(fish.fishID);
            RefreshUI();
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
    }

    public void LoadData(GameData data)
    {
        discoveredFishIDs = new HashSet<string>(data.discoveredFishIDs);
        RefreshUI();
    }
}
