using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadTrigger : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Enter the build index of the scene to load.")]
    public int sceneIndexToLoad = 0; // Set in Inspector

    [Header("Trigger Settings")]
    public string playerTag = "Player"; // Who triggers the scene load

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            // Check if the scene index is valid
            if (sceneIndexToLoad >= 0 && sceneIndexToLoad < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(sceneIndexToLoad);
            }
            else
            {
                Debug.LogError("Scene index " + sceneIndexToLoad + " is not in Build Settings!");
            }
        }
    }
}
