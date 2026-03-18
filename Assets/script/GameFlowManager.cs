using UnityEngine; // Needed for MonoBehaviour
using System;      // Needed for System.Action

public class GameFlowManager : MonoBehaviour
{
    public DialogueSequence dialogueSequence;
    public KillEventController killEventController;

    void Start()
    {
        // Subscribe to dialogue finished event
        if (dialogueSequence != null && killEventController != null)
        {
            dialogueSequence.OnSequenceFinished += () =>
            {
                killEventController.dialogueEnded = true;
                killEventController.StartKillEvent();
            };

            dialogueSequence.StartSequence();
        }
        else
        {
            Debug.LogError("GameFlowManager: Assign DialogueSequence and KillEventController in Inspector!");
        }
    }
}
