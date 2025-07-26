using System.Collections.Generic;
using UnityEngine;

public class PlayerInputChecker : MonoBehaviour
{
    private List<ArrowDirection> currentSequence;
    private int currentIndex = 0;
    public System.Action<bool> OnSequenceEnd;

    public AuditionManager ui;

    public void SetSequence(List<ArrowDirection> sequence)
    {
        currentSequence = sequence;
        currentIndex = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) CheckInput(ArrowDirection.Up);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) CheckInput(ArrowDirection.Down);
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) CheckInput(ArrowDirection.Left);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) CheckInput(ArrowDirection.Right);
    }

    void CheckInput(ArrowDirection input)
    {
        if (currentSequence == null || currentIndex >= currentSequence.Count)
            return;

        if (input == currentSequence[currentIndex])
        {
            ui.HighlightArrowAt(currentIndex, input);
            currentIndex++;

            if (currentIndex == currentSequence.Count)
            {
                OnSequenceEnd?.Invoke(true);
            }
        }
        else
        {
            Debug.Log("❌ Sai! Reset lại.");
            currentIndex = 0;
            ui.PlayWrongInputFlash();
            ui.ResetArrowSprites();
        }
    }
}
