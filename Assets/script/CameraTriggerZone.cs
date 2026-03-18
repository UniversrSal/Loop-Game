using UnityEngine;

public class CameraTriggerZone : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;           // Camera to override
    public Transform[] cameraPoints;    // Array of positions/rotations
    public float speed = 5f;            // How fast the camera moves

    private bool isActive = false;
    private int currentPointIndex = 0;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Save original camera transform
        originalPosition = mainCamera.transform.position;
        originalRotation = mainCamera.transform.rotation;
    }

    void LateUpdate()
    {
        if (isActive && cameraPoints.Length > 0)
        {
            Transform target = cameraPoints[currentPointIndex];

            // Smoothly move and rotate
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                target.position,
                speed * Time.deltaTime
            );

            mainCamera.transform.rotation = Quaternion.Lerp(
                mainCamera.transform.rotation,
                target.rotation,
                speed * Time.deltaTime
            );

            // Check if reached the current point
            if (Vector3.Distance(mainCamera.transform.position, target.position) < 0.05f &&
                Quaternion.Angle(mainCamera.transform.rotation, target.rotation) < 0.5f)
            {
                currentPointIndex++;
                if (currentPointIndex >= cameraPoints.Length)
                {
                    // Finished path
                    isActive = false;
                    currentPointIndex = 0;
                }
            }
        }
        else if (!isActive)
        {
            // Optionally return to original camera
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                originalPosition,
                speed * Time.deltaTime
            );

            mainCamera.transform.rotation = Quaternion.Lerp(
                mainCamera.transform.rotation,
                originalRotation,
                speed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isActive = true;
            currentPointIndex = 0;
        }
    }
}
