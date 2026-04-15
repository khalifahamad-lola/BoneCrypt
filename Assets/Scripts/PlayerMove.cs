using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    //Sounds
    [SerializeField] private AudioSource walkSound;


    [Header("Movement")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 6.5f;


    [Header("Jump & Gravity")]
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    CharacterController controller;
    Vector3 velocity;
    bool isGrounded;

    Vector2 moveInput;
    bool sprintHeld;
    bool jumpQueued;

    void Awake() => controller = GetComponent<CharacterController>();

    void Update()
    {



        // 1. Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0f) velocity.y = -2f;

        // 2. Body Rotation (Sync with CM3 Camera)
        // We only take the Y rotation (horizontal) so the body doesn't tilt up/down
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0; // Flatten to ground plane
        if (camForward != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(camForward);
        }

        // 3. Movement Logic (Now 'transform.forward' is correctly aligned)
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        float speed = sprintHeld ? sprintSpeed : walkSpeed;
        controller.Move(move * speed * Time.deltaTime);

        //Sounds
        if (move.magnitude > 0.1f && isGrounded)
        {
            if (!walkSound.isPlaying)
                walkSound.Play();
        }
        else
        {
            if (walkSound.isPlaying)
                walkSound.Stop();
        }

        // 4. Jump & Gravity
        if (jumpQueued && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpQueued = false;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Called by PlayerInput (Send Messages)

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        sprintHeld = context.ReadValueAsButton();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
            jumpQueued = true;
    }
}