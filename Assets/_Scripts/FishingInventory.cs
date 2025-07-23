using System.Collections.Generic;
using UnityEngine;

public class FishingInventory : Singleton<FishingInventory>
{
    public List<FishingRodData> ownedRods = new();

    public bool HasRod(FishingRodData rod)
    {
        return ownedRods.Contains(rod);
    }

    public void AddRod(FishingRodData rod)
    {
        if (!ownedRods.Contains(rod))
        {
            ownedRods.Add(rod);
        }
    }
}
