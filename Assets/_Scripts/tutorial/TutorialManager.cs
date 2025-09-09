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
        public bool showArrow = true; // nếu false thì không hiện mũi tên
        public ArrowPosition arrowPosition = ArrowPosition.Right; // vị trí mũi tên
    }

    public enum ArrowPosition { Left, Right, Top, Bottom }

    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private List<TutorialStep> steps;

    [Header("Highlight UI")]
    [SerializeField] private RectTransform highlightImage; // mũi tên / vòng sáng UI
    [SerializeField] private float arrowOffset = 80f;      // khoảng cách từ nút đến mũi tên

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

        // Highlight nút + chỉnh hướng mũi tên
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
                    scale = new Vector3(1, 1, 1);
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

        foreach (var kvp in originalStates)
        {
            if (kvp.Key != null)
                kvp.Key.interactable = kvp.Value;
        }

        GameSettings.EnableTutorial = false; // ✅ Tắt tutorial
        Debug.Log("✅ Tutorial finished & restored button states!");
    }

}
