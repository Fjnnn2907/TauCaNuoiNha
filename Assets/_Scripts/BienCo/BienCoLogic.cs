using System.Collections.Generic;
using UnityEngine;

public static class BienCoLogic
{
    public static bool IsValid(BienCoSO bienCo)
    {
        switch (bienCo.loaiBienCo)
        {
            case bienCoType.MatCa:
                return FishInventory.Instance.HasAnyFish();

            case bienCoType.BanCa:
                return FishInventory.Instance.HasAnyFish();

            case bienCoType.MatMoiCau:
                foreach (var eff in bienCo.baitEffects)
                {
                    if (BaitInventory.Instance.GetQuantity(eff.bait) > 0)
                        return true;
                }
                return false;

            case bienCoType.MatCanCau:
                return FishingInventory.Instance.HasRod(bienCo.rodData);

            case bienCoType.TruTien:
                return CoinManager.Instance.currentCoins >= bienCo.giaTriTien;

            default:
                return true;
        }
    }

    public static void XuLyBienCo(BienCoSO bienCo)
    {
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
                {
                    FishingInventory.Instance.AddRod(bienCo.rodData);
                }
                break;

            case bienCoType.MatMoiCau:
                RandomMatMoiCau(bienCo);
                break;

            case bienCoType.ThemMoiCau:
                BaitInventory.Instance.AddBait(bienCo.baitData, bienCo.soLuongMoiCau);
                break;

            case bienCoType.MatCa:
                RandomMatCa(bienCo);
                break;

            case bienCoType.DuocThemCa:
                foreach (var fishEff in bienCo.fishEffects)
                {
                    for (int i = 0; i < fishEff.quantity; i++)
                        FishInventory.Instance.AddFish(fishEff.fish);
                }
                break;

            case bienCoType.BanCa:
                RandomBanCa(bienCo);
                break;

            default:
                Debug.LogWarning("Loại biến cố chưa xử lý: " + bienCo.loaiBienCo);
                break;
        }
    }

    private static void RandomMatMoiCau(BienCoSO bienCo)
    {
        List<FishingBaitData> moiCo = new();
        foreach (var eff in bienCo.baitEffects)
        {
            if (BaitInventory.Instance.GetQuantity(eff.bait) > 0)
                moiCo.Add(eff.bait);
        }

        if (moiCo.Count == 0) return;

        int soLoaiMat = Random.Range(1, Mathf.Min(moiCo.Count + 1, 4));

        for (int i = 0; i < soLoaiMat; i++)
        {
            if (moiCo.Count == 0) break;

            FishingBaitData bait = moiCo[Random.Range(0, moiCo.Count)];
            int slHienTai = BaitInventory.Instance.GetQuantity(bait);
            int soLuongMat = Random.Range(1, slHienTai + 1);
            BaitInventory.Instance.AddBait(bait, -soLuongMat);
            moiCo.Remove(bait);
        }
    }

    private static void RandomMatCa(BienCoSO bienCo)
    {
        var dsCaCo = FishInventory.Instance.GetAllOwnedFish();
        if (dsCaCo.Count == 0) return;

        int soLoaiMat = Random.Range(1, Mathf.Min(dsCaCo.Count + 1, 4));

        for (int i = 0; i < soLoaiMat; i++)
        {
            if (dsCaCo.Count == 0) break;

            FishData ca = dsCaCo[Random.Range(0, dsCaCo.Count)];
            int sl = FishInventory.Instance.GetFishQuantity(ca);
            int slMat = Random.Range(1, sl + 1);
            FishInventory.Instance.RemoveFish(ca, slMat);
            dsCaCo.Remove(ca);
        }
    }

    private static void RandomBanCa(BienCoSO bienCo)
    {
        var dsCaCo = FishInventory.Instance.GetAllOwnedFish();
        if (dsCaCo.Count == 0) return;

        int soLoaiBan = Random.Range(1, Mathf.Min(dsCaCo.Count + 1, 4));

        for (int i = 0; i < soLoaiBan; i++)
        {
            if (dsCaCo.Count == 0) break;

            FishData ca = dsCaCo[Random.Range(0, dsCaCo.Count)];
            int slCo = FishInventory.Instance.GetFishQuantity(ca);
            int slBan = Random.Range(1, slCo + 1);

            float tyLeGia = Random.Range(0.8f, 1.2f);
            int tongTien = Mathf.RoundToInt(slBan * ca.sellPrice * tyLeGia);

            FishInventory.Instance.RemoveFish(ca, slBan);
            CoinManager.Instance.AddCoins(tongTien);

            dsCaCo.Remove(ca);
        }
    }
}
