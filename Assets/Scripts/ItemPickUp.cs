using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData item;
    [SerializeField] private int amount = 1;
    [SerializeField] private bool destroyOnPickup = true;

    private bool pickedUp = false;

    public void Interact(GameObject player)
    {
        if (pickedUp)
            return;

        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        if (inventory == null)
        {
            Debug.LogWarning("ItemPickup: No PlayerInventory found on player.");
            return;
        }

        if (item == null)
        {
            Debug.LogWarning("ItemPickup: No item assigned.");
            return;
        }

        pickedUp = true;

        Debug.Log("Picked up item asset: " + item.name + " | itemName: " + item.itemName + " | amount: " + amount);

        for (int i = 0; i < amount; i++)
        {
            inventory.Add(item);
        }

        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}