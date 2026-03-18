using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Emotion { Neutral, Anger, Disgust, Scared, Confused }

public class DialogueController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueText;
    public Image portraitImage;
    public CanvasGroup canvasGroup;

    [Header("Dialogue")]
    [TextArea(3, 5)]
    public string dialogueLine;
    public float startDelay = 1f;
    public float typingSpeed = 0.04f;
    public float timeBeforeFade = 2f;
    public float fadeDuration = 1f;

    [Header("Emotion")]
    public Emotion currentEmotion;
    public Sprite neutralSprite;
    public Sprite angerSprite;
    public Sprite disgustSprite;
    public Sprite scaredSprite;
    public Sprite confusedSprite;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip typeSound;

    [Header("Portrait Bounce")]
    public float bounceAmplitude = 8f;
    public float bounceSpeed = 6f;

    [Header("Portrait Squash")]
    public float squashAmount = 0.1f;
    public float squashSpeed = 8f;

    [Header("Objective")]
    public ObjectiveUI objectiveUI;
    public string objectiveText = "Going Home";

    private RectTransform portraitRect;
    private Vector2 portraitStartPos;
    private Vector3 portraitStartScale;
    private bool isTyping = false;

    void Start()
    {
        dialogueText.text = "";

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        portraitRect = portraitImage.GetComponent<RectTransform>();
        portraitStartPos = portraitRect.anchoredPosition;
        portraitStartScale = portraitRect.localScale;

        SetEmotionPortrait();

        // Auto-play dialogue on game start
        PlayDialogue();
    }

    public void PlayDialogue()
    {
        StopAllCoroutines();
        StartCoroutine(StartDialogue());
    }

    void Update()
    {
        if (isTyping)
        {
            float bounce = Mathf.Sin(Time.time * bounceSpeed) * bounceAmplitude;
            portraitRect.anchoredPosition = portraitStartPos + Vector2.up * bounce;

            float squash = Mathf.Sin(Time.time * squashSpeed) * squashAmount;
            portraitRect.localScale = new Vector3(
                portraitStartScale.x + squash,
                portraitStartScale.y - squash,
                portraitStartScale.z
            );
        }
    }

    IEnumerator StartDialogue()
    {
        yield return new WaitForSeconds(startDelay);
        yield return StartCoroutine(TypeText(dialogueLine));

        yield return new WaitForSeconds(timeBeforeFade);
        yield return StartCoroutine(FadeOut());
    }

    IEnumerator TypeText(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;

            if (!char.IsWhiteSpace(c))
            {
                if (audioSource != null && typeSound != null)
                {
                    audioSource.pitch = Random.Range(0.95f, 1.05f);
                    audioSource.PlayOneShot(typeSound);
                }
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        portraitRect.anchoredPosition = portraitStartPos;
        portraitRect.localScale = portraitStartScale;
    }

    IEnumerator FadeOut()
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (objectiveUI != null)
        {
            objectiveUI.ShowObjective(objectiveText);
        }

        gameObject.SetActive(false);
    }

    void SetEmotionPortrait()
    {
        switch (currentEmotion)
        {
            case Emotion.Neutral:
                portraitImage.sprite = neutralSprite;
                break;
            case Emotion.Anger:
                portraitImage.sprite = angerSprite;
                break;
            case Emotion.Disgust:
                portraitImage.sprite = disgustSprite;
                break;
            case Emotion.Scared:
                portraitImage.sprite = scaredSprite;
                break;
            case Emotion.Confused:
                portraitImage.sprite = confusedSprite;
                break;
        }
    }
}
