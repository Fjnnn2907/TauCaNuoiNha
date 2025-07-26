using System.Collections.Generic;
using UnityEngine;

public class FishCollection : Singleton<FishCollection>
{
    public List<FishCollectionSlot> allSlots; // Gán sẵn từ Unity Editor

    private HashSet<FishData> discovered = new();

    private void Start()
    {
        RefreshUI();
    }

    public void DiscoverFish(FishData fish)
    {
        if (!discovered.Contains(fish))
        {
            discovered.Add(fish);
            RefreshUI();
        }
    }

    public void RefreshUI()
    {
        for (int i = 0; i < allSlots.Count; i++)
        {
            var fish = FishDatabase.Instance.allFish[i];
            var isDiscovered = discovered.Contains(fish);
            allSlots[i].Setup(fish, isDiscovered);
        }
    }

    public bool IsDiscovered(FishData fish) => discovered.Contains(fish);
}
