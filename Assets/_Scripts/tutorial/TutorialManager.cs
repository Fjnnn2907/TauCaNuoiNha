using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        [TextArea] public string dialogueVN;   // 🇻🇳 Tiếng Việt
        [TextArea] public string dialogueEN;   // 🇬🇧 Tiếng Anh

        public Button buttonToClick; // nút cần highlight
        public bool showArrow = true; 
        public ArrowPosition arrowPosition = ArrowPosition.Right;
    }

    public enum ArrowPosition { Left, Right, Top, Bottom }

    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private List<TutorialStep> steps;

    [Header("Highlight UI")]
    [SerializeField] private RectTransform highlightImage;
    [SerializeField] private float arrowOffset = 80f;

    private int currentStep = -1;
    private Dictionary<Button, bool> originalStates = new Dictionary<Button, bool>();

    private void Start()
    {
        if (GameSettings.EnableTutorial)
        {
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

        // 🔥 Chọn ngôn ngữ hiển thị
        if (LanguageManager.Instance.currentLanguage == LanguageManager.Language.Vietnamese)
            dialogueText.text = step.dialogueVN;
        else
            dialogueText.text = step.dialogueEN;

        // Highlight + mũi tên
        if (highlightImage != null && step.buttonToClick != null && step.showArrow)
        {
            highlightImage.gameObject.SetActive(true);
            RectTransform btnRect = step.buttonToClick.GetComponent<RectTransform>();

            Vector3 offset = Vector3.zero;
            Vector3 scale = Vector3.one;

            switch (step.arrowPosition)
            {
                case ArrowPosition.Left:
                    offset = new Vector3(-arrowOffset, 0, 0);
                    scale = new Vector3(-1, 1, 1);
                    break;
                case ArrowPosition.Right:
                    offset = new Vector3(arrowOffset, 0, 0);
                    scale = new Vector3(1, 1, 1);
                    break;
                case ArrowPosition.Top:
                    offset = new Vector3(0, arrowOffset, 0);
                    break;
                case ArrowPosition.Bottom:
                    offset = new Vector3(0, -arrowOffset, 0);
                    scale = new Vector3(1, -1, 1);
                    break;
            }

            highlightImage.position = btnRect.position + offset;
            highlightImage.localScale = scale;
            highlightImage.SetAsLastSibling();
        }
        else if (highlightImage != null)
        {
            highlightImage.gameObject.SetActive(false);
        }

        // Disable tất cả button
        foreach (var b in originalStates.Keys)
            b.interactable = false;

        // Bật lại button cần bấm
        if (step.buttonToClick != null)
        {
            step.buttonToClick.interactable = true;
            step.buttonToClick.onClick.AddListener(OnTutorialButtonClicked);
        }
    }

    private void OnTutorialButtonClicked()
    {
        steps[currentStep].buttonToClick.onClick.RemoveListener(OnTutorialButtonClicked);
        NextStep();
    }

    private void EndTutorial()
    {
        tutorialPanel.SetActive(false);
        if (highlightImage != null) highlightImage.gameObject.SetActive(false);

        foreach (var kvp in originalStates)
        {
            if (kvp.Key != null)
                kvp.Key.interactable = kvp.Value;
        }

        GameSettings.EnableTutorial = false;
        Debug.Log("✅ Tutorial finished & restored button states!");
    }
}
