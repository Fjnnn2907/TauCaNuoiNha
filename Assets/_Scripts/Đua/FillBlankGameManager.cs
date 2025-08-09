using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[System.Serializable]
public class FillSentence
{
    public string template;
    public string answer;
    public string hint;
    public int difficulty;
}

public class FillBlankGameManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI hintText;
    public TMP_InputField answerInput;
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI cpuScoreText;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI countdownText;

    [Header("Canoe Settings")]
    public Transform canoePlayer;
    public Transform canoeCPU;
    public float pushUpDistance = 0.5f;
    public float pushSpeed = 5f;

    [Header("Game Settings")]
    public List<FillSentence> sentences = new List<FillSentence>();
    public int rounds = 5;
    public float cpuThinkMin = 1.0f;
    public float cpuThinkMax = 3.0f;
    public float timeLimitPerRound = 10f;

    [Header("Race Settings")]
    public Transform finishLine; // Dùng GameObject thay vì float

    int currentRound = 0;
    int playerScore = 0;
    int cpuScore = 0;
    FillSentence currentSentence;
    bool roundActive = false;
    bool gameEnded = false;
    Coroutine roundTimer;

    void Start()
    {
        if (sentences.Count == 0)
        {
            sentences.Add(new FillSentence { template = "I have a {0} in my pocket.", answer = "coin", hint = "Một vật nhỏ, tròn, dùng để mua bán", difficulty = 1 });
            sentences.Add(new FillSentence { template = "The capital of France is {0}.", answer = "paris", hint = "Thành phố ánh sáng", difficulty = 2 });
            sentences.Add(new FillSentence { template = "E = mc{0} is Einstein's famous equation.", answer = "2", hint = "Lũy thừa của tốc độ ánh sáng", difficulty = 1 });
            sentences.Add(new FillSentence { template = "She was wearing a {0} dress at the gala.", answer = "beautiful", hint = "Trái nghĩa với xấu xí", difficulty = 3 });
            sentences.Add(new FillSentence { template = "The quick brown {0} jumps over the lazy dog.", answer = "fox", hint = "Một loài động vật màu cam nâu", difficulty = 1 });
        }

        answerInput.lineType = TMP_InputField.LineType.SingleLine;
        answerInput.onSubmit.AddListener(delegate { OnPlayerSubmit(); });

        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        countdownText.gameObject.SetActive(true);
        questionText.gameObject.SetActive(false);
        hintText.gameObject.SetActive(false);
        answerInput.gameObject.SetActive(false);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);

        StartGame();
    }

    void StartGame()
    {
        playerScore = 0;
        cpuScore = 0;
        UpdateScoreUI();
        currentRound = 0;
        messageText.text = "";
        NextRound();
    }

    void NextRound()
    {
        if (gameEnded) return;
        if (roundTimer != null) StopCoroutine(roundTimer);
        answerInput.text = "";
        if (currentRound >= rounds) return;

        currentRound++;
        currentSentence = sentences[UnityEngine.Random.Range(0, sentences.Count)];
        ShowQuestion();
        roundActive = true;
        float cpuDelay = 2f; // Máy sẽ trả lời sau đúng 2 giây
        StartCoroutine(CPUThinkAndPlay(cpuDelay));
        roundTimer = StartCoroutine(RoundTimeout(timeLimitPerRound));
    }

    void ShowQuestion()
    {
        questionText.gameObject.SetActive(true);
        hintText.gameObject.SetActive(true);
        answerInput.gameObject.SetActive(true);

        questionText.text = currentSentence.template;
        hintText.text = "Gợi ý: " + currentSentence.hint;
        messageText.text = $"Vòng {currentRound}/{rounds} — Điền từ vào chỗ trống";
        answerInput.ActivateInputField();
    }

    public void OnPlayerSubmit()
    {
        if (!roundActive) return;
        string playerAnswer = answerInput.text.Trim().ToLower();
        if (string.IsNullOrEmpty(playerAnswer))
        {
            messageText.text = "Bạn chưa nhập gì!";
            return;
        }

        bool playerCorrect = CheckAnswer(playerAnswer, currentSentence.answer);
        if (playerCorrect)
        {
            playerScore++;
            messageText.text = $"Bạn đúng! (+1 điểm) — đáp án: {currentSentence.answer}";
            PushCanoe(canoePlayer);
            FinishRound();
        }
        else
        {
            messageText.text = $"Sai rồi! Máy sẽ thử đoán tiếp...";
        }
    }

    IEnumerator CPUThinkAndPlay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!roundActive) yield break;

        float baseChance = 0.6f;
        switch (currentSentence.difficulty)
        {
            case 1: baseChance = 0.75f; break;
            case 2: baseChance = 0.45f; break;
            case 3: baseChance = 0.25f; break;
        }

        bool cpuCorrect = (UnityEngine.Random.value <= baseChance);

        if (cpuCorrect)
        {
            cpuScore++;
            messageText.text = $"Máy đoán đúng: {currentSentence.answer}  (+1 điểm cho máy)";
            PushCanoe(canoeCPU);
            FinishRound();
        }
        else
        {
            messageText.text = $"Máy đoán sai. Bạn còn thời gian để nhập!";
        }
    }

    IEnumerator RoundTimeout(float seconds)
    {
        float t = 0f;
        while (t < seconds)
        {
            t += Time.deltaTime;
            yield return null;
        }
        if (!roundActive) yield break;

        messageText.text = $"Hết thời gian! Đáp án: {currentSentence.answer}";
        cpuScore++;
        messageText.text += " Máy được +1 điểm (timeout).";
        PushCanoe(canoeCPU);
        FinishRound();
    }

    void FinishRound()
    {
        roundActive = false;
        UpdateScoreUI();
        if (roundTimer != null) StopCoroutine(roundTimer);
        StartCoroutine(NextRoundAfterDelay(2f));
    }

    IEnumerator NextRoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!gameEnded) NextRound();
    }

    bool CheckAnswer(string given, string correct)
    {
        return Normalize(given) == Normalize(correct);
    }

    string Normalize(string s)
    {
        return s.Trim().ToLower();
    }

    void UpdateScoreUI()
    {
        playerScoreText.text = $"Bạn: {playerScore}";
        cpuScoreText.text = $"Máy: {cpuScore}";
    }

    void EndGame(string result)
    {
        gameEnded = true;
        roundActive = false;
        
        questionText.gameObject.SetActive(false);
        hintText.gameObject.SetActive(false);
        answerInput.gameObject.SetActive(false);

        messageText.text = $"Kết thúc — {result}";
        
        // Dừng các coroutines liên quan đến round, nhưng không dừng PushUpAndCheck
        if (roundTimer != null) StopCoroutine(roundTimer);
    }

    void PushCanoe(Transform canoe)
    {
        StartCoroutine(PushUpAndCheck(canoe));
    }

    IEnumerator PushUpAndCheck(Transform canoe)
    {
        Vector3 startPos = canoe.position;
        Vector3 upPos = startPos + Vector3.up * pushUpDistance;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * pushSpeed;
            canoe.position = Vector3.Lerp(startPos, upPos, t);
            yield return null;
        }

        // Kiểm tra cán đích bằng GameObject finishLine
        if (canoe.position.y >= finishLine.position.y)
        {
            if (canoe == canoePlayer)
            {
                EndGame("Bạn thắng! ");
                FishingManager.Instance?.TriggerFishingEvent(true);
            }
            else
            {
                EndGame("Máy thắng! ");
                FishingManager.Instance?.TriggerFishingEvent(false);
            }
            yield break;
        }
    }
}
