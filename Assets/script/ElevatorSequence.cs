using System.Collections;
using UnityEngine;

public class ElevatorSequence : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject pressButtonUI;
    public DialogueController dialogueController; // Your existing DialogueController
    public Transform leftDoor;
    public Transform rightDoor;
    public AudioSource floorBeepAudio;
    public AudioSource elevatorMoveAudio;
    public Camera mainCamera;

    [Header("Voice Announcement")]
    public AudioSource floorAnnouncementAudio; // 7th floor voice clip

    [Header("Elevator Settings")]
    public KeyCode interactKey = KeyCode.X;
    public float doorOpenDistance = 3f; // Doors open along X axis
    public float doorOpenSpeed = 1f;
    public float beepInterval = 2f;
    public int totalFloors = 7;

    [Header("Camera Shake Settings")]
    public float shakeAmount = 0.2f;
    public float shakeDecayTime = 1.5f;

    [Header("Dialogue Settings")]
    public int dialogueMark = 3; // Dialogue starts at this floor

    private bool playerInTrigger = false;
    private bool elevatorStarted = false;

    private Vector3 leftDoorStartPos;
    private Vector3 rightDoorStartPos;

    private Vector3 cameraOriginalPos;
    private bool isShaking = false;
    private float currentShakeAmount = 0f;

    private void Start()
    {
        leftDoorStartPos = leftDoor.position;
        rightDoorStartPos = rightDoor.position;
        pressButtonUI.SetActive(false);

        if (mainCamera != null)
            cameraOriginalPos = mainCamera.transform.localPosition;

        // Keep DialogueController inactive until triggered
        if (dialogueController != null)
            dialogueController.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Start elevator sequence when player presses X
        if (playerInTrigger && !elevatorStarted && Input.GetKeyDown(interactKey))
        {
            elevatorStarted = true;
            pressButtonUI.SetActive(false);
            StartCoroutine(StartElevatorSequence());
        }

        // Camera shake
        if (isShaking && mainCamera != null)
        {
            Vector3 shakeOffset = Random.insideUnitSphere * currentShakeAmount;
            shakeOffset.z = 0f;
            mainCamera.transform.localPosition = cameraOriginalPos + shakeOffset;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (elevatorStarted) return; // Ignore after elevator started
        if (other.transform == player)
        {
            playerInTrigger = true;
            pressButtonUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (elevatorStarted) return; // Ignore after elevator started
        if (other.transform == player)
        {
            playerInTrigger = false;
            pressButtonUI.SetActive(false);
        }
    }

    private IEnumerator StartElevatorSequence()
    {
        // Start elevator audio and camera shake
        elevatorMoveAudio.Play();
        isShaking = true;
        currentShakeAmount = shakeAmount;

        for (int floor = 1; floor <= totalFloors; floor++)
        {
            floorBeepAudio.Play();

            // Trigger dialogue at specified floor
            if (floor == dialogueMark && dialogueController != null)
                dialogueController.gameObject.SetActive(true);

            // Stop elevator audio and play 7th floor voice
            if (floor == totalFloors)
            {
                elevatorMoveAudio.Stop();

                if (floorAnnouncementAudio != null)
                    floorAnnouncementAudio.Play();
            }

            yield return new WaitForSeconds(beepInterval);
        }

        // Smoothly stop camera shake
        yield return StartCoroutine(StopShakeSmoothly());

        // Wait before doors start opening
        yield return new WaitForSeconds(5f);

        // Open doors slowly with struggle pause
        StartCoroutine(OpenDoorsStruggle());
    }

    private IEnumerator OpenDoorsStruggle()
    {
        Vector3 leftHalf = leftDoorStartPos + new Vector3(-doorOpenDistance / 2f, 0, 0);
        Vector3 rightHalf = rightDoorStartPos + new Vector3(doorOpenDistance / 2f, 0, 0);
        Vector3 leftFull = leftDoorStartPos + new Vector3(-doorOpenDistance, 0, 0);
        Vector3 rightFull = rightDoorStartPos + new Vector3(doorOpenDistance, 0, 0);

        float t = 0f;

        // First half
        while (t < 1f)
        {
            t += Time.deltaTime * doorOpenSpeed;
            leftDoor.position = Vector3.Lerp(leftDoorStartPos, leftHalf, t);
            rightDoor.position = Vector3.Lerp(rightDoorStartPos, rightHalf, t);
            yield return null;
        }

        // Pause to simulate struggle
        yield return new WaitForSeconds(1.5f);

        // Second half
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * doorOpenSpeed;
            leftDoor.position = Vector3.Lerp(leftHalf, leftFull, t);
            rightDoor.position = Vector3.Lerp(rightHalf, rightFull, t);
            yield return null;
        }
    }

    private IEnumerator StopShakeSmoothly()
    {
        float elapsed = 0f;
        float startingAmount = currentShakeAmount;

        while (elapsed < shakeDecayTime)
        {
            elapsed += Time.deltaTime;
            currentShakeAmount = Mathf.Lerp(startingAmount, 0f, elapsed / shakeDecayTime);
            yield return null;
        }

        currentShakeAmount = 0f;
        isShaking = false;

        if (mainCamera != null)
            mainCamera.transform.localPosition = cameraOriginalPos;
    }
}
