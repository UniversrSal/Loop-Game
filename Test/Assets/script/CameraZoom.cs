using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoom : MonoBehaviour
{
    [Header("Zoom Settings")]
    public Camera cam;                  // Assign your Camera
    public float zoomSpeed = 30f;       // How fast FOV changes
    public float minFOV = 20f;          // Zoom in
    public float maxFOV = 60f;          // Zoom out

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip zoomSound;
    public float volume = 0.5f;

    [Header("Input Settings")]
    public float zoomStep = 2f;         // How much to change per frame

    private PlayerInputActions inputActions;
    private float targetFOV;
    private bool zoomInPressed;
    private bool zoomOutPressed;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
        targetFOV = cam.fieldOfView;

        // Input System setup
        inputActions = new PlayerInputActions();

        // Zoom In button
        inputActions.Player.Zoom.performed += ctx =>
        {
            string key = ctx.control.displayName.ToLower();
            if (key == "q") zoomInPressed = true;
            if (key == "e") zoomOutPressed = true;
        };
        inputActions.Player.Zoom.canceled += ctx =>
        {
            string key = ctx.control.displayName.ToLower();
            if (key == "q") zoomInPressed = false;
            if (key == "e") zoomOutPressed = false;
        };
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Update()
    {
        // Adjust target FOV based on held keys
        if (zoomInPressed)
        {
            targetFOV -= zoomStep * Time.deltaTime * zoomSpeed;
            PlayZoomSound();
        }
        if (zoomOutPressed)
        {
            targetFOV += zoomStep * Time.deltaTime * zoomSpeed;
            PlayZoomSound();
        }

        targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);

        // Smooth interpolation
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }

    private void PlayZoomSound()
    {
        if (audioSource && zoomSound && !audioSource.isPlaying)
            audioSource.PlayOneShot(zoomSound, volume);
    }
}
