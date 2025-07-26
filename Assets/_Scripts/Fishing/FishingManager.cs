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

    [Header("UI Hiển thị cá")]
    public GameObject fishSprite; // Prefab UI
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

        if (!BaitInventory.Instance.ConsumeBait(CurrentBait))
        {
            Debug.Log("⚠️ Không đủ mồi!");
            return;
        }

        FishingBaitUI.Instance?.RefreshUI();
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
        rhythmMinigame.SetActive(false);

        FishData fish = FishDatabase.Instance.GetRandomFish(selectedRarity);
        if (fish != null)
        {
            FishInventory.Instance.AddFish(fish); // Add vào kho
            ShowCaughtFish(fish);
        }

        ChangeState(FishingState.Pulling);
    }
    public void OnMinigameLose()
    {
        Debug.Log("❌ Thua minigame vì hết giờ");

        rhythmMinigame.SetActive(false); // Tắt minigame
        ChangeState(FishingState.Idle);  // Quay lại trạng thái Idle
    }
    private void ShowCaughtFish(FishData fish)
    {
        if (fish == null || fishSprite == null) return;

        SpriteRenderer sr = fishSprite.GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.sprite = fish.sprite;

        fishSprite.SetActive(true);
        StartCoroutine(AutoHideFish(fishSprite));
        FishCollection.Instance.DiscoverFish(fish);
    }

    private IEnumerator AutoHideFish(GameObject go)
    {
        float timer = 0f;
        float maxTime = 5f;

        while (timer < maxTime && !Input.GetMouseButtonDown(0))
        {
            timer += Time.deltaTime;
            yield return null;
        }

        go.SetActive(false);
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

    private void ChangeState(FishingState newState)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        currentState = newState;

        switch (newState)
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

    private DifficultyLevel GetDifficultyFromRarity(FishRarity rarity)
    {
        return rarity switch
        {
            FishRarity.Common => DifficultyLevel.Easy,
            FishRarity.Rare => DifficultyLevel.Medium,
            FishRarity.Legendary => DifficultyLevel.Hard,
            _ => DifficultyLevel.Easy
        };
    }
}

public enum FishRarity { Common, Rare, Legendary }
public enum FishingState { Idle, Casting, Waiting, Minigame, Pulling }
