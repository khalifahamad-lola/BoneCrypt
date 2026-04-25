using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] public Transform playerBody;
    [SerializeField] public float sensitivity = 200f;

    private float xRotation;
    private Vector2 lookInput;
    private float ignoreLookUntilTime;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Time.time < ignoreLookUntilTime)
            return;

        float mouseX = lookInput.x * sensitivity * Time.deltaTime;
        float mouseY = lookInput.y * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (playerBody != null)
            playerBody.Rotate(Vector3.up * mouseX);
    }

    public void Look(InputAction.CallbackContext context)
    {
        if (Time.time < ignoreLookUntilTime)
        {
            lookInput = Vector2.zero;
            return;
        }

        lookInput = context.ReadValue<Vector2>();
    }

    public void ResetLookInput()
    {
        lookInput = Vector2.zero;
    }

    public void SuppressLookInputTemporarily(float duration)
    {
        lookInput = Vector2.zero;
        ignoreLookUntilTime = Time.time + duration;
    }

    private void OnEnable()
    {
        lookInput = Vector2.zero;
    }

    private void OnDisable()
    {
        lookInput = Vector2.zero;
    }
}