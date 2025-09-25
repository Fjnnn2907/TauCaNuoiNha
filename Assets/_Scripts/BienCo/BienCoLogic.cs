using System.Linq;
using UnityEngine;

public static class BienCoLogic
{
    public static bool IsValid(BienCoSO bienCo)
    {
        switch (bienCo.loaiBienCo)
        {
            case bienCoType.MatCa:
            case bienCoType.BanCa:
                return bienCo.fishEffects.Any(f => f.fish != null && FishInventory.Instance.GetFishQuantity(f.fish) > 0);

            case bienCoType.MatMoiCau:
                return bienCo.baitEffects.Any(b => b.bait != null && BaitInventory.Instance.GetQuantity(b.bait) > 0);

            case bienCoType.MatCanCau:
                return bienCo.rodData != null && FishingInventory.Instance.HasRod(bienCo.rodData);

            case bienCoType.TruTien:
                return CoinManager.Instance.currentCoins >= bienCo.giaTriTien;

            default:
                return true;
        }
    }

    public static void PrepareBienCoData(BienCoSO bienCo)
    {
        var bcm = BienCoManager.Instance;
        bcm.lastCoinChange = 0;

        if (bienCo.loaiBienCo == bienCoType.TruTien || bienCo.loaiBienCo == bienCoType.CongTien)
        {
            bcm.currentRandomFactor = Random.Range(0.8f, 1.2f);
            int actualAmount = Mathf.RoundToInt(bienCo.giaTriTien * bcm.currentRandomFactor);
            bcm.lastCoinChange = bienCo.loaiBienCo == bienCoType.TruTien ? -actualAmount : actualAmount;
        }

        if (bienCo.loaiBienCo == bienCoType.MatMoiCau)
        {
            var validBaits = bienCo.baitEffects
                .Where(b => b.bait != null && BaitInventory.Instance.GetQuantity(b.bait) > 0)
                .OrderBy(x => Random.value)
                .ToList();

            int numToRemove = Mathf.Min(Random.Range(1, 3), validBaits.Count);

            for (int i = 0; i < numToRemove; i++)
            {
                var baitEff = validBaits[i];
                int owned = BaitInventory.Instance.GetQuantity(baitEff.bait);
                int qty = Random.Range(1, Mathf.Min(owned, baitEff.quantity) + 1);
                bcm.lastLostBaits.Add((baitEff.bait, qty));
            }
        }

        if (bienCo.loaiBienCo == bienCoType.ThemMoiCau)
        {
            var validBaits = bienCo.baitEffects
                .Where(b => b.bait != null)
                .OrderBy(x => Random.value)
                .ToList();

            if (validBaits.Count > 0)
            {
                var randomBait = validBaits[0];
                int randomQty = Random.Range(1, 11);

                bcm.lastAddedBaits.Clear();
                bcm.lastAddedBaits.Add((randomBait.bait, randomQty));

                bienCo.baitData = randomBait.bait;
                bienCo.soLuongMoiCau = randomQty;
            }
        }

        if (bienCo.loaiBienCo == bienCoType.MatCa || bienCo.loaiBienCo == bienCoType.BanCa)
        {
            var validFish = bienCo.fishEffects
                .Where(f => f.fish != null && FishInventory.Instance.GetFishQuantity(f.fish) > 0)
                .OrderBy(x => Random.value)
                .ToList();

            int numToRemove = Mathf.Min(Random.Range(1, 3), validFish.Count);
            bcm.lastGoldEarnedFromFish = 0;

            for (int i = 0; i < numToRemove; i++)
            {
                var fishEff = validFish[i];
                int owned = FishInventory.Instance.GetFishQuantity(fishEff.fish);
                int qty = Random.Range(1, Mathf.Min(owned, fishEff.quantity) + 1);
                bcm.lastAffectedFish.Add((fishEff.fish, qty));

                if (bienCo.loaiBienCo == bienCoType.BanCa)
                {
                    bcm.lastGoldEarnedFromFish += qty * fishEff.fish.sellPrice;
                }
            }
        }

        // ✅ THÊM CÁ NGẪU NHIÊN – lưu vào fishEffects
        if (bienCo.loaiBienCo == bienCoType.DuocThemCa)
        {
            bcm.lastAffectedFish.Clear();
            bienCo.fishEffects.Clear();

            var allFish = FishDatabase.Instance.allFish
                .Where(f => f != null)
                .OrderBy(x => Random.value)
                .ToList();

            int fishToAdd = Mathf.Min(Random.Range(1, 4), allFish.Count); // 1–3 loài cá

            for (int i = 0; i < fishToAdd; i++)
            {
                var fish = allFish[i];
                int qty = Random.Range(1, 6); // 1–5 con

                // Lưu vào fishEffects
                bienCo.fishEffects.Add(new FishEffect
                {
                    fish = fish,
                    quantity = qty
                });

                // Lưu vào lastAffectedFish để thực thi
                bcm.lastAffectedFish.Add((fish, qty));
            }
        }
    }

    public static void XuLyBienCo(BienCoSO bienCo)
    {
        var bcm = BienCoManager.Instance;

        switch (bienCo.loaiBienCo)
        {
            case bienCoType.TruTien:
                {
                    int actualAmount = Mathf.RoundToInt(bienCo.giaTriTien * bcm.currentRandomFactor);
                    CoinManager.Instance.SpendCoins(actualAmount);
                    bcm.lastCoinChange = -actualAmount;
                    break;
                }

            case bienCoType.CongTien:
                {
                    int actualAmount = Mathf.RoundToInt(bienCo.giaTriTien * bcm.currentRandomFactor);
                    CoinManager.Instance.AddCoins(actualAmount);
                    bcm.lastCoinChange = actualAmount;
                    break;
                }

            case bienCoType.MatCanCau:
                for (int i = 0; i < bienCo.soLuongCanCau; i++)
                {
                    if (FishingInventory.Instance.HasRod(bienCo.rodData))
                        FishingInventory.Instance.ownedRods.Remove(bienCo.rodData);
                    else break;
                }
                break;

            case bienCoType.ThemCanCau:
                for (int i = 0; i < bienCo.soLuongCanCau; i++)
                    FishingInventory.Instance.AddRod(bienCo.rodData);
                break;

            case bienCoType.MatMoiCau:
                foreach (var baitInfo in bcm.lastLostBaits)
                    BaitInventory.Instance.AddBait(baitInfo.bait, -baitInfo.quantity);
                break;

            case bienCoType.ThemMoiCau:
                BaitInventory.Instance.AddBait(bienCo.baitData, bienCo.soLuongMoiCau);
                break;

            case bienCoType.MatCa:
                foreach (var fishInfo in bcm.lastAffectedFish)
                    FishInventory.Instance.RemoveFish(fishInfo.fish, fishInfo.quantity);
                break;

            case bienCoType.BanCa:
                foreach (var fishInfo in bcm.lastAffectedFish)
                    FishInventory.Instance.RemoveFish(fishInfo.fish, fishInfo.quantity);

                CoinManager.Instance.AddCoins(bcm.lastGoldEarnedFromFish);
                break;

            case bienCoType.DuocThemCa:
                foreach (var fishInfo in bcm.lastAffectedFish)
                    FishInventory.Instance.AddFishMultiple(fishInfo.fish, fishInfo.quantity);
                break;
        }
    }
}
