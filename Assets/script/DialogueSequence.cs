using System.Collections;   // Needed for IEnumerator
using UnityEngine;          // Needed for MonoBehaviour
using System;               // Needed for System.Action

public class DialogueSequence : MonoBehaviour
{
    [Tooltip("Add dialogue controllers in order: Enemy line, Player line, Enemy line, ...")]
    public DialogueController[] dialogueSequence;

    // Called once after all dialogues finish
    public Action OnSequenceFinished;

    // Call this manually to start the sequence
    public void StartSequence()
    {
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        foreach (DialogueController dialogue in dialogueSequence)
        {
            if (dialogue == null) continue;

            dialogue.gameObject.SetActive(true);  // Show dialogue
            dialogue.PlayDialogue();              // Start dialogue animation

            // Wait until this dialogue disables itself
            yield return new WaitUntil(() => !dialogue.gameObject.activeSelf);
        }

        // Sequence finished
        OnSequenceFinished?.Invoke();
    }
}
