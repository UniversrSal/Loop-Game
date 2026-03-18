using UnityEngine;
using TMPro;

public class RealTimeTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public int startHours = 2;
    public int startMinutes = 32;
    public int startSeconds = 29;
    public bool countDown = false;  // false = count up

    [Header("UI")]
    public TMP_Text timerText;      // Assign your TextMeshPro text here

    private float currentTime;

    void Start()
    {
        // Convert start time to total seconds
        currentTime = startHours * 3600 + startMinutes * 60 + startSeconds;
        UpdateTimerText();
    }

    void Update()
    {
        // Count up or down in real time
        currentTime += Time.unscaledDeltaTime * (countDown ? -1f : 1f);

        if (countDown && currentTime <= 0f)
        {
            currentTime = 0f;
            // Optional: trigger an event when timer reaches 0
        }

        UpdateTimerText();
    }

    void UpdateTimerText()
    {
        int hours = Mathf.FloorToInt(currentTime / 3600f);
        int minutes = Mathf.FloorToInt((currentTime % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        int milliseconds = Mathf.FloorToInt((currentTime * 1000f) % 1000f); // 3 digits

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}:{3:000}", hours, minutes, seconds, milliseconds);
    }
}
