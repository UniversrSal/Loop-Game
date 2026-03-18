using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [Header("Camera Follow Reference")]
    public CameraFollow cameraFollow; // Reference to your CameraFollow script

    [Header("Override Transform")]
    public Vector3 overridePosition;  // Camera position to use in the trigger
    public Vector3 overrideRotation;  // Camera rotation to use in the trigger (Euler angles)

    // Store original camera state
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool originalFollowState;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || cameraFollow == null)
            return;

        // Save current camera state
        originalPosition = cameraFollow.transform.position;
        originalRotation = cameraFollow.transform.rotation;
        originalFollowState = cameraFollow.followEnabled;

        // Disable camera follow and apply override
        cameraFollow.followEnabled = false;
        cameraFollow.transform.position = overridePosition;
        cameraFollow.transform.rotation = Quaternion.Euler(overrideRotation);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || cameraFollow == null)
            return;

        // Restore original camera state
        cameraFollow.transform.position = originalPosition;
        cameraFollow.transform.rotation = originalRotation;
        cameraFollow.followEnabled = originalFollowState;
    }
}
