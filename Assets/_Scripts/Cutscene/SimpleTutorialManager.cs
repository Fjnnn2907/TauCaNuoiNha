using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class TutorialStep
{
    [TextArea]
    public string instruction;
    public Button targetButton;
    public GameObject pointerIcon;
}

public class SimpleTutorialManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI instructionText;

    [Header("Tutorial Steps")]
    public List<TutorialStep> steps;

    private int currentIndex = 0;

    void Start()
    {
        StartStep(currentIndex);
    }

    void StartStep(int index)
    {
        if (index >= steps.Count)
        {
            EndTutorial();
            return;
        }

        TutorialStep step = steps[index];

        // Hiển thị hướng dẫn
        instructionText.text = step.instruction;

        // Bật icon chỉ tay
        if (step.pointerIcon != null)
            step.pointerIcon.SetActive(true);

        // Gắn listener: khi người chơi nhấn đúng nút
        step.targetButton.onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        // Gỡ listener cũ
        steps[currentIndex].targetButton.onClick.RemoveListener(OnButtonClicked);

        // Tắt chỉ tay cũ
        if (steps[currentIndex].pointerIcon != null)
            steps[currentIndex].pointerIcon.SetActive(false);

        // Qua bước tiếp theo
        currentIndex++;
        StartStep(currentIndex);
    }

    void EndTutorial()
    {
        instructionText.text = "";
        Debug.Log("Tutorial complete!");
        gameObject.SetActive(false); // Hoặc tắt UI hướng dẫn
    }
}
