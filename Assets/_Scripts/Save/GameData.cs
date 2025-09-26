using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int currentCoins = 9999;
    public List<string> discoveredFishIDs = new();
    public bool hasShownAllFishPanel = false;

    public SerializableDirectory<string, int> fishInventory = new();

    public List<string> ownedRodIDs = new();    
    public string currentRodID = "";             

    public SerializableDirectory<string, int> baitInventory = new();
    public string currentBaitID = "";
    public string currentSceneName;
    public string targetSceneName;

    public string questFishName;
    public FishRarity questRarity;
    public int questAmount;
    public int questProgress;
    public int questReward;
    public int questsCompleted;

    public int zoneQuestLimit;
    public int zoneLockedQuestCount;

    //public float savedMusicVolume = .3f;
    //public float savedSFXVolume = .3f;
}
