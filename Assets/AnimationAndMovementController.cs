using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class AnimationAndMovementController : MonoBehaviour
{
    private PlayerInput playerInput;
    private CharacterController characterController;
    private Animator animator;

    [SerializeField] private Transform cameraTransform;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 2.5f;
    [SerializeField] private float runMultiplier = 2.5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Mouse")]
    [SerializeField] private float sensX = 3f;

    private Vector2 moveInput;
    private bool isRunPressed;
    private float verticalVelocity;

    void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // SOLO per Run (digitale → sicuro)
        playerInput.CharacterControls.Run.started += _ => isRunPressed = true;
        playerInput.CharacterControls.Run.canceled += _ => isRunPressed = false;

        // SICUREZZA ASSOLUTA
        animator.applyRootMotion = false;
    }

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        ReadMovementInput();
        HandleYawRotation();
        HandleGravity();
        HandleMovement();
        HandleAnimation();
    }

    // 🔒 INPUT: POLLING ONLY
    void ReadMovementInput()
    {
        moveInput = playerInput.CharacterControls.Move.ReadValue<Vector2>();
        // Quando rilasci WASD → moveInput == Vector2.zero
    }

    void HandleYawRotation()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        transform.Rotate(Vector3.up, mouseDelta.x * sensX);
    }

    void HandleMovement()
    {
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection =
            forward * moveInput.y +
            right * moveInput.x;

        if (moveDirection.sqrMagnitude > 1f)
            moveDirection.Normalize();

        float speed = walkSpeed;
        if (isRunPressed && moveDirection.sqrMagnitude > 0f)
            speed *= runMultiplier;

        Vector3 velocity = moveDirection * speed;
        velocity.y = verticalVelocity;

        characterController.Move(velocity * Time.deltaTime);
    }

    void HandleGravity()
    {
        if (characterController.isGrounded)
            verticalVelocity = -0.5f;
        else
            verticalVelocity += gravity * Time.deltaTime;
    }

    void HandleAnimation()
    {
        float horizontalSpeed = new Vector3(
            characterController.velocity.x,
            0f,
            characterController.velocity.z
        ).magnitude;

        animator.SetFloat("Speed", horizontalSpeed);
        animator.SetBool("IsRunning", isRunPressed && horizontalSpeed > 0.1f);
    }

    void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }

    // 🔒 BLOCCO ROOT MOTION A RUNTIME (ULTIMA LINEA DI DIFESA)
    void OnAnimatorMove() { }
}
