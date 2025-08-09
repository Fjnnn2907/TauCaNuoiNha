using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public Sprite icon;
    public string npcName;
    [TextArea(2, 5)]
    public List<string> sentences;
}
