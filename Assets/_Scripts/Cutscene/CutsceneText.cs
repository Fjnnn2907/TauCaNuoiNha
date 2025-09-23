using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CutsceneLine
{
    public string key;        // Key cho text
    public Sprite image;      // Ảnh đi kèm
}

public class CutsceneText : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueText;
    public Image cutsceneImage;             // Thêm image để hiển thị
    public GameObject finalObjectToShow;
    public Button skipButton;

    [Header("Cutscene Settings")]
    public List<CutsceneLine> cutsceneLines;  // thay List<string> thành List<CutsceneLine>
    public float typingSpeed = 0.05f;

    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool isWaitingForNext = false;

    void Start()
    {
        finalObjectToShow.SetActive(false);

        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipCutscene);
            skipButton.gameObject.SetActive(true);
        }

        StartCoroutine(TypeLine());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = GetLocalizedLine(cutsceneLines[currentLineIndex].key);
                isTyping = false;
                isWaitingForNext = true;
            }
            else if (isWaitingForNext)
            {
                currentLineIndex++;

                if (currentLineIndex < cutsceneLines.Count)
                    StartCoroutine(TypeLine());
                else
                    EndCutscene();
            }
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        isWaitingForNext = false;
        dialogueText.text = "";

        // Lấy dữ liệu từ cutsceneLines
        CutsceneLine lineData = cutsceneLines[currentLineIndex];

        // Đổi hình ảnh theo đoạn
        if (cutsceneImage != null)
        {
            cutsceneImage.sprite = lineData.image;
            cutsceneImage.gameObject.SetActive(lineData.image != null);
        }

        // Hiện text kiểu typing
        string line = GetLocalizedLine(lineData.key);
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        isWaitingForNext = true;
    }

    void EndCutscene()
    {
        dialogueText.text = "";
        finalObjectToShow.SetActive(true);

        if (skipButton != null)
            skipButton.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }

    string GetLocalizedLine(string key)
    {
        if (LanguageManager.Instance != null)
            return LanguageManager.Instance.GetText(key);
        return key; // fallback nếu chưa load ngôn ngữ
    }

    // Gán hàm này vào OnClick của nút Skip
    public void SkipCutscene()
    {
        StopAllCoroutines();
        EndCutscene();
    }
}
