using UnityEngine;
using UnityEngine.InputSystem;

public class HeadBob : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public PlayerInput playerInput;

    [Header("Walk Settings")]
    public float walkBobSpeed = 10f;
    public float walkBobAmount = 0.03f;

    [Header("Run Settings")]
    public float runBobSpeed = 14f;
    public float runBobAmount = 0.06f;

    public float returnSpeed = 8f;

    private InputAction moveAction;
    private InputAction sprintAction;

    private float bobTimer;
    private Vector3 startLocalPos;

    private void Awake()
    {
        moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];
    }

    private void Start()
    {
        if (cameraTransform == null)
        {
            Debug.LogError("HeadBob: cameraTransform is not assigned!");
            enabled = false;
            return;
        }

        startLocalPos = cameraTransform.localPosition;
    }

    private void Update()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        bool isMoving = moveInput.magnitude > 0.1f;

        if (!isMoving)
        {
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                startLocalPos,
                Time.deltaTime * returnSpeed
            );

            bobTimer = 0f;
            return;
        }

        bool isRunning = sprintAction.IsPressed();

        float bobSpeed = isRunning ? runBobSpeed : walkBobSpeed;
        float bobAmount = isRunning ? runBobAmount : walkBobAmount;

        bobTimer += Time.deltaTime * bobSpeed;

        float offsetY = Mathf.Sin(bobTimer) * bobAmount;

        cameraTransform.localPosition = startLocalPos + new Vector3(0f, offsetY, 0f);
    }
}