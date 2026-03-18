using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class QTESystem : MonoBehaviour
{
    // -----------------------------
    // QTE Settings
    // -----------------------------
    private KeyCode key;
    private float timer;
    private Action<bool> onResult;

    [Header("Overlay & Timing UI")]
    public GameObject qteOverlay;
    public GameObject timingContainer;

    [Header("Moving Circle")]
    public RectTransform movingCircle;
    public RectTransform successZone;
    public float moveSpeed = 500f;

    private float minX;
    private float maxX;
    private int direction = 1;

    [Header("Result UI")]
    public GameObject successImage;
    public GameObject failImage;

    [Header("Audio")]
    public AudioSource successSound;
    public AudioSource failSound;

    private bool qteActive = false;
    public static bool QTEActive = false;

    // -----------------------------
    // INITIALIZATION
    // -----------------------------
    void Awake()
    {
        HideAllUI();
    }

    void Start()
    {
        if (movingCircle != null)
        {
            // Optionally, use the parent width instead of hardcoded values
            minX = -300f;
            maxX = 300f;
        }
    }

    // -----------------------------
    // UPDATE LOOP
    // -----------------------------
    void Update()
    {
        if (!qteActive) return;

        // Countdown timer
        if (timer > 0)
        {
            timer -= Time.unscaledDeltaTime;

            if (timer <= 0)
                EndQTE(false);
        }

        MoveCircle();

        // Check input
        if (Input.GetKeyDown(key))
            CheckTiming();
    }

    // -----------------------------
    // START QTE
    // -----------------------------
    public void StartTimingQTE(KeyCode key, float timeout, Action<bool> callback)
    {
        ResetQTE();

        QTEActive = true;
        this.key = key;
        timer = timeout;
        onResult = callback;
        qteActive = true;

        ShowUI(qteOverlay, true);
        ShowUI(timingContainer, true);
        ShowUI(successImage, false);
        ShowUI(failImage, false);

        Time.timeScale = 0f;

        // Reset circle position
        if (movingCircle != null)
            movingCircle.anchoredPosition = new Vector2(minX, movingCircle.anchoredPosition.y);
    }

    // -----------------------------
    // MOVE CIRCLE
    // -----------------------------
    void MoveCircle()
    {
        if (movingCircle == null) return;

        Vector2 pos = movingCircle.anchoredPosition;
        pos.x += direction * moveSpeed * Time.unscaledDeltaTime;

        if (pos.x >= maxX)
        {
            pos.x = maxX;
            direction = -1;
        }
        else if (pos.x <= minX)
        {
            pos.x = minX;
            direction = 1;
        }

        movingCircle.anchoredPosition = pos;
    }

    // -----------------------------
    // CHECK TIMING
    // -----------------------------
    void CheckTiming()
    {
        if (movingCircle == null || successZone == null) return;

        float circleX = movingCircle.anchoredPosition.x;
        float zoneMin = successZone.anchoredPosition.x - (successZone.rect.width / 2);
        float zoneMax = successZone.anchoredPosition.x + (successZone.rect.width / 2);

        bool success = circleX >= zoneMin && circleX <= zoneMax;
        EndQTE(success);
    }

    // -----------------------------
    // END QTE
    // -----------------------------
    void EndQTE(bool success)
    {
        qteActive = false;

        ShowUI(timingContainer, false);

        if (success)
        {
            if (successSound != null) successSound.Play();
            ShowUI(successImage, true);
        }
        else
        {
            if (failSound != null) failSound.Play();
            ShowUI(failImage, true);
        }

        onResult?.Invoke(success);
        StartCoroutine(EndQTECoroutine());
    }

    IEnumerator EndQTECoroutine()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        ResetQTE();
    }

    // -----------------------------
    // HELPERS
    // -----------------------------
    void ResetQTE()
    {
        qteActive = false;
        timer = 0;
        QTEActive = false;

        HideAllUI();
        Time.timeScale = 1f;
    }

    void ShowUI(GameObject obj, bool show)
    {
        if (obj != null)
            obj.SetActive(show);
    }

    void HideAllUI()
    {
        ShowUI(qteOverlay, false);
        ShowUI(timingContainer, false);
        ShowUI(successImage, false);
        ShowUI(failImage, false);
    }
}