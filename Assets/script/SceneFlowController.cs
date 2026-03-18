using System.Collections;
using UnityEngine;

public class SceneFlowController : MonoBehaviour
{
    [Header("Intro Dialogue")]
    public DialogueController introDialogue;

    [Header("Mailbox Objective")]
    public ObjectiveUI objectiveUI;
    public string mailObjectiveText = "Get Mail";
    public MailboxInteraction mailbox;

    void Start()
    {
        StartCoroutine(SceneSequence());
    }

    IEnumerator SceneSequence()
    {
        // Lock mailbox interaction initially
        mailbox.enabled = false;

        // WAIT for intro dialogue to finish (it already started on its own)
        yield return new WaitUntil(() => !introDialogue.gameObject.activeSelf);

        // Show "Get Mail" objective
        objectiveUI.ShowObjective(mailObjectiveText);

        // Enable mailbox interaction
        mailbox.enabled = true;
    }
}
