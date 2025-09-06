using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        [TextArea] public string dialogue;
        public Button buttonToClick; // nút cần highlight
    }

    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private List<TutorialStep> steps;

    [Header("Highlight UI")]
    [SerializeField] private RectTransform highlightImage; // mũi tên / vòng sáng UI

    private int currentStep = -1;

    // Lưu trạng thái gốc của tất cả button
    private Dictionary<Button, bool> originalStates = new Dictionary<Button, bool>();

    private void Start()
    {
        if (GameSettings.EnableTutorial)
        {
            // Lưu lại trạng thái gốc
            originalStates.Clear();
            foreach (var b in FindObjectsOfType<Button>(true))
                originalStates[b] = b.interactable;

            tutorialPanel.SetActive(true);
            NextStep();
        }
        else
        {
            tutorialPanel.SetActive(false);
            if (highlightImage != null) highlightImage.gameObject.SetActive(false);
        }
    }

    private void NextStep()
    {
        currentStep++;

        if (currentStep >= steps.Count)
        {
            EndTutorial();
            return;
        }

        var step = steps[currentStep];
        dialogueText.text = step.dialogue;

        // Highlight nút
        if (highlightImage != null && step.buttonToClick != null)
        {
            highlightImage.gameObject.SetActive(true);
            RectTransform btnRect = step.buttonToClick.GetComponent<RectTransform>();
            highlightImage.position = btnRect.position;
            highlightImage.SetAsLastSibling();
        }
        else if (highlightImage != null)
        {
            highlightImage.gameObject.SetActive(false);
        }

        // Disable toàn bộ button
        foreach (var b in originalStates.Keys)
            b.interactable = false;

        // Chỉ bật nút cần bấm
        if (step.buttonToClick != null)
        {
            step.buttonToClick.interactable = true;

            // Thêm listener tạm thời
            step.buttonToClick.onClick.AddListener(OnTutorialButtonClicked);
        }
    }

    private void OnTutorialButtonClicked()
    {
        // Gỡ listener tạm
        steps[currentStep].buttonToClick.onClick.RemoveListener(OnTutorialButtonClicked);

        NextStep();
    }

    private void EndTutorial()
    {
        tutorialPanel.SetActive(false);
        if (highlightImage != null) highlightImage.gameObject.SetActive(false);

        // Restore lại trạng thái gốc thay vì bật tất cả
        foreach (var kvp in originalStates)
        {
            if (kvp.Key != null)
                kvp.Key.interactable = kvp.Value;
        }

        Debug.Log("✅ Tutorial finished & restored button states!");
    }
}
