using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private InteractPromptUI interactPromptUI;
    [SerializeField] private VendorUI vendorUI;
    [SerializeField] private InventoryUI inventoryUI;

    private void Update()
    {
        CheckForInteractable();
    }

    private void OnDisable()
    {
        if (interactPromptUI != null)
            interactPromptUI.HidePrompt();
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if ((vendorUI != null && vendorUI.IsOpen) ||
            (inventoryUI != null && inventoryUI.IsOpen))
        {
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                interactable.Interact(gameObject);
            }
        }
    }

    private void CheckForInteractable()
    {
        if (interactPromptUI == null) return;

        if ((vendorUI != null && vendorUI.IsOpen) ||
            (inventoryUI != null && inventoryUI.IsOpen))
        {
            interactPromptUI.HidePrompt();
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                interactPromptUI.ShowPrompt("Press E");
                return;
            }
        }

        interactPromptUI.HidePrompt();
    }
}