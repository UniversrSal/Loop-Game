using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [Header("Camera Follow Reference")]
    public CameraFollow cameraFollow; // Reference to your CameraFollow script

    [Header("Override Transform")]
    public Vector3 overridePosition;  // Camera position to use in the trigger
    public Vector3 overrideRotation;  // Camera rotation to use in the trigger (Euler angles)

    [Header("Shake Settings")]
    public float shakeMagnitude = 0.2f;  // How strong the shake is
    public float shakeSpeed = 25f;       // How fast it shakes

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool originalFollowState;

    private bool isShaking = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || cameraFollow == null)
            return;

        // Save current camera state
        originalPosition = cameraFollow.transform.position;
        originalRotation = cameraFollow.transform.rotation;
        originalFollowState = cameraFollow.followEnabled;

        // Disable camera follow and set base override
        cameraFollow.followEnabled = false;
        cameraFollow.transform.position = overridePosition;
        cameraFollow.transform.rotation = Quaternion.Euler(overrideRotation);

        // Start shaking
        isShaking = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || cameraFollow == null)
            return;

        // Stop shaking
        isShaking = false;

        // Restore original camera state
        cameraFollow.transform.position = originalPosition;
        cameraFollow.transform.rotation = originalRotation;
        cameraFollow.followEnabled = originalFollowState;
    }

    private void LateUpdate()
    {
        if (isShaking)
        {
            // Apply shake offset using Perlin noise for smooth, natural shake
            float offsetX = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f) * 2f * shakeMagnitude;
            float offsetY = (Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f) * 2f * shakeMagnitude;
            float offsetZ = (Mathf.PerlinNoise(Time.time * shakeSpeed, Time.time * shakeSpeed) - 0.5f) * 2f * shakeMagnitude;

            cameraFollow.transform.position = overridePosition + new Vector3(offsetX, offsetY, offsetZ);
        }
    }
}
