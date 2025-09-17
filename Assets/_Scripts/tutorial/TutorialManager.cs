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

            // Đưa highlight về cùng canvas (đảm bảo đúng hệ quy chiếu)
            Canvas rootCanvas = tutorialPanel.GetComponentInParent<Canvas>();
            highlightImage.SetParent(rootCanvas.transform, false);

            // Lấy vị trí button trong local của canvas
            Vector3 worldPos = btnRect.position;
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rootCanvas.transform as RectTransform,
                RectTransformUtility.WorldToScreenPoint(null, worldPos),
                rootCanvas.worldCamera,
                out localPoint
            );

            Vector2 offset = Vector2.zero;
            Vector3 scale = Vector3.one;

            switch (step.arrowPosition)
            {
                case ArrowPosition.Left:
                    offset = new Vector2(-arrowOffset, 0);
                    scale = new Vector3(-1, 1, 1);
                    break;
                case ArrowPosition.Right:
                    offset = new Vector2(arrowOffset, 0);
                    scale = new Vector3(1, 1, 1);
                    break;
                case ArrowPosition.Top:
                    offset = new Vector2(0, arrowOffset);
                    break;
                case ArrowPosition.Bottom:
                    offset = new Vector2(0, -arrowOffset);
                    scale = new Vector3(1, -1, 1);
                    break;
            }

            highlightImage.localPosition = localPoint + offset;
            highlightImage.localScale = scale;

            highlightImage.SetAsLastSibling();
        }
        else if (highlightImage != null)
        {
            highlightImage.gameObject.SetActive(false);
        }

        // 🔒 Khóa toàn bộ button
        foreach (var b in originalStates.Keys)
            b.interactable = false;

        // ✅ Chỉ bật nút cần bấm
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
