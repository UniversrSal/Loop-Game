using UnityEngine;

public class CameraFollowOverride : MonoBehaviour
{
    [Header("Camera Follow Reference")]
    public CameraFollow cameraFollow; // Reference to your CameraFollow script

    [Header("Override Transform")]
    public Vector3 overridePosition;  // Offset from player position
    public Vector3 overrideRotation;  // Rotation offset (Euler angles)

    [Header("Follow Settings")]
    public float smoothSpeed = 5f;    // How smoothly the camera follows

    private bool inTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || cameraFollow == null)
            return;

        // Enable trigger override
        inTrigger = true;
        cameraFollow.followEnabled = false; // Disable normal camera follow
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || cameraFollow == null)
            return;

        // Disable trigger override
        inTrigger = false;
        cameraFollow.followEnabled = true; // Re-enable normal camera follow
    }

    private void LateUpdate()
    {
        if (inTrigger && cameraFollow != null && cameraFollow.target != null)
        {
            // Compute desired position and rotation relative to player
            Vector3 targetPos = cameraFollow.target.position + overridePosition;
            Quaternion targetRot = Quaternion.Euler(overrideRotation);

            // Smoothly move camera
            cameraFollow.transform.position = Vector3.Lerp(
                cameraFollow.transform.position,
                targetPos,
                smoothSpeed * Time.deltaTime
            );

            cameraFollow.transform.rotation = Quaternion.Slerp(
                cameraFollow.transform.rotation,
                targetRot,
                smoothSpeed * Time.deltaTime
            );
        }
    }
}
