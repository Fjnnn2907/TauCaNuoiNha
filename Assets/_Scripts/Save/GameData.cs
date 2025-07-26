using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<string> discoveredFishIDs = new();

    public SerializableDirectory<string, int> fishInventory = new();
}
