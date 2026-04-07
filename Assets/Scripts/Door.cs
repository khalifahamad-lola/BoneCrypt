using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Door Movement")]
    [SerializeField] private float openHeight = 3f;
    [SerializeField] private float speed = 3f;

    [Header("Lock Settings")]
    [SerializeField] private bool isLocked = false;
    [SerializeField] private string requiredKeyId;

    private bool isOpen = false;
    private Vector3 closedPosition;
    private Vector3 openPosition;

    private void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + new Vector3(0, openHeight, 0);
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
        if (isOpen)
        {
            transform.position = Vector3.Lerp(transform.position, openPosition, Time.deltaTime * speed);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, closedPosition, Time.deltaTime * speed);
        }
    }
}