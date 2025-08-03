using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CutsceneText : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueText;
    //public GameObject continueIndicator;
    public GameObject finalObjectToShow;

    [Header("Text Settings")]
    [TextArea(3, 10)]
    public List<string> cutsceneLines;
    public float typingSpeed = 0.05f;

    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool isWaitingForNext = false;

    void Start()
    {
        //continueIndicator.SetActive(false);
        finalObjectToShow.SetActive(false);
        StartCoroutine(TypeLine());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // Show full line immediately
                StopAllCoroutines();
                dialogueText.text = cutsceneLines[currentLineIndex];
                isTyping = false;
                isWaitingForNext = true;
                //continueIndicator.SetActive(true);
            }
            else if (isWaitingForNext)
            {
                //continueIndicator.SetActive(false);
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

        string line = cutsceneLines[currentLineIndex];
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        isWaitingForNext = true;
        //continueIndicator.SetActive(true);
    }

    void EndCutscene()
    {
        dialogueText.text = "";
        finalObjectToShow.SetActive(true);
        gameObject.SetActive(false); // hoặc disable cutscene UI
    }
}
