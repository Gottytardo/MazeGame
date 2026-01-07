using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementInput : MonoBehaviour
{
    [Header("Movement")]
    public float Velocity = 4f;
    public float gravity = -9.81f;

    [Header("Input")]
    public float InputX;
    public float InputZ;

    [Header("References")]
    public Animator anim;
    public Camera cam;
    public CharacterController controller;

    [Header("Animation")]
    [Range(0, 1f)] public float StartAnimTime = 0.3f;
    [Range(0, 1f)] public float StopAnimTime = 0.15f;

    float verticalVel;
    Vector3 moveDirection;

    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    void Update()
    {
        ReadInput();
        ApplyGravity();
        Move();
        UpdateAnimations();
    }

    void ReadInput()
    {
        InputX = Input.GetAxis("Horizontal"); // A / D
        InputZ = Input.GetAxis("Vertical");   // W / S
    }

    void Move()
    {
        // Direzioni relative alla camera
        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        moveDirection = (forward * InputZ + right * InputX) * Velocity;

        // Applica movimento + gravità
        Vector3 finalMove = moveDirection;
        finalMove.y = verticalVel;

        controller.Move(finalMove * Time.deltaTime);
    }

    void ApplyGravity()
    {
        if (controller.isGrounded)
        {
            if (verticalVel < 0)
                verticalVel = -2f;
        }
        else
        {
            verticalVel += gravity * Time.deltaTime;
        }
    }

    void UpdateAnimations()
    {
        float speed = new Vector2(InputX, InputZ).sqrMagnitude;
        anim.SetFloat("Blend", speed, speed > 0 ? StartAnimTime : StopAnimTime, Time.deltaTime);
    }
}
