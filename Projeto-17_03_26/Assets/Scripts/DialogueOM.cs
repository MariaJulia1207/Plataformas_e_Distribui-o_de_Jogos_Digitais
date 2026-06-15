using System.Collections.Generic;
using UnityEngine;

public class DialogueOM : MonoBehaviour
{
    public static event System.Action<string> OnDialogueStarted;
    public static void PublishDialogueStarted(string dialogueId)
    {
        OnDialogueStarted?.Invoke(dialogueId);
    }
}
