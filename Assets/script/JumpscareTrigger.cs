using UnityEngine;

public class JumpscareTrigger : MonoBehaviour
{
    [Header("References")]
    public AudioSource loudNoise;
    public DialogueSequence dialogueSequence;
    public CanvasGroup jumpscareCanvas;
    public QTESystem qteSystem;

    [Header("QTE Settings")]
   
    public float qteTimeout = 3f;

    private bool hasTriggered = false;

    void Start()
    {
        if (jumpscareCanvas != null)
        {
            jumpscareCanvas.alpha = 0f;
            jumpscareCanvas.interactable = false;
            jumpscareCanvas.blocksRaycasts = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || hasTriggered)
            return;

        hasTriggered = true;

        if (loudNoise != null)
            loudNoise.Play();

        if (jumpscareCanvas != null)
        {
            jumpscareCanvas.alpha = 1f;
            jumpscareCanvas.interactable = true;
            jumpscareCanvas.blocksRaycasts = true;
        }

        if (dialogueSequence != null)
        {
            dialogueSequence.OnSequenceFinished += StartQTEAfterDialogue;
            dialogueSequence.StartSequence();
        }
        else
        {
            StartQTEAfterDialogue();
        }
    }

    void StartQTEAfterDialogue()
{
    if (qteSystem != null)
    {
        qteSystem.StartTimingQTE(
            KeyCode.Space,
            qteTimeout,
            OnQTEResult
        );
    }
}

    void OnQTEResult(bool success)
    {
        if (success)
            Debug.Log("Jumpscare QTE succeeded!");
        else
            Debug.Log("Jumpscare QTE failed!");
    }
}
