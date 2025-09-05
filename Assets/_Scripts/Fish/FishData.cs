using UnityEngine;

[CreateAssetMenu(fileName = "FishData", menuName = "Fishing/FishData")]
public class FishData : ScriptableObject
{
    public string fishID;
    public string fishName;
    public Sprite sprite;
    public Sprite shadowSprite;
    [TextArea] public string description;
    public FishRarity rarity;
    public int sellPrice = 10;
    public string zone;

    public bool isUnique; // ✅ Cá chỉ câu được 1 lần

    public bool isSpecial; // ✅ Là cá đặc biệt (trigger event)
    public string specialEventID; // ✅ Tên hoặc ID event để kích hoạt

    public bool isNotSellable;
}
