using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueNPCSO", menuName = "Scriptable Objects/DialogueNPCSO")]
public class DialogueNPCSO : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;
    public Color npcColor;
    public List<string> dialogueLines;
}
