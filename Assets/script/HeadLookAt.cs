using UnityEngine;

public class HeadLookAt : MonoBehaviour
{
    public Transform player;
    public float lookWeight = 1f;
    public float bodyWeight = 0.1f;
    public float headWeight = 1f;
    public float eyesWeight = 1f;
    public float clampWeight = 0.5f;
    public float smoothSpeed = 5f;

    private Animator animator;
    private float currentWeight;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null || player == null) return;

        // Smoothly blend in
        currentWeight = Mathf.Lerp(currentWeight, lookWeight, Time.deltaTime * smoothSpeed);

        animator.SetLookAtWeight(
            currentWeight,
            bodyWeight,
            headWeight,
            eyesWeight,
            clampWeight
        );

        animator.SetLookAtPosition(player.position + Vector3.up * 1.6f);
    }
}
