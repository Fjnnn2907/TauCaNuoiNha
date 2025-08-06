using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FishEffect
{
    public FishData fish;
    public int quantity;
}

[System.Serializable]
public class BaitEffect
{
    public FishingBaitData bait;
    public int quantity;
}

[CreateAssetMenu(fileName = "BienCoData", menuName = "BienCo")]
public class BienCoSO : ScriptableObject
{
    public string tenBienCo;
    [TextArea(2, 5)] public string moTaBienCo;
    public Sprite iconBienCo;
    public bienCoType loaiBienCo;

    public float xacSuatXuatHien = 1f;

    public int giaTriTien;

    public FishingRodData rodData;
    public int soLuongCanCau;

    public FishingBaitData baitData;
    public int soLuongMoiCau;

    public List<FishEffect> fishEffects = new();
    public List<BaitEffect> baitEffects = new();
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
