using System.Collections.Generic;
using UnityEngine;

public class FishInventory : Singleton<FishInventory>
{
    [Header("References")]
    public Transform slotParent;
    public GameObject slotPrefab;

    private Dictionary<FishData, int> fishCollection = new();

    // Thêm cá vào kho
    public void AddFish(FishData fish)
    {
        if (fishCollection.ContainsKey(fish))
            fishCollection[fish]++;
        else
            fishCollection[fish] = 1;

        RefreshUI();
    }

    // Loại bỏ một số lượng cá nhất định
    public void RemoveFish(FishData fish, int amount)
    {
        if (!fishCollection.ContainsKey(fish)) return;

        fishCollection[fish] -= amount;

        if (fishCollection[fish] <= 0)
            fishCollection.Remove(fish);

        RefreshUI();
    }

    // Lấy số lượng cá hiện có
    public int GetFishQuantity(FishData fish)
    {
        if (fishCollection.TryGetValue(fish, out int qty))
            return qty;

        return 0;
    }

    // Làm mới UI hiển thị kho cá
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
}
