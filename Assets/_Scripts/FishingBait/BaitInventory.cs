using System.Collections.Generic;
using UnityEngine;

public class BaitInventory : Singleton<BaitInventory>
{
    private Dictionary<FishingBaitData, int> baitQuantities = new();

    public void AddBait(FishingBaitData bait, int quantity)
    {
        if (!baitQuantities.ContainsKey(bait))
            baitQuantities[bait] = 0;
        baitQuantities[bait] += quantity;
    }

    public bool HasBait(FishingBaitData bait)
    {
        return baitQuantities.ContainsKey(bait) && baitQuantities[bait] >= bait.quantityRequired;
    }

    public bool ConsumeBait(FishingBaitData bait)
    {
        if (!HasBait(bait)) return false;

        baitQuantities[bait] -= bait.quantityRequired;
        return true;
    }

    public int GetQuantity(FishingBaitData bait)
    {
        return baitQuantities.ContainsKey(bait) ? baitQuantities[bait] : 0;
    }
}
