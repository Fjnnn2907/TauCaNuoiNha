using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Thêm namespace TMPro

public class AuditionManager : Singleton<AuditionManager>
{
    public PlayerInputChecker inputChecker;
    public Transform arrowContainer;
    public GameObject arrowPrefab;

    public Sprite upSprite, downSprite, leftSprite, rightSprite;
    public Sprite pressedUpSprite, pressedDownSprite, pressedLeftSprite, pressedRightSprite;

    private List<Image> arrowImages = new List<Image>();
    private List<ArrowDirection> currentDirections = new List<ArrowDirection>();
    private DifficultyLevel currentDifficulty = DifficultyLevel.Easy;
    public int sequenceLength = 5;

    public Slider timerSlider;
    private float timeLimit = 10f;
    private Coroutine countdownCoroutine;

    [Header("UI Elements")]
    public TextMeshProUGUI timerText; // Thêm TextMeshPro để hiển thị thời gian

    //private void OnEnable()
    //{
    //    StartSequence(inputChecker);
    //}

    public void SetDifficulty(DifficultyLevel difficulty)
    {
        currentDifficulty = difficulty;

        switch (difficulty)
        {
            case DifficultyLevel.Easy:
                sequenceLength = 5;
                timeLimit = 10f;
                break;
            case DifficultyLevel.Medium:
                sequenceLength = 9;
                timeLimit = 8f;
                break;
            case DifficultyLevel.Hard:
                sequenceLength = 15;
                timeLimit = 6f;
                break;
        }

        StartSequence(inputChecker);
        Debug.Log(difficulty);
    }

    public void StartSequence(PlayerInputChecker inputChecker)
    {
        timerSlider.maxValue = timeLimit;
        timerSlider.value = timeLimit;

        var sequence = GenerateAndDisplaySequence();
        inputChecker.ui = this;
        inputChecker.SetSequence(sequence);

        // Stop any existing timer
        if (countdownCoroutine != null)
            StopCoroutine(countdownCoroutine);

        // Start countdown timer
        countdownCoroutine = StartCoroutine(CountdownTimer());

        inputChecker.OnSequenceEnd = (success) =>
        {
            if (countdownCoroutine != null)
                StopCoroutine(countdownCoroutine);

            if (success)
                WinGame();
            else
                LoseGame();
        };
    }

    private IEnumerator CountdownTimer()
    {
        float timer = timeLimit;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            timerSlider.value = timer;
            UpdateTimerText(timer); // Cập nhật text thời gian
            yield return null;
        }

        timerSlider.value = 0;
        UpdateTimerText(0); // Cập nhật text khi hết thời gian
        Debug.Log("⏰ Hết thời gian!");
        LoseGame();
    }

    // Phương thức cập nhật text thời gian
    private void UpdateTimerText(float currentTime)
    {
        if (timerText != null)
        {
            // Hiển thị thời gian với 1 chữ số thập phân
            timerText.text = $"{currentTime:F1}s";

            // Đổi màu khi thời gian sắp hết (dưới 3 giây)
            if (currentTime <= 3f)
                timerText.color = Color.yellow;
            else
                timerText.color = Color.white;
        }
    }

    public List<ArrowDirection> GenerateAndDisplaySequence()
    {
        List<ArrowDirection> sequence = ArrowSequenceGenerator.GenerateSequence(sequenceLength);
        DisplaySequence(sequence);
        return sequence;
    }

    public void DisplaySequence(List<ArrowDirection> sequence)
    {
        foreach (Transform child in arrowContainer)
            Destroy(child.gameObject);

        arrowImages.Clear();
        currentDirections.Clear();

        foreach (var dir in sequence)
        {
            GameObject arrow = Instantiate(arrowPrefab, arrowContainer);
            Image img = arrow.GetComponent<Image>();

            switch (dir)
            {
                case ArrowDirection.Up: img.sprite = upSprite; break;
                case ArrowDirection.Down: img.sprite = downSprite; break;
                case ArrowDirection.Left: img.sprite = leftSprite; break;
                case ArrowDirection.Right: img.sprite = rightSprite; break;
            }

            arrowImages.Add(img);
            currentDirections.Add(dir);
        }
    }

    public void HighlightArrowAt(int index, ArrowDirection dir)
    {
        if (index < 0 || index >= arrowImages.Count) return;

        var img = arrowImages[index];

        switch (dir)
        {
            case ArrowDirection.Up: img.sprite = pressedUpSprite; break;
            case ArrowDirection.Down: img.sprite = pressedDownSprite; break;
            case ArrowDirection.Left: img.sprite = pressedLeftSprite; break;
            case ArrowDirection.Right: img.sprite = pressedRightSprite; break;
        }
    }

    public void ResetArrowSprites()
    {
        for (int i = 0; i < arrowImages.Count; i++)
        {
            switch (currentDirections[i])
            {
                case ArrowDirection.Up: arrowImages[i].sprite = upSprite; break;
                case ArrowDirection.Down: arrowImages[i].sprite = downSprite; break;
                case ArrowDirection.Left: arrowImages[i].sprite = leftSprite; break;
                case ArrowDirection.Right: arrowImages[i].sprite = rightSprite; break;
            }
        }
    }

    public void PlayWrongInputFlash()
    {
        StartCoroutine(FlashRedCoroutine());
    }

    private IEnumerator FlashRedCoroutine()
    {
        int flashCount = 2;
        float flashDuration = 0.2f;

        for (int i = 0; i < flashCount; i++)
        {
            foreach (var img in arrowImages)
                img.color = Color.red;

            yield return new WaitForSeconds(flashDuration);

            foreach (var img in arrowImages)
                img.color = Color.white;

            yield return new WaitForSeconds(flashDuration);
        }
    }

    private void WinGame()
    {
        FishingManager.Instance.OnMinigameWin();
    }

    private void LoseGame()
    {
        FishingManager.Instance.OnMinigameLose();
    }
}