using UnityEngine;

[CreateAssetMenu(menuName = "Fishing/Fishing Rod")]
public class FishingRodData : ScriptableObject
{
    public string id;
    public string rodName;
    public Sprite icon;
    public int price;
    public float bonusRareRate;
    public float bonusLegendaryRate;
}
