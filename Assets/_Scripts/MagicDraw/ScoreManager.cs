using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScoreManager : Singleton<ScoreManager>
{
    [Header("UI References")]
    public Slider scoreSlider;
    public TMP_Text scoreText;
    public TMP_Text timeText;
    public TMP_Text difficultyText;

    [Header("Score Settings")]
    public int pointsPerCorrectAnswer = 20;
    public int pointsDeductionPerWrong = 5; // Điểm trừ khi vẽ sai
    public int maxScore = 100;

    [Header("Time Settings")]
    public float easyTime = 60f; // 2 phút cho Easy
    public float mediumTime = 30f; // 1.5 phút cho Medium  
    public float hardTime = 15f; // 1 phút cho Hard

    private int currentScore = 0;
    private float remainingTime;
    private DifficultyLevel currentDifficulty = DifficultyLevel.Easy;
    private bool isGameActive = false;
    private Coroutine timeCoroutine;

    // Events để game chính có thể lắng nghe
    public System.Action OnGameWin;
    public System.Action OnGameLose;
    public System.Action OnTimeChanged;
    public System.Action<int> OnScoreChanged; // Truyền điểm hiện tại

    private void Start()
    {
        // Khởi tạo slider
        if (scoreSlider != null)
        {
            scoreSlider.minValue = 0;
            scoreSlider.maxValue = maxScore;
            scoreSlider.value = currentScore;
        }
        //UpdateScoreDisplay();
        //UpdateTimeDisplay();
        //UpdateDifficultyDisplay();
    }

    // Public method để game chính bắt đầu minigame
    public void StartMinigame(DifficultyLevel difficulty)
    {
        currentDifficulty = difficulty;
        currentScore = 0;
        isGameActive = true;

        switch (difficulty)
        {
            case DifficultyLevel.Easy:
                remainingTime = easyTime;
                break;
            case DifficultyLevel.Medium:
                remainingTime = mediumTime;
                break;
            case DifficultyLevel.Hard:
                remainingTime = hardTime;
                break;
        }

        // Cập nhật UI
        if (scoreSlider != null)
        {
            scoreSlider.value = currentScore;
        }

        UpdateScoreDisplay();
        UpdateTimeDisplay();
        UpdateDifficultyDisplay();

        // Bắt đầu đếm ngược
        if (timeCoroutine != null)
            StopCoroutine(timeCoroutine);
        timeCoroutine = StartCoroutine(CountdownTimer());

        Debug.Log($"�� Bắt đầu minigame vẽ pattern với độ khó: {difficulty}");
    }

    // Public method để game chính dừng minigame
    public void StopMinigame()
    {
        isGameActive = false;
        if (timeCoroutine != null)
            StopCoroutine(timeCoroutine);
    }

    // Public method để game chính lấy kết quả
    public MinigameResult GetResult()
    {
        return new MinigameResult
        {
            score = currentScore,
            maxScore = maxScore,
            timeRemaining = remainingTime,
            difficulty = currentDifficulty,
            isWin = currentScore >= maxScore,
            isLose = remainingTime <= 0 && currentScore < maxScore
        };
    }

    public void AddScore()
    {
        if (!isGameActive) return;

        currentScore += pointsPerCorrectAnswer;

        // Giới hạn điểm tối đa
        if (currentScore > maxScore)
        {
            currentScore = maxScore;
        }

        // Cập nhật UI
        if (scoreSlider != null)
        {
            scoreSlider.value = currentScore;
        }

        UpdateScoreDisplay();
        OnScoreChanged?.Invoke(currentScore);

        // Kiểm tra thắng
        if (currentScore >= maxScore)
        {
            OnGameWin?.Invoke();
            GameWin();
        }
    }

    public void DeductScore()
    {
        if (!isGameActive) return;

        currentScore -= pointsDeductionPerWrong;

        // Giới hạn điểm tối thiểu
        if (currentScore < 0)
        {
            currentScore = 0;
        }

        // Cập nhật UI
        if (scoreSlider != null)
        {
            scoreSlider.value = currentScore;
        }

        UpdateScoreDisplay();
        OnScoreChanged?.Invoke(currentScore);
    }

    public void ResetScore()
    {
        currentScore = 0;

        if (scoreSlider != null)
        {
            scoreSlider.value = currentScore;
        }

        UpdateScoreDisplay();
    }

    private IEnumerator CountdownTimer()
    {
        while (remainingTime > 0 && isGameActive)
        {
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
            UpdateTimeDisplay();
            OnTimeChanged?.Invoke();

            // Cảnh báo khi gần hết thời gian
            if (remainingTime <= 10f)
            {
                timeText.color = Color.red;
            }
        }

        // Hết thời gian
        if (remainingTime <= 0 && isGameActive)
        {
            OnGameLose?.Invoke();
            GameLose();
        }
    }

    private void GameWin()
    {
        isGameActive = false;
        if (timeCoroutine != null)
            StopCoroutine(timeCoroutine);

        Debug.Log("🎉 Chúc mừng! Bạn đã thắng minigame vẽ pattern!");
        FishingManager.Instance.OnMinigameWin();
    }

    private void GameLose()
    {
        isGameActive = false;
        if (timeCoroutine != null)
            StopCoroutine(timeCoroutine);

        Debug.Log("😢 Hết thời gian! Bạn đã thua minigame vẽ pattern!");
        FishingManager.Instance.OnMinigameLose();
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Điểm: {currentScore}/{maxScore}";
        }
    }

    private void UpdateTimeDisplay()
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);
            timeText.text = $"Thời gian: {minutes:00}:{seconds:00}";
        }
    }

    private void UpdateDifficultyDisplay()
    {
        if (difficultyText != null)
        {
            string difficultyName = "";
            switch (currentDifficulty)
            {
                case DifficultyLevel.Easy:
                    difficultyName = "Dễ";
                    break;
                case DifficultyLevel.Medium:
                    difficultyName = "Trung bình";
                    break;
                case DifficultyLevel.Hard:
                    difficultyName = "Khó";
                    break;
            }
            difficultyText.text = $"Độ khó: {difficultyName}";
        }
    }

    // Public getters để game chính có thể truy cập
    public int GetCurrentScore() => currentScore;
    public bool IsMaxScoreReached() => currentScore >= maxScore;
    public bool IsGameActive() => isGameActive;
    public float GetRemainingTime() => remainingTime;
    public DifficultyLevel GetCurrentDifficulty() => currentDifficulty;
}

// Struct kết quả minigame để trả về cho game chính
[System.Serializable]
public struct MinigameResult
{
    public int score;
    public int maxScore;
    public float timeRemaining;
    public DifficultyLevel difficulty;
    public bool isWin;
    public bool isLose;
}
