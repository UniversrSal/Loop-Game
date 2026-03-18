using UnityEngine;
using System.Collections;

public class Throwaway : MonoBehaviour
{
    [Header("References")]
    public Animator playerAnimator;
    public GameObject trashcan;
    public GameObject throwawayPromptUI;
    public AudioSource throwawaySFX;
    public Collider apartmentHomeTrigger;
    public GameObject trash;   // the trash bag

    [Header("Settings")]
    public string throwAwayAnimationTrigger = "ThrowAway";
    public KeyCode throwAwayKey = KeyCode.X;
    public float animationStartDelay = 0.2f;

    private bool isInTrigger = false;
    private bool isThrowingAway = false;

    void Start()
    {
        if (throwawayPromptUI != null)
            throwawayPromptUI.SetActive(false);

        if (apartmentHomeTrigger != null)
            apartmentHomeTrigger.enabled = false;
    }

    void Update()
    {
        if (isInTrigger && !isThrowingAway)
        {
            throwawayPromptUI.SetActive(true);

            if (Input.GetKeyDown(throwAwayKey))
            {
                throwawayPromptUI.SetActive(false);
                StartCoroutine(ThrowAwayRoutine());
            }
        }
        else
        {
            throwawayPromptUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == trashcan)
            isInTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == trashcan)
            isInTrigger = false;
    }

    private IEnumerator ThrowAwayRoutine()
    {
        isThrowingAway = true;

        // Play animation
        if (playerAnimator != null)
            playerAnimator.SetTrigger(throwAwayAnimationTrigger);

        yield return new WaitForSeconds(animationStartDelay);

       

        yield return new WaitForSeconds(1.5f);
        Destroy(trash);


        // Play sound
        if (throwawaySFX != null)
            throwawaySFX.Play();

        // Unlock exit
        if (apartmentHomeTrigger != null)
            apartmentHomeTrigger.enabled = true;
    }
}
