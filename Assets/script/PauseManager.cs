using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManagerWithSound : MonoBehaviour
{
    [Header("UI Canvas")]
    [SerializeField] private GameObject pauseMenu;

    [Header("Audio")]
    [SerializeField] private AudioSource pauseAudioSource;
    [SerializeField] private AudioClip pauseSound;
    [SerializeField] private AudioClip unpauseSound;

    [Header("Gameplay Scripts to Disable")]
    [SerializeField] private MonoBehaviour[] gameplayScripts;

    public static bool IsGamePaused = false;

    private bool isPaused = false;   // <-- REQUIRED


    private void Awake()
    {
        if (pauseMenu)
            pauseMenu.SetActive(false);

        // Ensure we have an AudioSource for pause/unpause sounds
        if (pauseAudioSource == null && (pauseSound || unpauseSound))
        {
            pauseAudioSource = gameObject.AddComponent<AudioSource>();
            pauseAudioSource.playOnAwake = false;
            pauseAudioSource.ignoreListenerPause = true; // This makes the sound play even if game is paused
        }
    }

    private void Update()
    {
        // P key using Input System
        if (Keyboard.current.pKey.wasPressedThisFrame && !QTESystem.QTEActive)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
       isPaused = !isPaused;
       IsGamePaused = isPaused;

        // Show/hide menu
        if (pauseMenu)
            pauseMenu.SetActive(isPaused);

        // Lock/unlock cursor
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;

        // Enable/disable gameplay scripts
        foreach (var script in gameplayScripts)
        {
            if (script != null)
                script.enabled = !isPaused;
        }

        // Freeze time
        Time.timeScale = isPaused ? 0f : 1f;

        // Pause/unpause all other audio
        AudioListener.pause = isPaused;

        // Play pause/unpause sound (ignores AudioListener.pause)
        if (pauseAudioSource)
        {
            if (isPaused && pauseSound)
                pauseAudioSource.PlayOneShot(pauseSound);
            else if (!isPaused && unpauseSound)
                pauseAudioSource.PlayOneShot(unpauseSound);
        }
    }

    // Optional: UI button
    public void ResumeGame() => TogglePause();
}
