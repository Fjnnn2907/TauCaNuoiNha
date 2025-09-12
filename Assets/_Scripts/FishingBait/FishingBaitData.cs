using UnityEngine;

[CreateAssetMenu(menuName = "Fishing/Fishing Bait")]
public class FishingBaitData : ScriptableObject
{
    public string id;
    public string baitName;
    public Sprite icon;
    public int quantityRequired = 1;

    public float bonusRareRate;
    public float bonusLegendaryRate;

    public int price = 10;

    public string GetBaitName()
    {
        return LanguageManager.Instance.GetText(id);
    }
}
