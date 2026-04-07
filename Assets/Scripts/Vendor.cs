using UnityEngine;

public class Vendor : MonoBehaviour, IInteractable
{
    public VendorEntry[] stock;

    private PlayerInventory inventory;
    private SoulsWallet wallet;
    private VendorUI vendorUI;

    private void Start()
    {
        inventory = FindFirstObjectByType<PlayerInventory>();
        wallet = FindFirstObjectByType<SoulsWallet>();
        vendorUI = FindFirstObjectByType<VendorUI>();

        if (wallet == null) Debug.LogError("Vendor: No SoulsWallet found in scene.");
        if (inventory == null) Debug.LogError("Vendor: No PlayerInventory found in scene.");
        if (vendorUI == null) Debug.LogError("Vendor: No VendorUI found in scene.");
    }

    public void Interact(GameObject player)
    {
        if (vendorUI != null)
        {
            vendorUI.OpenVendor(this);
        }
    }

    public void Buy(int index)
    {
        if (index < 0 || index >= stock.Length)
        {
            Debug.LogWarning("Vendor: Invalid stock index");
            return;
        }

        VendorEntry entry = stock[index];
        int price = (entry.priceOverride > 0) ? entry.priceOverride : entry.item.buyPrice;

        if (entry.stockQuantity <= 0)
        {
            Debug.Log(entry.item.itemName + " is out of stock.");
            return;
        }

        if (wallet.SpendSouls(price))
        {
            inventory.Add(entry.item);
            entry.stockQuantity--;

            Debug.Log($"Vendor: Purchased {entry.item.itemName} for {price} souls. Remaining stock: {entry.stockQuantity}");
        }
        else
        {
            Debug.Log($"Vendor: Not enough souls for {entry.item.itemName} (cost {price})");
        }
    }
}