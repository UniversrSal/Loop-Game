using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class FootstepSet
{
    public string surfaceTag;
    public AudioClip[] clips;
}

public class CharController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float moveSmoothTime = 0.12f;
    [SerializeField] float rotationSmoothTime = 0.12f;
    [SerializeField] float inputSmoothTime = 0.1f;

    [Header("Animator")]
    [SerializeField] Animator animator;

    [Header("Footsteps")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] float stepInterval = 0.4f;
    [SerializeField] FootstepSet[] footstepSets;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckDistance = 1.5f;

    // --- CharacterController ---
    private CharacterController controller;
    private Vector3 currentVelocity;
    private Vector3 velocityRef;

    // --- Input System ---
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private Vector2 currentInput;
    private Vector2 inputVelocity;

    // --- Footstep Timer ---
    private float stepTimer;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        inputActions = new PlayerInputActions();
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Update()
    {
        SmoothInput();
        MoveCharacter();
        HandleFootsteps();
    }

    private void SmoothInput()
    {
        // Smooth the raw input to avoid sudden jumps
        currentInput = Vector2.SmoothDamp(
            currentInput,
            moveInput,
            ref inputVelocity,
            inputSmoothTime
        );
    }

    private void MoveCharacter()
    {
        // Camera-relative movement
        Vector3 camForward = Camera.main ? Camera.main.transform.forward : Vector3.forward;
        Vector3 camRight = Camera.main ? Camera.main.transform.right : Vector3.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // Convert 2D input to 3D world movement
        Vector3 inputDir = new Vector3(currentInput.x, 0, currentInput.y);
        float inputMag = Mathf.Clamp01(inputDir.magnitude);

        Vector3 desiredDir = (camRight * inputDir.x + camForward * inputDir.z).normalized;
        Vector3 targetVelocity = desiredDir * moveSpeed * inputMag;

        // Smooth movement
        currentVelocity = Vector3.SmoothDamp(
            currentVelocity,
            targetVelocity,
            ref velocityRef,
            moveSmoothTime
        );

        // Move character while keeping grounded
        Vector3 move = currentVelocity + Vector3.down * 2f;
        controller.Move(move * Time.deltaTime);

        // --- Free rotation based on input ---
        if (inputDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(desiredDir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                360f * Time.deltaTime / rotationSmoothTime
            );
        }

        // Update animator
        if (animator)
            animator.SetFloat("Speed", inputMag, 0.1f, Time.deltaTime);
    }

    private void HandleFootsteps()
    {
        if (!controller.isGrounded || currentVelocity.magnitude < 0.1f)
        {
            stepTimer = 0f;
            return;
        }

        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0f)
        {
            PlaySurfaceFootstep();
            stepTimer = stepInterval;
        }
    }

    private void PlaySurfaceFootstep()
    {
        if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer))
            return;

        foreach (FootstepSet set in footstepSets)
        {
            if (hit.collider.CompareTag(set.surfaceTag) && set.clips.Length > 0)
            {
                audioSource.PlayOneShot(set.clips[Random.Range(0, set.clips.Length)]);
                return;
            }
        }
    }
}
