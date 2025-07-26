using System.Collections.Generic;
using UnityEngine;

public class FishingInventory : Singleton<FishingInventory>, ISaveable
{
    public List<FishingRodData> ownedRods = new();

    private void Start()
    {
        SaveManager.Instance.RegisterSaveable(this);
    }

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

    // ================= SAVE / LOAD =================

    public void SaveData(ref GameData data)
    {
        data.ownedRodIDs.Clear();
        foreach (var rod in ownedRods)
        {
            data.ownedRodIDs.Add(rod.id);
        }

        if (FishingManager.Instance.CurrentRod != null)
        {
            data.currentRodID = FishingManager.Instance.CurrentRod.id;
        }
        else
        {
            data.currentRodID = "";
        }
    }

    public void LoadData(GameData data)
    {
        ownedRods.Clear();

        foreach (var id in data.ownedRodIDs)
        {
            FishingRodData rod = FishingRodUI.Instance.allRods.Find(r => r.id == id);
            if (rod != null)
            {
                ownedRods.Add(rod);
            }
        }

        if (!string.IsNullOrEmpty(data.currentRodID))
        {
            FishingRodData selectedRod = FishingRodUI.Instance.allRods.Find(r => r.id == data.currentRodID);
            if (selectedRod != null)
            {
                FishingManager.Instance.CurrentRod = selectedRod;
                FishingManager.Instance.SetRodBonus(selectedRod);
                FishingRodUI.Instance?.UpdateSelectedRodIcon(selectedRod.icon);
            }
        }

        FishingRodUI.Instance?.RefreshUI();
    }
}
