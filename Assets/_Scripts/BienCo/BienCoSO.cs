using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BienCoData", menuName = "BienCo")]
public class BienCoSO : ScriptableObject
{
    public string tenBienCo;
    [TextArea(2, 5)] public string moTaBienCo;
    public Sprite iconBienCo;
    public bienCoType loaiBienCo;

    [Range(0f, 1f)] public float xacSuatXuatHien = 1f;

    public int giaTriTien;

    public FishingRodData rodData;
    public int soLuongCanCau;

    public FishingBaitData baitData;
    public int soLuongMoiCau;

    public List<FishEffect> fishEffects = new List<FishEffect>();
}

public enum bienCoType
{
    TruTien,
    CongTien,
    MatCanCau,
    ThemCanCau,
    MatMoiCau,
    ThemMoiCau,
    BanCa,
    MatCa,
    DuocThemCa
}
[System.Serializable]
public class FishEffect
{
    public FishData fish;
    public int quantity;
}
