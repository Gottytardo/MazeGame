using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementController : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;

    [SerializeField] Transform cameraTransform;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;

    bool isMovementPressed;
    bool isRunPressed;

    float rotationFactorPerFrame = 10f;
    float runMultiplier = 3f;
    float gravity = -9.81f;
    float verticalVelocity;

    void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;

        playerInput.CharacterControls.Run.started += ctx => isRunPressed = true;
        playerInput.CharacterControls.Run.canceled += ctx => isRunPressed = false;
    }

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        isMovementPressed = currentMovementInput.sqrMagnitude > 0;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 direction = forward * currentMovementInput.y + right * currentMovementInput.x;

        currentMovement.x = direction.x;
        currentMovement.z = direction.z;

        currentRunMovement.x = direction.x * runMultiplier;
        currentRunMovement.z = direction.z * runMultiplier;
    }

    void handleRotation()
    {
        if (!isMovementPressed) return;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * currentMovementInput.y + right * currentMovementInput.x;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationFactorPerFrame * Time.deltaTime
            );
        }
    }

    void handleGravity()
    {
        if (characterController.isGrounded)
            verticalVelocity = -2f;
        else
            verticalVelocity += gravity * Time.deltaTime;

        currentMovement.y = verticalVelocity;
        currentRunMovement.y = verticalVelocity;
    }

    void handleAnimation()
    {
        animator.SetBool("isWalking", isMovementPressed);
        animator.SetBool("isRunning", isRunPressed && isMovementPressed);
    }

    void Update()
    {
        handleGravity();
        handleRotation();
        handleAnimation();

        if (isRunPressed)
            characterController.Move(currentRunMovement * Time.deltaTime);
        else
            characterController.Move(currentMovement * Time.deltaTime);
    }

    void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}
