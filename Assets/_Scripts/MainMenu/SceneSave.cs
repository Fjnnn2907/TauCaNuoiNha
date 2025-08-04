using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSave : MonoBehaviour, ISaveable
{
    private void Awake()
    {
        SaveManager.Instance?.RegisterSaveable(this);
    }

    private void OnDestroy()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance?.UnregisterSaveable(this);
    }

    public void SaveData(ref GameData data)
    {
        data.currentSceneName = SceneManager.GetActiveScene().name;
    }

    public void LoadData(GameData data)
    {
        // Không cần load lại scene ở đây.
    }
}
