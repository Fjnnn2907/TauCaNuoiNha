using System.Collections;
using UnityEngine;

public class SpecialEventManager : Singleton<SpecialEventManager>
{

    public DialogueData turtleShellDialogue;

    public void TriggerSpecialEvent(string eventID)
    {
        switch (eventID)
        {
            case "TurtleShellEvent":
                StartCoroutine(TurtleShellSequence());
                break;
                // Có thể thêm nhiều event khác ở đây
        }
    }

    private IEnumerator TurtleShellSequence()
    {
        NPCController npc = NPCController.Instance;
        if (npc != null)
        {
            yield return npc.MoveToPointB();
        }

        DialogueManager.Instance.StartDialogue(turtleShellDialogue);

        // Chờ tới khi thoại kết thúc
        while (DialogueManager.Instance.IsDialoguePlaying)
            yield return null;

        SpecialMinigameUI.Instance.ShowMinigame("TurtleGame");
    }
}
