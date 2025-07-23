using System.Collections;
using UnityEngine;

public class FishingManager : Singleton<FishingManager>
{
    [Header("References")]
    public Animator playerAnimator;
    public GameObject rhythmMinigame;
    public FishingSlider fishingSlider;
    public KeySpawner keySpawner;
    public FishingRodData CurrentRod { get; set; }

    [Header("Timing")]
    public float waitTimeMin = 3f;
    public float waitTimeMax = 8f;

    [Header("Bonus Rate Settings")]
    [Range(0, 100)] public float bonusRareRate = 0f;
    [Range(0, 100)] public float bonusLegendaryRate = 0f;

    private FishingState currentState = FishingState.Idle;
    private Coroutine currentCoroutine;
    private FishRarity selectedRarity = FishRarity.Common;

    // ---------------------- ENTRY POINT ------------------------

    public void PrepareCastWithSlider()
    {
        if (currentState != FishingState.Idle) return;

        float totalBonusRate = bonusRareRate + bonusLegendaryRate;
        fishingSlider.StartSlider(OnSliderResult, totalBonusRate);
    }

    private void OnSliderResult(bool isInGreenZone)
    {
        selectedRarity = GetFishRarity(isInGreenZone);
        ChangeState(FishingState.Casting);
    }

    public void OnMinigameWin()
    {
        Debug.Log("🏆 Win: " + selectedRarity);
        rhythmMinigame.SetActive(false);
        ChangeState(FishingState.Pulling);
    }

    public void ResetToIdle()
    {
        ChangeState(FishingState.Idle);
    }

    // ---------------------- STATE MACHINE ----------------------------

    private void ChangeState(FishingState newState)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        currentState = newState;

        switch (currentState)
        {
            case FishingState.Idle: IdleState(); break;
            case FishingState.Casting: QuangCanState(); break;
            case FishingState.Waiting: WaitingState(); break;
            case FishingState.Minigame: MinigameState(); break;
            case FishingState.Pulling: KeoCanState(); break;
        }
    }

    private void IdleState()
    {
        playerAnimator.Play("Idle");
    }

    private void QuangCanState()
    {
        playerAnimator.Play("QuangCan");
        currentCoroutine = StartCoroutine(DelayThen(() => ChangeState(FishingState.Waiting), 1f));
    }

    private void WaitingState()
    {
        playerAnimator.Play("Fishing");
        currentCoroutine = StartCoroutine(WaitForFishCoroutine());
    }

    private void MinigameState()
    {
        playerAnimator.Play("CanCau");
        rhythmMinigame.SetActive(true);
        keySpawner.SetDifficulty(GetDifficultyFromRarity(selectedRarity));
    }

    private void KeoCanState()
    {
        playerAnimator.Play("KeoCan");
        currentCoroutine = StartCoroutine(DelayThen(() => ChangeState(FishingState.Idle), 0.5f));
    }

    // ---------------------- UTILITY ----------------------------

    /// <summary>
    /// Trả về độ hiếm cá dựa vào vùng xanh và bonus
    /// </summary>
    private FishRarity GetFishRarity(bool isInGreenZone)
    {
        float roll = Random.Range(0f, 100f);

        float baseRare = isInGreenZone ? 30f : 5f;
        float baseLegendary = isInGreenZone ? 10f : 2f;

        float finalRareChance = baseRare + bonusRareRate;
        float finalLegendaryChance = baseLegendary + bonusLegendaryRate;

        // Giới hạn để tránh vượt 100%
        finalRareChance = Mathf.Clamp(finalRareChance, 0f, 100f);
        finalLegendaryChance = Mathf.Clamp(finalLegendaryChance, 0f, 100f);

        if (roll < finalLegendaryChance) return FishRarity.Legendary;
        if (roll < finalLegendaryChance + finalRareChance) return FishRarity.Rare;
        return FishRarity.Common;
    }

    private DifficultyLevel GetDifficultyFromRarity(FishRarity rarity)
    {
        switch (rarity)
        {
            case FishRarity.Common: return DifficultyLevel.Easy;
            case FishRarity.Rare: return DifficultyLevel.Medium;
            case FishRarity.Legendary: return DifficultyLevel.Hard;
            default: return DifficultyLevel.Easy;
        }
    }

    private IEnumerator WaitForFishCoroutine()
    {
        float waitTime = Random.Range(waitTimeMin, waitTimeMax);
        yield return new WaitForSeconds(waitTime);
        ChangeState(FishingState.Minigame);
    }

    private IEnumerator DelayThen(System.Action callback, float delay)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }

    // ---------------------- BONUS RATE API ----------------------------

    /// <summary>
    /// Gọi hàm này sau khi người chơi chọn cần hoặc mồi để cập nhật tỉ lệ thưởng
    /// </summary>
    public void SetBonusRate(float rareBonus, float legendaryBonus)
    {
        bonusRareRate = rareBonus;
        bonusLegendaryRate = legendaryBonus;
        Debug.Log($"🎣 Bonus Updated: +{rareBonus}% Rare, +{legendaryBonus}% Legendary");
    }
}

// ---------------------- ENUMS ----------------------------

public enum FishRarity
{
    Common,
    Rare,
    Legendary
}

public enum FishingState
{
    Idle,
    Casting,
    Waiting,
    Minigame,
    Pulling
}
