using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 defaultOffset = new Vector3(0, 5, -10);

    public bool followEnabled = true;

    private Vector3 currentOffset;

    private void Start()
    {
        currentOffset = defaultOffset;
    }

    private void LateUpdate()
    {
        if (!followEnabled || target == null) return;

        transform.position = target.position + currentOffset;
        transform.LookAt(target);
    }

    public void SetCameraOffset(Vector3 newOffset)
    {
        currentOffset = newOffset;
        transform.position = target.position + currentOffset;
        transform.LookAt(target);
    }

    // ✅ ADD THIS
    public void ResetOffset()
    {
        SetCameraOffset(defaultOffset);
    }
}
