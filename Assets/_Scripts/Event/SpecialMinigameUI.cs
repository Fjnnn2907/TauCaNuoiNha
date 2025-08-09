using UnityEngine;

public class SpecialMinigameUI : Singleton<SpecialMinigameUI>
{
    public GameObject turtleGameUI;

    public void ShowMinigame(string gameID)
    {
        switch (gameID)
        {
            case "TurtleGame":
                turtleGameUI.SetActive(true);
                break;
        }
    }
}
