using System.Collections.Generic;

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
        if (!HasBait(bait)) 
            return false; 
        
        baitQuantities[bait] -= bait.quantityRequired; 
        return true; 
    }

    public void CheckAndAutoSwitchBait()
    {
        var currentBait = FishingManager.Instance?.CurrentBait;
        if (currentBait == null || GetQuantity(currentBait) > 0)
            return;

        var ui = FishingBaitUI.Instance;
        if (ui == null)
        {
            FishingManager.Instance?.SetBaitBonus(null);
            return;
        }

        // Tìm mồi mới còn trong túi
        FishingBaitData nextBait = null;
        foreach (var newBait in ui.allBaits)
        {
            if (GetQuantity(newBait) > 0)
            {
                nextBait = newBait;
                break;
            }
        }

        if (nextBait != null)
        {
            // 🔄 Còn mồi khác => tự động đổi
            ui.SetCurrentBait(nextBait);
            FishingManager.Instance.SetBaitBonus(nextBait);

            NotificationManager.Instance?.ShowNotification(
                string.Format(LanguageManager.Instance.GetText("thongbao_doi_moi"),
                LanguageManager.Instance.GetText(nextBait.baitName))
            );
        }
        else
        {
            // ❌ Không còn mồi nào => reset + thông báo hết mồi
            ui.SetCurrentBait(null);
            FishingManager.Instance.SetBaitBonus(null);

            NotificationManager.Instance?.ShowNotification(
                LanguageManager.Instance.GetText("thongbao_het_moi")
            );
        }
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
