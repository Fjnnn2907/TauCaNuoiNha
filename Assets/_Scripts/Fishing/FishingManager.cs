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
    public FishingBaitData CurrentBait { get; set; }

    [Header("Timing")]
    public float waitTimeMin = 3f;
    public float waitTimeMax = 8f;

    [Header("Rate")]
    [SerializeField][Range(0, 100)] private float rodBonusRare = 0f;
    [SerializeField][Range(0, 100)] private float rodBonusLegendary = 0f;
    [SerializeField][Range(0, 100)] private float baitBonusRare = 0f;
    [SerializeField][Range(0, 100)] private float baitBonusLegendary = 0f;

    private FishingState currentState = FishingState.Idle;
    private Coroutine currentCoroutine;
    private FishRarity selectedRarity = FishRarity.Common;

    public float TotalBonusRareRate => rodBonusRare + baitBonusRare;
    public float TotalBonusLegendaryRate => rodBonusLegendary + baitBonusLegendary;

    public void SetRodBonus(FishingRodData rod)
    {
        rodBonusRare = rod != null ? rod.bonusRareRate : 0f;
        rodBonusLegendary = rod != null ? rod.bonusLegendaryRate : 0f;
    }

    public void SetBaitBonus(FishingBaitData bait)
    {
        baitBonusRare = bait != null ? bait.bonusRareRate : 0f;
        baitBonusLegendary = bait != null ? bait.bonusLegendaryRate : 0f;
    }

    public void PrepareCastWithSlider()
    {
        if (currentState != FishingState.Idle) return;

        if (CurrentRod == null)
        {
            Debug.Log("⚠️ Bạn chưa chọn cần câu!");
            return;
        }

        if (CurrentBait == null)
        {
            Debug.Log("⚠️ Bạn chưa chọn mồi câu!");
            return;
        }

        if (CurrentBait != null && !BaitInventory.Instance.ConsumeBait(CurrentBait))
        {
            Debug.Log("⚠️ Không đủ mồi để câu!");
            return;
        }

        float totalBonusRate = TotalBonusRareRate + TotalBonusLegendaryRate;
        fishingSlider.StartSlider(OnSliderResult, totalBonusRate);
    }


    private void OnSliderResult(bool isInGreenZone)
    {
        selectedRarity = GetFishRarity(isInGreenZone);
        ChangeState(FishingState.Casting);
    }

    public void OnMinigameWin()
    {
        Debug.Log("\uD83C\uDFC6 Win: " + selectedRarity);
        rhythmMinigame.SetActive(false);
        ChangeState(FishingState.Pulling);
    }

    public void ResetToIdle()
    {
        ChangeState(FishingState.Idle);
    }

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

    private void IdleState() => playerAnimator.Play("Idle");

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

    private FishRarity GetFishRarity(bool isInGreenZone)
    {
        float roll = Random.Range(0f, 100f);

        float baseRare = isInGreenZone ? 30f : 5f;
        float baseLegendary = isInGreenZone ? 10f : 2f;

        float finalRareChance = Mathf.Clamp(baseRare + TotalBonusRareRate, 0f, 100f);
        float finalLegendaryChance = Mathf.Clamp(baseLegendary + TotalBonusLegendaryRate, 0f, 100f);

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
}

public enum FishRarity { Common, Rare, Legendary }
public enum FishingState { Idle, Casting, Waiting, Minigame, Pulling }
