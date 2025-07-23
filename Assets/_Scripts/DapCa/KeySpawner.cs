using UnityEngine;
using TMPro;
using System.Collections;

public class KeySpawner : MonoBehaviour
{
    public TextMeshProUGUI keyText;
    public float spawnRate = 2f;
    private string[] keys = { "A", "S", "D", "W", "J", "K", "L" };
    public string currentKey = "";

    public void SetDifficulty(DifficultyLevel difficulty)
    {
        switch (difficulty)
        {
            case DifficultyLevel.Easy:
                spawnRate = 2.5f;
                break;
            case DifficultyLevel.Medium:
                spawnRate = 1.5f;
                break;
            case DifficultyLevel.Hard:
                spawnRate = 0.8f;
                break;
        }
        Debug.Log(difficulty.ToString());
    }

    public void SetDifficulty(string difficulty)
    {
        switch (difficulty.ToLower())
        {
            case "easy":
                SetDifficulty(DifficultyLevel.Easy);
                break;
            case "medium":
                SetDifficulty(DifficultyLevel.Medium);
                break;
            case "hard":
                SetDifficulty(DifficultyLevel.Hard);
                break;
        }
    }

    void Start()
    {
        StartCoroutine(SpawnKeys());
    }

    IEnumerator SpawnKeys()
    {
        while (true)
        {
            currentKey = keys[Random.Range(0, keys.Length)];
            keyText.text = currentKey;
            yield return new WaitForSeconds(spawnRate);
        }
    }
}
public enum DifficultyLevel
{
    Easy,
    Medium,
    Hard
}
