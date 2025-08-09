using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : Singleton<DialogueManager>
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Image icon;

    private Queue<string> sentences = new Queue<string>();
    public bool IsDialoguePlaying { get; private set; } = false;

    // ✅ Nhận trực tiếp DialogueData
    public void StartDialogue(DialogueData dialogueData)
    {
        dialoguePanel.SetActive(true);
        nameText.text = dialogueData.npcName;
        icon.sprite = dialogueData.icon;
        sentences.Clear();

        foreach (string line in dialogueData.sentences)
        {
            sentences.Enqueue(line);
        }

        IsDialoguePlaying = true;
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        IsDialoguePlaying = false;
    }

    private void Update()
    {
        if (IsDialoguePlaying && Input.GetKeyDown(KeyCode.Mouse0))
        {
            DisplayNextSentence();
        }
    }
}
