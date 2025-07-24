using UnityEngine;

[CreateAssetMenu(menuName = "Fishing/Fishing Bait")]
public class FishingBaitData : ScriptableObject
{
    public string baitName;
    public Sprite icon;
    public int quantityRequired = 1;

    public float bonusRareRate;
    public float bonusLegendaryRate;

    public int price = 10;
}
