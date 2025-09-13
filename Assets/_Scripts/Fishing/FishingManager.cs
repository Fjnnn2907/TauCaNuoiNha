using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingManager : Singleton<FishingManager>
{
    [Header("References")]
    public Animator playerAnimator;
    public GameObject rhythmMinigame;
    public FishingSlider fishingSlider;
    public Button cauButton;
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

    [Header("Special Event Settings")]
    public GameObject canvanObj;
    public DialogueData winDialogue;           // Thoại khi thắng
    public FishingRodData rewardRod;           // Cần câu thưởng
    public FishData fishEvent;           // Mai rùa
    public int turtleShellLoseAmount = 1;      // Số lượng mất khi thua

    [Header("Objects to Hide in Minigame")]
    public List<GameObject> objectsToHide = new List<GameObject>();


    public float TotalBonusRareRate => rodBonusRare + baitBonusRare;
    public float TotalBonusLegendaryRate => rodBonusLegendary + baitBonusLegendary;

    public bool isPlayMiniGame { get; private set; }
    private void Start()
    {
        cauButton.onClick.AddListener(PrepareCastWithSlider);
    }

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
            NotificationManager.Instance?.ShowNotification("Hình như chưa có cần câu á");
            NotificationManager.Instance?.ShowNotification(LanguageManager.Instance.GetText("thongbao_chua_chon_can_cau"));
            return;
        }

        if (CurrentBait == null)
        {
            Debug.Log("⚠️ Bạn chưa chọn mồi câu!");
            NotificationManager.Instance?.ShowNotification(LanguageManager.Instance.GetText("thongbao_chua_chon_moi"));
            return;
        }

        if (!BaitInventory.Instance.ConsumeBait(CurrentBait))
        {
            Debug.Log("⚠️ Không đủ mồi!");
            NotificationManager.Instance?.ShowNotification(LanguageManager.Instance.GetText("thongbao_khong_du_moi"));
            return;
        }

        FishingBaitUI.Instance?.RefreshUI();
        float totalBonusRate = TotalBonusRareRate + TotalBonusLegendaryRate;
        Debug.Log(totalBonusRate);
        fishingSlider.StartSlider(OnSliderResult, totalBonusRate);
        //cauButton.interactable = false;
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
            FishInventory.Instance.AddFish(fish);
            QuestManager.Instance.OnFishCaught(fish);
            ShowCaughtFish(fish);
            isPlayMiniGame = false;
            // ✅ Nếu cá là cá đặc biệt thì kích hoạt event
            if (fish.isSpecial)
            {
                Debug.Log("a");
                isPlayMiniGame = true;
                canvanObj.SetActive(false);
                SpecialEventManager.Instance.TriggerSpecialEvent(fish.specialEventID);
            }           
        }

        ChangeState(FishingState.Pulling);
    }

    public void OnMinigameLose()
    {
        Debug.Log("❌ Thua minigame vì hết giờ");

        rhythmMinigame.SetActive(false); // Tắt minigame
        ChangeState(FishingState.Idle);  // Quay lại trạng thái Idle
        isPlayMiniGame = false;
    }
    public void TriggerFishingEvent(bool isWin)
    {
        if (isWin)
        {
            // ✅ Tặng cần câu
            if (rewardRod != null)
            {
                FishingInventory.Instance?.AddRod(rewardRod);
                NotificationManager.Instance?.ShowNotification(
                    string.Format(LanguageManager.Instance.GetText("thongbao_nhan_can_cau"),
                    LanguageManager.Instance.GetText(rewardRod.rodName))
                );
            }
        }
        else
        {
            // ❌ Mất cá
            if (fishEvent != null)
            {
                FishInventory.Instance?.RemoveFish(fishEvent, turtleShellLoseAmount);
                NotificationManager.Instance?.ShowNotification(
                    string.Format(LanguageManager.Instance.GetText("thongbao_mat_ca"),
                    LanguageManager.Instance.GetText(fishEvent.nameKey))
                );
            }
        }
    }

    private void ShowCaughtFish(FishData fish)
    {
        if (fish == null || fishSprite == null) return;

        SpriteRenderer sr = fishSprite.GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.sprite = fish.sprite;

        fishSprite.SetActive(true);
        StartCoroutine(AutoHideFish(fishSprite));
        FishCollection.Instance.DiscoverFish(fish);

        if (fish.isUnique)
        {
            NotificationManager.Instance?.ShowNotification(
                string.Format(LanguageManager.Instance.GetText("thongbao_ca_doc_nhat"),
                LanguageManager.Instance.GetText(fish.nameKey))
            );
        }
        else
        {
            NotificationManager.Instance?.ShowNotification(
                string.Format(LanguageManager.Instance.GetText("thongbao_ca_thuong"),
                LanguageManager.Instance.GetText(fish.nameKey))
            );
        }
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

        // Base rate (rộng rãi hơn)
        float baseRare = isInGreenZone ? 20f : 10f;       // Hit: 20% | Miss: 10%
        float baseLegendary = isInGreenZone ? 6f : 2f;    // Hit: 6%  | Miss: 2%

        // --- Config theo range hiện tại ---
        const float RARE_MIN = 9f, RARE_MAX = 92f;
        const float LEG_MIN = 4f, LEG_MAX = 83f;

        // Bonus tối đa
        const float RARE_ADD_MAX = 25f;   // Rare có thể tăng thêm 25%
        const float LEG_ADD_MAX = 15f;   // Legendary có thể tăng thêm 15%

        // Miss vẫn có cơ hội khá cao (80% hiệu lực bonus)
        float hitFactor = isInGreenZone ? 1f : 0.8f;

        float Scale(float v, float min, float max)
        {
            return Mathf.Clamp01((v - min) / Mathf.Max(0.0001f, max - min));
        }

        float rareAdd = Scale(TotalBonusRareRate, RARE_MIN, RARE_MAX) * RARE_ADD_MAX * hitFactor;
        float legAdd = Scale(TotalBonusLegendaryRate, LEG_MIN, LEG_MAX) * LEG_ADD_MAX * hitFactor;

        float finalRare = Mathf.Clamp(baseRare + rareAdd, 0f, 45f);        // Rare tối đa 45%
        float finalLegendary = Mathf.Clamp(baseLegendary + legAdd, 0f, 18f); // Legendary tối đa 18%

        if (roll < finalLegendary) return FishRarity.Legendary;
        if (roll < finalLegendary + finalRare) return FishRarity.Rare;
        return FishRarity.Common;
    }
    private void SetObjectsActive(List<GameObject> objects, bool state)
    {
        foreach (var obj in objects)
        {
            if (obj != null) obj.SetActive(state);
        }
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
        isPlayMiniGame = true;
        playerAnimator.Play("CanCau");
        rhythmMinigame.SetActive(true);

        SetObjectsActive(objectsToHide, false);

        KeySpawner.Instance?.SetDifficulty(GetDifficultyFromRarity(selectedRarity));
        if (AuditionManager.Instance != null)
            AuditionManager.Instance?.SetDifficulty(GetDifficultyFromRarity(selectedRarity));

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.StartMinigame(GetDifficultyFromRarity(selectedRarity));
    }

    private void KeoCanState()
    {
        playerAnimator.Play("KeoCan");
        //cauButton.interactable = true;
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
