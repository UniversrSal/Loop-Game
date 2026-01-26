using UnityEngine;
using UnityEngine.InputSystem;

public class CharController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float moveSmoothTime = 0.12f;       // Smooth movement
    [SerializeField] float rotationSmoothTime = 0.12f;   // Smooth turning
    [SerializeField] float inputSmoothTime = 0.1f;       // Smooth input

    [Header("Animator")]
    [SerializeField] Animator animator;

    [Header("Footsteps")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip footstep;
    [SerializeField] float stepInterval = 0.4f;

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

        // New Input System setup
        inputActions = new PlayerInputActions();
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        SmoothInput();
        MoveCharacter();
        HandleFootsteps();
    }

    private void SmoothInput()
    {
        // Smooth the raw input to avoid jumps
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
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // Convert 2D input to 3D world movement
        Vector3 inputDir = new Vector3(currentInput.x, 0f, currentInput.y);
        float inputMag = Mathf.Clamp01(inputDir.magnitude);

        Vector3 desiredDirection = (camRight * inputDir.x + camForward * inputDir.z).normalized;
        Vector3 targetVelocity = desiredDirection * moveSpeed * inputMag;

        // Smooth velocity
        currentVelocity = Vector3.SmoothDamp(
            currentVelocity,
            targetVelocity,
            ref velocityRef,
            moveSmoothTime
        );

        // Keep grounded (prevents small floating)
        Vector3 move = currentVelocity + Vector3.down * 2f;
        controller.Move(move * Time.deltaTime);

        // Smooth rotation towards movement direction
        if (currentVelocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(currentVelocity);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                (360f / rotationSmoothTime) * Time.deltaTime
            );
        }

        // Update animator
        if (animator)
        {
            animator.SetFloat("Speed", inputMag, 0.1f, Time.deltaTime);
        }
    }

    private void HandleFootsteps()
    {
        bool isMoving = currentVelocity.magnitude > 0.1f;
        bool isGrounded = controller.isGrounded;

        if (!isMoving || !isGrounded)
        {
            stepTimer = 0f;
            return;
        }

        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0f)
        {
            if (audioSource && footstep)
                audioSource.PlayOneShot(footstep);
            stepTimer = stepInterval;
        }
    }
}
