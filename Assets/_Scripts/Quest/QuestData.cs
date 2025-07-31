using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Fishing/QuestData")]
public class QuestData : ScriptableObject
{
    public string questID;
    public string description;
    public FishRarity requiredRarity;
    public string requiredFishName;
    public int requiredAmount;
    public int rewardGold;
}
