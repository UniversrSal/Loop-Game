using UnityEngine;
using System.Collections;

public class GunPickup : MonoBehaviour
{
    [Header("References")]
    public GameObject gunObject;              // The gun in the world
    public GameObject pickupPromptUI;         // "Press E" UI
    public GameObject pickupImageUI;          // Image that pops up
    public AudioSource pickupSFX;

    [Header("Dialogue")]
    public DialogueController dialogue;       // Your dialogue script

    [Header("Player")]
    public MonoBehaviour playerMovement;      // Your movement script

    [Header("Animation")]
    public Animator playerAnimator;
    public string pickupTrigger = "Pickup";   // your trigger name
    public string pickupStateName = "Pickup"; // EXACT animator state name

    [Header("Settings")]
    public KeyCode pickupKey = KeyCode.E;
    public float imageDisplayTime = 1.5f;

    private bool isInTrigger = false;
    private bool hasPickedUp = false;

    void Start()
    {
        if (pickupPromptUI != null)
            pickupPromptUI.SetActive(false);

        if (pickupImageUI != null)
            pickupImageUI.SetActive(false);
    }

    void Update()
    {
        if (isInTrigger && !hasPickedUp)
        {
            if (pickupPromptUI != null)
                pickupPromptUI.SetActive(true);

            if (Input.GetKeyDown(pickupKey))
            {
                pickupPromptUI.SetActive(false);
                StartCoroutine(PickupRoutine());
            }
        }
        else
        {
            if (pickupPromptUI != null)
                pickupPromptUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isInTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isInTrigger = false;
    }

    private IEnumerator PickupRoutine()
{
    hasPickedUp = true;

    // Freeze player movement
    if (playerMovement != null)
        playerMovement.enabled = false;

    // Trigger animation
    if (playerAnimator != null)
        playerAnimator.SetTrigger(pickupTrigger);

    // Wait until animation STARTS
    yield return null;

    // Wait until we ENTER the pickup animation
    while (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(pickupStateName))
        yield return null;

    // Wait until animation FINISHES
    while (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(pickupStateName))
        yield return null;

    // 🔫 Destroy gun AFTER animation ends
    if (gunObject != null)
        Destroy(gunObject);

    // Play sound
    if (pickupSFX != null)
        pickupSFX.Play();

    // Show pickup image
    if (pickupImageUI != null)
        pickupImageUI.SetActive(true);

    yield return new WaitForSeconds(imageDisplayTime);

    if (pickupImageUI != null)
        pickupImageUI.SetActive(false);

    // Start dialogue
    if (dialogue != null)
    {
        dialogue.gameObject.SetActive(true);
        dialogue.PlayDialogue();
    }
}
}