using UnityEngine;

public class CarFollowPath : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 10f;
    public float rotationSpeed = 5f;
    private int currentWaypoint = 0;

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypoint];
        Vector3 direction = (target.position - transform.position).normalized;

        // Rotate towards the target
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        // Move forward
        transform.position += transform.forward * speed * Time.deltaTime;

        // Check if reached the waypoint
        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }
}
