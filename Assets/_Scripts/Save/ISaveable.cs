using UnityEngine;

public interface ISaveable
{
    public void SaveData(ref GameData data);
    public void LoadData(GameData data);
}
