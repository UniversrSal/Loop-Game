using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MailboxInteraction : MonoBehaviour
{
    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.X;
    private bool playerInRange;
    private bool mailCollected;

    [Header("Prompt UI")]
    public GameObject interactPrompt; // "Get Mail - Press X"

    [Header("Mail Canvas")]
    public CanvasGroup mailCanvas;
    public float canvasVisibleTime = 4f;
    public float fadeDuration = 1f;
    public AudioSource audioSource;
    public AudioClip mailSound;

    [Header("Player Control")]
    public MonoBehaviour playerMovement; // Drag your movement script here

    [Header("Dialogue")]
    public DialogueController dialogueController;

    [Header("Objective")]
    public ObjectiveUI objectiveUI;
    public string nextObjectiveText = "Go Home";
    public GameObject objectiveLocator; // Regular next objective
    public float objectiveFadeDuration = 1f;
    private CanvasGroup objectiveCanvasGroup;

    [Header("Mail Objective Locator")]
    public GameObject mailObjectiveLocator; // Drag your mail objective UI here
    public float mailObjectiveFadeDuration = 1f;
    private CanvasGroup mailObjectiveCanvasGroup;

    [Header("Scene Progression")]
    public GameObject sceneLoaderTrigger; // Disabled by default

    void Start()
    {
        // Interaction prompt
        interactPrompt.SetActive(false);

        // Mail canvas setup
        mailCanvas.alpha = 0f;
        mailCanvas.gameObject.SetActive(false);

        // Regular objective locator setup
        if (objectiveLocator != null)
        {
            objectiveCanvasGroup = objectiveLocator.GetComponent<CanvasGroup>();
            if (objectiveCanvasGroup == null)
                objectiveCanvasGroup = objectiveLocator.AddComponent<CanvasGroup>();

            objectiveCanvasGroup.alpha = 0f;
            objectiveLocator.SetActive(false);
        }

        // Mail objective locator setup
        if (mailObjectiveLocator != null)
        {
            mailObjectiveCanvasGroup = mailObjectiveLocator.GetComponent<CanvasGroup>();
            if (mailObjectiveCanvasGroup == null)
                mailObjectiveCanvasGroup = mailObjectiveLocator.AddComponent<CanvasGroup>();

            mailObjectiveCanvasGroup.alpha = 1f; // visible at start
            mailObjectiveLocator.SetActive(true);
        }

        // Scene trigger and dialogue
        sceneLoaderTrigger.SetActive(false);
        dialogueController.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && !mailCollected && Input.GetKeyDown(interactKey))
        {
            StartCoroutine(MailSequence());
        }
    }

    IEnumerator MailSequence()
    {
        mailCollected = true;
        interactPrompt.SetActive(false);

        // Fade out mail objective locator
        if (mailObjectiveLocator != null)
            StartCoroutine(FadeMailObjective(0f, mailObjectiveFadeDuration));

        // Disable player movement
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Show mail canvas
        mailCanvas.gameObject.SetActive(true);
        mailCanvas.alpha = 1f;

        if (audioSource && mailSound)
            audioSource.PlayOneShot(mailSound);

        yield return new WaitForSeconds(canvasVisibleTime);

        // Fade out mail canvas
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            mailCanvas.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        mailCanvas.alpha = 0f;
        mailCanvas.gameObject.SetActive(false);

        // Re-enable player movement
        if (playerMovement != null)
            playerMovement.enabled = true;

        // Start dialogue
        dialogueController.gameObject.SetActive(true);
        dialogueController.PlayDialogue();

        // Show next objective
        if (objectiveUI != null)
            objectiveUI.ShowObjective(nextObjectiveText);

        if (objectiveLocator != null)
        {
            objectiveLocator.SetActive(true);
            StartCoroutine(FadeObjective(1f, objectiveFadeDuration));
        }

        // Enable scene trigger
        sceneLoaderTrigger.SetActive(true);
    }

    IEnumerator FadeObjective(float targetAlpha, float duration)
    {
        if (objectiveCanvasGroup == null) yield break;

        float startAlpha = objectiveCanvasGroup.alpha;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            objectiveCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / duration);
            yield return null;
        }

        objectiveCanvasGroup.alpha = targetAlpha;

        if (targetAlpha == 0f)
            objectiveLocator.SetActive(false);
    }

    IEnumerator FadeMailObjective(float targetAlpha, float duration)
    {
        if (mailObjectiveCanvasGroup == null) yield break;

        float startAlpha = mailObjectiveCanvasGroup.alpha;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            mailObjectiveCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / duration);
            yield return null;
        }

        mailObjectiveCanvasGroup.alpha = targetAlpha;

        if (targetAlpha == 0f)
            mailObjectiveLocator.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !mailCollected)
        {
            playerInRange = true;
            interactPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactPrompt.SetActive(false);
        }
    }
}
