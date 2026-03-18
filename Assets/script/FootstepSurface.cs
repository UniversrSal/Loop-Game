using UnityEngine;

public class FootstepSurface : MonoBehaviour
{
    [SerializeField] AudioClip[] footstepClips;
    [SerializeField] float stepInterval = 0.4f;

    private float stepTimer;

    private void OnTriggerStay(Collider other)
    {
        CharacterController controller = other.GetComponent<CharacterController>();
        AudioSource audioSource = other.GetComponent<AudioSource>();

        if (!controller || !audioSource)
            return;

        if (!controller.isGrounded || controller.velocity.magnitude < 0.1f)
        {
            stepTimer = 0f;
            return;
        }

        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0f)
        {
            audioSource.PlayOneShot(
                footstepClips[Random.Range(0, footstepClips.Length)]
            );
            stepTimer = stepInterval;
        }
    }
}
