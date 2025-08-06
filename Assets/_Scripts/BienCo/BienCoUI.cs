using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BienCoUI : MonoBehaviour
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
    }

    public void OnAcceptBienCo()
    {
        BienCoManager.Instance.XuLyBienCo(bienCoDangHien);
        Close();
    }

    public void Close()
    {
        panel.SetActive(false);
    }

    // ✅ Tạo mô tả phần thưởng hoặc hình phạt
    private string GenerateRewardDescription(BienCoSO bienCo)
    {
        switch (bienCo.loaiBienCo)
        {
            case bienCoType.CongTien:
                return $" Nhận +{bienCo.giaTriTien} vàng";

            case bienCoType.TruTien:
                return $" Mất {bienCo.giaTriTien} vàng";

            case bienCoType.MatCanCau:
                return $" Mất {bienCo.soLuongCanCau} cần câu: {bienCo.rodData?.name}";

            case bienCoType.ThemCanCau:
                return $" Nhận {bienCo.soLuongCanCau} cần câu: {bienCo.rodData?.name}";

            case bienCoType.MatMoiCau:
                return $" Mất {bienCo.soLuongMoiCau} mồi: {bienCo.baitData?.name}";

            case bienCoType.ThemMoiCau:
                return $" Nhận {bienCo.soLuongMoiCau} mồi: {bienCo.baitData?.name}";

            case bienCoType.MatCa:
                return GenerateFishEffectDescription(bienCo, " Mất");

            case bienCoType.DuocThemCa:
                return GenerateFishEffectDescription(bienCo, " Nhận");

            case bienCoType.BanCa:
                return GenerateFishEffectDescription(bienCo, " Bán");

            default:
                return "⚠ Không xác định phần thưởng hoặc hình phạt";
        }
    }

    // ✅ Sinh mô tả từ fishEffects
    private string GenerateFishEffectDescription(BienCoSO bienCo, string prefix)
    {
        if (bienCo.fishEffects == null || bienCo.fishEffects.Count == 0)
            return $"{prefix} cá";

        string result = "";
        foreach (var eff in bienCo.fishEffects)
        {
            if (eff.fish != null && eff.quantity > 0)
            {
                result += $"{prefix} {eff.quantity}x {eff.fish.fishName}, ";
            }
        }
        return result.TrimEnd();
    }
}
