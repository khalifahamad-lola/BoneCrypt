using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 6.5f;
    public bool canMove = true;

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

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        if (!canMove)
        {
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            return;
        }

        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0f;

        if (camForward != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(camForward);

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        float speed = sprintHeld ? sprintSpeed : walkSpeed;
        controller.Move(move * speed * Time.deltaTime);

        if (jumpQueued && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpQueued = false;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!canMove)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = context.ReadValue<Vector2>();
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (!canMove)
        {
            sprintHeld = false;
            return;
        }

        sprintHeld = context.ReadValueAsButton();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!canMove)
            return;

        if (context.performed)
            jumpQueued = true;
    }
}