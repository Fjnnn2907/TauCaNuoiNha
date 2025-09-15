using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CutsceneText : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueText;
    public GameObject finalObjectToShow;

    [Header("Text Settings")]
    public List<string> cutsceneKeys;
    public float typingSpeed = 0.05f;

    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool isWaitingForNext = false;

    void Start()
    {
        finalObjectToShow.SetActive(false);
        StartCoroutine(TypeLine());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = GetLocalizedLine(cutsceneKeys[currentLineIndex]);
                isTyping = false;
                isWaitingForNext = true;
            }
            else if (isWaitingForNext)
            {
                currentLineIndex++;

                if (currentLineIndex < cutsceneKeys.Count)
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

        string line = GetLocalizedLine(cutsceneKeys[currentLineIndex]);
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
        gameObject.SetActive(false);
    }

    string GetLocalizedLine(string key)
    {
        if (LanguageManager.Instance != null)
            return LanguageManager.Instance.GetText(key);
        return key; // fallback nếu chưa load ngôn ngữ
    }
}
