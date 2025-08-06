using System.Collections.Generic;
using UnityEngine;

public class BaitInventory : Singleton<BaitInventory>, ISaveable
{
    private Dictionary<FishingBaitData, int> baitQuantities = new();

    private void Start()
    {
        SaveManager.Instance.RegisterSaveable(this);
    }

    public void AddBait(FishingBaitData bait, int quantity)
    {
        if (!baitQuantities.ContainsKey(bait))
            baitQuantities[bait] = 0;
        baitQuantities[bait] += quantity;

        FishingBaitUI.Instance?.RefreshUI();
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

    // ================= SAVE / LOAD =================
    public void SaveData(ref GameData data)
    {
        data.baitInventory.Clear();
        foreach (var pair in baitQuantities)
        {
            data.baitInventory[pair.Key.id] = pair.Value;
        }

        var currentBait = FishingManager.Instance.CurrentBait;
        if (currentBait != null)
            data.currentBaitID = currentBait.id;
        else
            data.currentBaitID = "";
    }

    public void LoadData(GameData data)
    {
        baitQuantities.Clear();

        foreach (var pair in data.baitInventory)
        {
            var bait = FishingBaitUI.Instance.allBaits.Find(b => b.id == pair.Key);
            if (bait != null)
                baitQuantities[bait] = pair.Value;
        }

        if (!string.IsNullOrEmpty(data.currentBaitID))
        {
            var selectedBait = FishingBaitUI.Instance.allBaits.Find(b => b.id == data.currentBaitID);
            if (selectedBait != null)
            {
                FishingBaitUI.Instance.SetCurrentBait(selectedBait);
                FishingManager.Instance.SetBaitBonus(selectedBait);
            }
        }

        FishingBaitUI.Instance?.RefreshUI();
    }
}
