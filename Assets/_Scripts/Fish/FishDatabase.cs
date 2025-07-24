using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FishDatabase : Singleton<FishDatabase>
{
    [SerializeField] private List<FishData> allFish;

    public FishData GetRandomFish(FishRarity rarity)
    {
        var list = allFish.Where(f => f.rarity == rarity).ToList();
        if (list.Count == 0) return null;

        return list[Random.Range(0, list.Count)];
    }
}
