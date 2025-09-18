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

        // Nếu hết mồi hiện tại thì tự động đổi sang mồi khác
        if (baitQuantities[bait] <= 0 && FishingManager.Instance.CurrentBait == bait)
        {
            AutoSwitchBait();
        }

        return true;
    }

    private void AutoSwitchBait()
    {
        foreach (var newBait in FishingBaitUI.Instance.allBaits)
        {
            if (GetQuantity(newBait) > 0)
            {
                FishingBaitUI.Instance.SetCurrentBait(newBait);
                FishingManager.Instance.SetBaitBonus(newBait);
                Debug.Log($"🔄 Tự động đổi sang mồi: {newBait.baitName}");
                return;
            }
        }

        // Nếu không còn mồi nào
        FishingBaitUI.Instance.SetCurrentBait(null);
        FishingManager.Instance.SetBaitBonus(null);
        Debug.Log("❌ Hết sạch mồi, không thể đổi!");
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
