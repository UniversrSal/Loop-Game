using UnityEngine;

public class CameraStopTrigger : MonoBehaviour
{
    public CameraFollow cameraFollow;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && cameraFollow != null)
        {
            cameraFollow.followEnabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && cameraFollow != null)
        {
            cameraFollow.followEnabled = true;
        }
    }
}
