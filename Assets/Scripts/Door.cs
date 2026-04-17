using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    [Header("Door Rotation")]
    [SerializeField] private RotationAxis rotationAxis = RotationAxis.Y;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float speed = 3f;

    [Header("Lock Settings")]
    [SerializeField] private bool isLocked = false;
    [SerializeField] private string requiredKeyId;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    private void Start()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(GetOpenEuler());
    }

    public void Interact(GameObject player)
    {
        if (isLocked)
        {
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();

            if (inventory == null)
            {
                Debug.LogWarning("Door: No PlayerInventory found on player.");
                return;
            }

            bool usedKey = inventory.TryUseKey(requiredKeyId);

            if (!usedKey)
            {
                Debug.Log("Door is locked. Missing key: " + requiredKeyId);
                return;
            }

            isLocked = false;
            Debug.Log("Door unlocked with key: " + requiredKeyId);
        }

        isOpen = !isOpen;
    }

    private void Update()
    {
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * speed
        );
    }

    private Vector3 GetOpenEuler()
    {
        switch (rotationAxis)
        {
            case RotationAxis.X:
                return new Vector3(openAngle, 0f, 0f);

            case RotationAxis.Y:
                return new Vector3(0f, openAngle, 0f);

            case RotationAxis.Z:
                return new Vector3(0f, 0f, openAngle);

            default:
                return Vector3.zero;
        }
    }
}