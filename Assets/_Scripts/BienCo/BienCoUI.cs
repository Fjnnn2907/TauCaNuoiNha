using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BienCoUI : Singleton<BienCoUI>
{
    [Header("UI References")]
    public GameObject panel;
    public TextMeshProUGUI tenBienCoText;
    public TextMeshProUGUI moTaText;
    public TextMeshProUGUI moTaPhanThuongText;
    public Button acceptButton;

    private BienCoSO bienCoDangHien;

    private void Start()
    {
        acceptButton.onClick.AddListener(OnAcceptBienCo);
    }

    public void ShowBienCo(BienCoSO bienCo)
    {
        bienCoDangHien = bienCo;

        tenBienCoText.text = bienCo.tenBienCo;
        moTaText.text = bienCo.moTaBienCo;
        moTaPhanThuongText.text = GenerateRewardDescription(bienCo);

        panel.SetActive(true);
        Time.timeScale = 0;
    }

    public void OnAcceptBienCo()
    {
        BienCoManager.Instance.XuLyBienCo(bienCoDangHien);
        Close();
        Time.timeScale = 1;
    }

    public void Close()
    {
        panel.SetActive(false);
    }

    private string GenerateRewardDescription(BienCoSO bienCo)
    {
        switch (bienCo.loaiBienCo)
        {
            case bienCoType.CongTien:
                return $"Nhận {BienCoManager.Instance.lastCoinChange} vàng";

            case bienCoType.TruTien:
                return $"Mất {Mathf.Abs(BienCoManager.Instance.lastCoinChange)} vàng";

            case bienCoType.MatCanCau:
                return $"Mất {bienCo.soLuongCanCau} cần câu: {bienCo.rodData?.name}";

            case bienCoType.ThemCanCau:
                return $"Nhận {bienCo.soLuongCanCau} cần câu: {bienCo.rodData?.name}";

            case bienCoType.MatMoiCau:
                return GenerateLostBaitDescription();

            case bienCoType.ThemMoiCau:
                return GenerateMoreBaitDescription();

            case bienCoType.MatCa:
                return GenerateFishEffectDescription("Mất");

            case bienCoType.BanCa:
                return GenerateFishEffectDescription("Bán");
            case bienCoType.DuocThemCa:
                return GenerateFishEffectDescription("Nhận");
            default:
                return "⚠ Không xác định phần thưởng hoặc hình phạt";
        }
    }

    private string GenerateLostBaitDescription()
    {
        var lostBaits = BienCoManager.Instance.lastLostBaits;
        if (lostBaits == null || lostBaits.Count == 0)
            return "Mất mồi câu";

        string result = "Mất ";
        foreach (var baitInfo in lostBaits)
        {
            if (baitInfo.bait != null && baitInfo.quantity > 0)
                result += $"{baitInfo.bait.baitName} x{baitInfo.quantity}, ";
        }
        return result.TrimEnd(' ', ',');
    }

    private string GenerateMoreBaitDescription()
    {
        var moreBaits = BienCoManager.Instance.lastAddedBaits;
        if (moreBaits == null || moreBaits.Count == 0)
            return "Nhận mồi câu";

        string result = "Nhận ";
        foreach (var baitInfo in moreBaits)
        {
            if (baitInfo.bait != null && baitInfo.quantity > 0)
                result += $"{baitInfo.bait.baitName} x{baitInfo.quantity}, ";
        }
        return result.TrimEnd(' ', ',');
    }
    private string GenerateFishEffectDescription(string prefix)
    {
        var fishList = BienCoManager.Instance.lastAffectedFish;
        if (fishList == null || fishList.Count == 0)
            return $"{prefix} cá";

        string result = "";
        foreach (var info in fishList)
        {
            if (info.fish != null && info.quantity > 0)
                result += $"{prefix} {info.fish.fishName} x{info.quantity}, ";
        }

        result = result.TrimEnd(' ', ',');

        if (prefix == "Bán")
        {
            int gold = BienCoManager.Instance.lastGoldEarnedFromFish;
            result += $" với giá {gold} vàng";
        }

        return result;
    }


}
