using UnityEngine;

public static class BienCoLogic
{
    public static bool IsValid(BienCoSO bienCo)
    {
        switch (bienCo.loaiBienCo)
        {
            case bienCoType.MatCa:
            case bienCoType.BanCa:
                foreach (var fishEff in bienCo.fishEffects)
                {
                    if (FishInventory.Instance.GetFishQuantity(fishEff.fish) < fishEff.quantity)
                        return false;
                }
                return true;

            case bienCoType.MatCanCau:
                return FishingInventory.Instance.HasRod(bienCo.rodData);

            case bienCoType.MatMoiCau:
                return BaitInventory.Instance.GetQuantity(bienCo.baitData) >= bienCo.soLuongMoiCau;

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
                }
                break;

            case bienCoType.ThemCanCau:
                for (int i = 0; i < bienCo.soLuongCanCau; i++)
                {
                    FishingInventory.Instance.AddRod(bienCo.rodData);
                }
                break;

            case bienCoType.MatMoiCau:
                BaitInventory.Instance.AddBait(bienCo.baitData, -bienCo.soLuongMoiCau);
                break;

            case bienCoType.ThemMoiCau:
                BaitInventory.Instance.AddBait(bienCo.baitData, bienCo.soLuongMoiCau);
                break;

            case bienCoType.MatCa:
                foreach (var fishEff in bienCo.fishEffects)
                {
                    FishInventory.Instance.RemoveFish(fishEff.fish, fishEff.quantity);
                }
                break;

            case bienCoType.DuocThemCa:
                foreach (var fishEff in bienCo.fishEffects)
                {
                    for (int i = 0; i < fishEff.quantity; i++)
                        FishInventory.Instance.AddFish(fishEff.fish);
                }
                break;

            case bienCoType.BanCa:
                foreach (var fishEff in bienCo.fishEffects)
                {
                    int qty = FishInventory.Instance.GetFishQuantity(fishEff.fish);
                    int sellQty = Mathf.Min(fishEff.quantity, qty);
                    if (sellQty > 0)
                    {
                        FishInventory.Instance.RemoveFish(fishEff.fish, sellQty);
                        CoinManager.Instance.AddCoins(sellQty * fishEff.fish.sellPrice);
                    }
                }
                break;

            default:
                Debug.LogWarning("Loại biến cố chưa xử lý: " + bienCo.loaiBienCo);
                break;
        }
    }
}
