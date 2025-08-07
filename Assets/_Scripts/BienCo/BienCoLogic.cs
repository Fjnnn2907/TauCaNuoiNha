using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
                int randomQty = Random.Range(1, 11); // 1 đến 10

                bcm.lastAddedBaits.Clear();
                bcm.lastAddedBaits.Add((randomBait.bait, randomQty));

                // Gán lại cho bienCo để xử lý chính xác
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

            for (int i = 0; i < numToRemove; i++)
            {
                var fishEff = validFish[i];
                int owned = FishInventory.Instance.GetFishQuantity(fishEff.fish);
                int qty = Random.Range(1, Mathf.Min(owned, fishEff.quantity) + 1);
                bcm.lastAffectedFish.Add((fishEff.fish, qty));
            }
        }
    }


    public static void XuLyBienCo(BienCoSO bienCo)
    {
        var bcm = BienCoManager.Instance;

        switch (bienCo.loaiBienCo)
        {
            case bienCoType.TruTien:
                CoinManager.Instance.SpendCoins(bienCo.giaTriTien);
                break;

            case bienCoType.CongTien:
                CoinManager.Instance.AddCoins(bienCo.giaTriTien);
                break;

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
                {
                    FishInventory.Instance.RemoveFish(fishInfo.fish, fishInfo.quantity);
                    CoinManager.Instance.AddCoins(fishInfo.quantity * fishInfo.fish.sellPrice);
                }
                break;
        }
    }
}