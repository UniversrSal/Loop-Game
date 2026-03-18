using UnityEngine;
using System.Collections;

public class SpeechBublbleTrigger : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup speechBubble;
    public float visibleTime = 5f;
    public float fadeSpeed = 2f;

    private bool hasTriggered = false;

    void Start()
    {
        if (speechBubble != null)
        {
            speechBubble.alpha = 0f;
            speechBubble.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(ShowSpeechBubble());
        }
    }

    IEnumerator ShowSpeechBubble()
    {
        speechBubble.gameObject.SetActive(true);

        // Fade in
        while (speechBubble.alpha < 1f)
        {
            speechBubble.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        speechBubble.alpha = 1f;

        // Wait while visible
        yield return new WaitForSeconds(visibleTime);

        // Fade out
        while (speechBubble.alpha > 0f)
        {
            speechBubble.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        speechBubble.alpha = 0f;
        speechBubble.gameObject.SetActive(false);
    }
}
