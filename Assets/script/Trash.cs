using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TrashPickupController : MonoBehaviour
{
    [Header("References")]
    public Transform handTransform;       // Hand bone to follow
    public Animator playerAnimator;       // Player Animator
    public GameObject trash;              // Trash object
    public GameObject pickUpPromptUI;    // UI for "Press X"
    public AudioSource trashPickupSFX;    // Pickup sound
    public Collider apartmentExitTrigger; // Collider that blocks exit

    [Header("Settings")]
    public string pickUpAnimationTrigger = "PickUp"; // Animator trigger
    public KeyCode pickUpKey = KeyCode.X;           // Key to pick up trash
    public float animationStartDelay = 1f;          // Wait after pressing X before animation starts
    public float attachDelay = 0.5f;                // Delay after animation starts before attaching trash

    private bool isInTrigger = false;    // Player is near trash
    private bool trashPickedUp = false;  // Trash already picked up
    private bool canLeave = false;       // True after picking up trash
    private bool trashAttached = false;  // Tracks if trash is following the hand

    void Start()
    {
        pickUpPromptUI.SetActive(false);

        if (apartmentExitTrigger != null)
            apartmentExitTrigger.enabled = false; // block exit initially
    }

    void Update()
    {
        if (isInTrigger && !trashPickedUp)
        {
            pickUpPromptUI.SetActive(true);

            if (Input.GetKeyDown(pickUpKey))
            {
                trashPickedUp = true;
                pickUpPromptUI.SetActive(false);

                // Start the pickup process coroutine
                StartCoroutine(PickUpTrashRoutine());
            }
        }
        else
        {
            pickUpPromptUI.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        // Keep trash following the hand's position without rotating
        if (trashAttached && trash != null && handTransform != null)
        {
            trash.transform.position = handTransform.position;
            // Rotation stays untouched
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == trash)
            isInTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == trash)
            isInTrigger = false;
    }

    private IEnumerator PickUpTrashRoutine()
    {
        // Wait before animation starts
        yield return new WaitForSeconds(animationStartDelay);

        // Trigger pick-up animation
        playerAnimator.SetTrigger(pickUpAnimationTrigger);

        // Wait attachDelay before snapping trash to hand
        yield return new WaitForSeconds(attachDelay);

        if (trash != null && handTransform != null)
        {
            // Snap trash to hand's position immediately
            trash.transform.position = handTransform.position;

            // Mark trash to follow hand in LateUpdate
            trashAttached = true;

            // Play pickup sound
            if (trashPickupSFX != null)
                trashPickupSFX.Play();
        }

        // Unlock exit
        canLeave = true;
        if (apartmentExitTrigger != null)
            apartmentExitTrigger.enabled = true;
    }
}
