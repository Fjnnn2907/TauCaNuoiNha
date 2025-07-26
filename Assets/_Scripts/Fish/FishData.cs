using UnityEngine;

[CreateAssetMenu(fileName = "FishData", menuName = "Fishing/FishData")]
public class FishData : ScriptableObject
{
    public string fishName;
    public Sprite sprite;
    public Sprite shadowSprite;
    [TextArea] public string description;
    public FishRarity rarity;
    public int sellPrice = 10;
}