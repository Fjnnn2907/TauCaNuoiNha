using System.Collections.Generic;
using UnityEngine;

public class FishInventory : Singleton<FishInventory>, ISaveable
{
    [Header("References")]
    public Transform slotParent;
    public GameObject slotPrefab;

    private Dictionary<FishData, int> fishCollection = new();

    private void Start()
    {
        SaveManager.Instance.RegisterSaveable(this);
    }

    public void AddFish(FishData fish)
    {
        if (fishCollection.ContainsKey(fish))
            fishCollection[fish]++;
        else
            fishCollection[fish] = 1;

        RefreshUI();
    }

    public void RemoveFish(FishData fish, int amount)
    {
        if (!fishCollection.ContainsKey(fish)) return;

        fishCollection[fish] -= amount;
        if (fishCollection[fish] <= 0)
            fishCollection.Remove(fish);

        RefreshUI();
    }

    public int GetFishQuantity(FishData fish)
    {
        if (fishCollection.TryGetValue(fish, out int qty))
            return qty;

        return 0;
    }

    public void RefreshUI()
    {
        foreach (Transform child in slotParent)
            Destroy(child.gameObject);

        foreach (var pair in fishCollection)
        {
            var go = Instantiate(slotPrefab, slotParent);
            go.GetComponent<FishInventorySlot>().Setup(pair.Key, pair.Value);
        }
    }

    // ====================================
    // SAVE / LOAD
    // ====================================

    public void SaveData(ref GameData data)
    {
        data.fishInventory.Clear();

        foreach (var pair in fishCollection)
        {
            string fishID = pair.Key.fishID;
            int quantity = pair.Value;
            data.fishInventory[fishID] = quantity;
        }
    }

    public void LoadData(GameData data)
    {
        fishCollection.Clear();

        foreach (var pair in data.fishInventory)
        {
            FishData fish = FishDatabase.Instance.allFish.Find(f => f.fishID == pair.Key);
            if (fish != null)
            {
                fishCollection[fish] = pair.Value;
            }
        }

        RefreshUI();
    }
}
