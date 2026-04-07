using UnityEngine;

public class QuickSlotsUIController : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private QuickSlotUI[] slotUIs;

    private void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (inventory == null || slotUIs == null) return;

        for (int i = 0; i < slotUIs.Length; i++)
        {
            if (slotUIs[i] == null) continue;

            ItemData item = inventory.GetQuickSlotItem(i);
            int quantity = inventory.GetItemQuantity(item);

            slotUIs[i].SetSlot(item, quantity);
        }
    }
}