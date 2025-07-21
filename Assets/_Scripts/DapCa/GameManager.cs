using UnityEngine;

public class GameManager : MonoBehaviour
{
    public KeySpawner keySpawner;

    [Header("Chọn độ khó bằng string: easy, medium, hard")]
    public string difficulty = "easy";

    void Start()
    {
        keySpawner.SetDifficulty(difficulty);
    }
}
