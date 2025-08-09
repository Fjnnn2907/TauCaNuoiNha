using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FishDatabase : Singleton<FishDatabase>
{
    public List<FishData> zoneFish;
    public List<FishData> allFish;
    public FishData GetRandomFish(FishRarity rarity)
    {
        var list = zoneFish
            .Where(f => f.rarity == rarity)
            .Where(f => !f.isUnique || FishInventory.Instance.GetFishQuantity(f) == 0) // ✅ Loại cá unique đã câu rồi
            .ToList();

        if (list.Count == 0) return null;

        return list[Random.Range(0, list.Count)];
    }

}
