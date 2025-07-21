using System.Collections;
using UnityEngine;

public enum FishingState
{
    Idle,
    Casting,
    Waiting,
    Minigame,
    Pulling
}

public class FishingManager : Singleton<FishingManager>
{
    [Header("References")]
    public Animator playerAnimator;
    public GameObject rhythmMinigame;

    [Header("Timing")]
    public float waitTimeMin = 3f;
    public float waitTimeMax = 8f;

    private FishingState currentState = FishingState.Idle;
    private Coroutine currentCoroutine;

    private void Update()
    {
        // Nếu cần điều kiện hoặc phím kiểm tra theo từng trạng thái có thể làm tại đây
    }

    // ---------------------- PUBLIC ENTRY POINT ------------------------

    public void StartFishing()
    {
        if (currentState != FishingState.Idle) return;
        ChangeState(FishingState.Casting);
    }

    public void OnMinigameWin()
    {
        if (currentState != FishingState.Minigame) return;
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
        // Stop any running coroutine when changing state
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        currentState = newState;

        switch (currentState)
        {
            case FishingState.Idle:
                IdleState();
                break;
            case FishingState.Casting:
                QuangCanState();
                break;
            case FishingState.Waiting:
                WaitingState();
                break;
            case FishingState.Minigame:
                MinigameState();
                break;
            case FishingState.Pulling:
                KeoCanState();
                break;
        }
    }

    // ---------------------- STATE LOGIC ----------------------------

    public void IdleState()
    {
        playerAnimator.Play("Idle");
    }

    private void QuangCanState()
    {
        playerAnimator.Play("QuangCan");

        // Delay switching to Waiting after casting animation time
        currentCoroutine = StartCoroutine(DelayThen(() => ChangeState(FishingState.Waiting), 1.0f));
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
        // Chờ người chơi thắng hoặc thua mới chuyển tiếp
    }

    private void KeoCanState()
    {
        playerAnimator.Play("KeoCan");

        // Sau kéo lên thì quay lại trạng thái Idle
        currentCoroutine = StartCoroutine(DelayThen(() => ChangeState(FishingState.Idle), 0.5f));
    }

    // ---------------------- COROUTINES ----------------------------

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
