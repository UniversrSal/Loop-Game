using UnityEngine;

public class Spinner : MonoBehaviour
{
    [Header("Rotation")]
    public float rotationSpeed = 90f; // degrees per second

    [Header("Bobbing")]
    public float bobHeight = 0.25f;   // how high it moves
    public float bobSpeed = 2f;       // how fast it bobs

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Rotate continuously (Y-axis)
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

        // Bob up and down using sine wave
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(
            startPosition.x,
            newY,
            startPosition.z
        );
    }
}
