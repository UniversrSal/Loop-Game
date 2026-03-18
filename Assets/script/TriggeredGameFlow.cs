using System.Collections;
using UnityEngine;

public class TriggeredGameFlow : MonoBehaviour
{
    [Header("Player References")]
    public MonoBehaviour playerMovementScript; // Your CharController
    public Animator playerAnimator;

    // Animator trigger names
    private string scaredTrigger = "Scared";

    [Header("Kill Event References")]
    public KillEventController killEventController;
    public DialogueSequence dialogueSequence;

    private bool triggerEntered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggerEntered) return;
        if (!other.CompareTag("Player")) return;

        triggerEntered = true;

        // Disable input immediately
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        // Trigger Scared animation
        if (playerAnimator != null)
            playerAnimator.SetTrigger(scaredTrigger);

        // Start coroutine to wait for scared animation
        StartCoroutine(WaitForScaredAnimation());
    }

    private IEnumerator WaitForScaredAnimation()
    {
        // Freeze Speed in Animator
        playerAnimator.SetFloat("Speed", 0f);

        // Wait until Scared finishes
        bool scaredDone = false;
        while (!scaredDone)
        {
            if (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Scared"))
                scaredDone = true;

            yield return null;
        }

        // Continue dialogue or KillEvent
        if (dialogueSequence != null)
        {
            dialogueSequence.OnSequenceFinished += OnDialogueFinished;
            dialogueSequence.StartSequence();
        }
        else
        {
            StartKillEvent();
        }
    }

    private void OnDialogueFinished()
    {
        StartKillEvent();
    }

    private void StartKillEvent()
    {
        if (killEventController != null)
        {
            killEventController.dialogueEnded = true;
            killEventController.StartKillEvent();
        }
    }
}
