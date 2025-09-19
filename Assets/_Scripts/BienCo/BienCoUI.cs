using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BienCoUI : Singleton<BienCoUI>
{
    [Header("UI References")]
    public GameObject panel;
    public TextMeshProUGUI tenBienCoText;
    public TextMeshProUGUI moTaText;
    public TextMeshProUGUI moTaPhanThuongText;
    public Button acceptButton;
    public Image icon;

    private BienCoSO bienCoDangHien;

    private void Start()
    {
        acceptButton.onClick.AddListener(OnAcceptBienCo);
    }

    public void ShowBienCo(BienCoSO bienCo)
    {
        bienCoDangHien = bienCo;

        // 🔥 Lấy text từ key nếu có, fallback về text gốc
        if (!string.IsNullOrEmpty(bienCo.tenBienCoKey))
            tenBienCoText.text = LanguageManager.Instance.GetText(bienCo.tenBienCoKey);
        else
            tenBienCoText.text = bienCo.tenBienCo;

        if (!string.IsNullOrEmpty(bienCo.moTaBienCoKey))
            moTaText.text = LanguageManager.Instance.GetText(bienCo.moTaBienCoKey);
        else
            moTaText.text = bienCo.moTaBienCo;

        moTaPhanThuongText.text = GenerateRewardDescription(bienCo);

        if (bienCo.iconBienCo != null)
            icon.sprite = bienCo.iconBienCo;

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
                return string.Format(
                    LanguageManager.Instance.GetText("thuong_them_tien"),
                    BienCoManager.Instance.lastCoinChange
                );

            case bienCoType.TruTien:
                return string.Format(
                    LanguageManager.Instance.GetText("thuong_mat_tien"),
                    Mathf.Abs(BienCoManager.Instance.lastCoinChange)
                );

            case bienCoType.MatCanCau:
                return string.Format(
                    LanguageManager.Instance.GetText("thuong_mat_cancau"),
                    bienCo.soLuongCanCau,
                    bienCo.rodData != null ? bienCo.rodData.name : "?"
                );

            case bienCoType.ThemCanCau:
                return string.Format(
                    LanguageManager.Instance.GetText("thuong_them_cancau"),
                    bienCo.soLuongCanCau,
                    bienCo.rodData != null ? bienCo.rodData.name : "?"
                );

            case bienCoType.MatMoiCau:
                return GenerateBaitDescription("thuong_mat_moicau", BienCoManager.Instance.lastLostBaits);

            case bienCoType.ThemMoiCau:
                return GenerateBaitDescription("thuong_them_moicau", BienCoManager.Instance.lastAddedBaits);

            case bienCoType.MatCa:
                return GenerateFishEffectDescription("thuong_mat_ca");

            case bienCoType.BanCa:
                return GenerateFishEffectDescription("thuong_ban_ca");

            case bienCoType.DuocThemCa:
                return GenerateFishEffectDescription("thuong_them_ca");

            default:
                return LanguageManager.Instance.GetText("thuong_khong_xac_dinh");
        }
    }

    private string GenerateBaitDescription(string key, List<(FishingBaitData bait, int quantity)> baitList)
    {
        if (baitList == null || baitList.Count == 0)
            return string.Format(LanguageManager.Instance.GetText(key), "mồi câu", 0);

        string result = "";
        foreach (var baitInfo in baitList)
        {
            if (baitInfo.bait != null && baitInfo.quantity > 0)
            {
                string baitName = !string.IsNullOrEmpty(baitInfo.bait.GetBaitName())
                    ? LanguageManager.Instance.GetText(baitInfo.bait.GetBaitName())
                    : baitInfo.bait.baitName;

                result += string.Format(LanguageManager.Instance.GetText(key), baitName, baitInfo.quantity) + ", ";
            }
        }
        return result.TrimEnd(' ', ',');
    }

    private string GenerateFishEffectDescription(string key)
    {
        var fishList = BienCoManager.Instance.lastAffectedFish;
        if (fishList == null || fishList.Count == 0)
            return string.Format(LanguageManager.Instance.GetText(key), "cá", 0);

        string result = "";
        foreach (var info in fishList)
        {
            if (info.fish != null && info.quantity > 0)
            {
                string fishName = !string.IsNullOrEmpty(info.fish.nameKey)
                    ? LanguageManager.Instance.GetText(info.fish.nameKey)
                    : info.fish.fishName;

                if (key == "thuong_ban_ca")
                {
                    int gold = BienCoManager.Instance.lastGoldEarnedFromFish;
                    result += string.Format(LanguageManager.Instance.GetText(key), fishName, info.quantity, gold) + ", ";
                }
                else
                {
                    result += string.Format(LanguageManager.Instance.GetText(key), fishName, info.quantity) + ", ";
                }
            }
        }

        return result.TrimEnd(' ', ',');
    }
}
