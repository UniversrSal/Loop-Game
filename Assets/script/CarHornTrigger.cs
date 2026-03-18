using UnityEngine;

public class CarHornTrigger : MonoBehaviour
{
    public AudioSource hornAudio;

    void OnTriggerEnter(Collider other)
    {
        // Check if the object we hit has the tag "Player"
        if (other.CompareTag("Player"))
        {
            if (hornAudio != null)
                hornAudio.Play(); // Play the horn
        }
    }
}
