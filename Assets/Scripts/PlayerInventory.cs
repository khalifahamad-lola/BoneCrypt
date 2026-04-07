using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public List<InventorySlot> slots = new List<InventorySlot>();
    public ItemData[] quickSlots = new ItemData[4];

    [SerializeField] private int maxSlots = 20;
    [SerializeField] private QuickSlotsUIController quickSlotsUI;

    private PlayerStats player;

    void Start()
    {
        player = GetComponent<PlayerStats>();

        if (player == null)
        {
            Debug.LogError("PlayerInventory: No PlayerStats found on player.");
        }
    }

    public void Add(ItemData item)
    {
        if (item == null)
        {
            Debug.LogWarning("Tried to add null item.");
            return;
        }

        // Potions: fill an existing non-full stack first
        if (item is PotionItemData potion)
        {
            foreach (InventorySlot s in slots)
            {
                if (s.item == item && s.quantity < potion.maxStack)
                {
                    s.quantity++;
                    Debug.Log($"Stacked {item.itemName}. Quantity: {s.quantity}");
                    RefreshQuickSlotsUI();
                    return;
                }
            }

            // No non-full stack found, create a new stack
            if (slots.Count >= maxSlots)
            {
                Debug.Log("Inventory Full!");
                return;
            }

            slots.Add(new InventorySlot(item, 1));
            Debug.Log($"Inventory: Added new potion stack for {item.itemName}");
            RefreshQuickSlotsUI();
            return;
        }

        // Non-potion items: stack identical assets
        foreach (InventorySlot s in slots)
        {
            if (s.item == item)
            {
                s.quantity++;
                Debug.Log($"Stacked {item.itemName}. Quantity: {s.quantity}");
                RefreshQuickSlotsUI();
                return;
            }
        }

        if (slots.Count >= maxSlots)
        {
            Debug.Log("Inventory Full!");
            return;
        }

        slots.Add(new InventorySlot(item, 1));
        Debug.Log($"Inventory: Added {item.itemName}");
        RefreshQuickSlotsUI();
    }

    public bool TryUseKey(string keyId)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            KeyItemData keyItem = slots[i].item as KeyItemData;

            if (keyItem != null && keyItem.keyId == keyId)
            {
                slots[i].quantity--;

                Debug.Log("Used key: " + keyItem.itemName);

                if (slots[i].quantity <= 0)
                {
                    slots.RemoveAt(i);
                }

                RefreshQuickSlotsUI();
                return true;
            }
        }

        return false;
    }

    public void UseItem(ItemData item)
    {
        PotionItemData potion = item as PotionItemData;

        if (potion == null)
        {
            Debug.Log($"{item.itemName} is not usable right now.");
            return;
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item == item)
            {
                player.Heal(potion.healAmount);
                slots[i].quantity--;

                Debug.Log($"Used {item.itemName}. Remaining: {slots[i].quantity}");

                if (slots[i].quantity <= 0)
                {
                    slots.RemoveAt(i);
                    RemoveFromQuickSlot(item);
                }

                RefreshQuickSlotsUI();
                return;
            }
        }
    }

    public void RemoveOne(ItemData item)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item == item)
            {
                slots[i].quantity--;

                Debug.Log($"Removed 1 {item.itemName}. Remaining: {slots[i].quantity}");

                if (slots[i].quantity <= 0)
                {
                    slots.RemoveAt(i);
                }

                bool stillExists = false;

                foreach (InventorySlot s in slots)
                {
                    if (s.item == item)
                    {
                        stillExists = true;
                        break;
                    }
                }

                if (!stillExists)
                {
                    RemoveFromQuickSlot(item);
                }

                RefreshQuickSlotsUI();
                return;
            }
        }
    }

    public int GetQuickSlotIndex(ItemData item)
    {
        for (int i = 0; i < quickSlots.Length; i++)
        {
            if (quickSlots[i] == item)
                return i;
        }

        return -1;
    }

    public bool IsInQuickSlot(ItemData item)
    {
        return GetQuickSlotIndex(item) != -1;
    }

    public bool AssignToFirstEmptyQuickSlot(ItemData item)
    {
        if (item is not PotionItemData)
        {
            Debug.Log($"{item.itemName} cannot be assigned to a quick slot.");
            return false;
        }

        if (IsInQuickSlot(item))
        {
            Debug.Log($"{item.itemName} is already assigned to a quick slot.");
            return false;
        }

        for (int i = 0; i < quickSlots.Length; i++)
        {
            if (quickSlots[i] == null)
            {
                quickSlots[i] = item;
                Debug.Log($"{item.itemName} assigned to quick slot {i + 1}");
                RefreshQuickSlotsUI();
                return true;
            }
        }

        Debug.Log("No empty quick slot available.");
        return false;
    }

    public void RemoveFromQuickSlot(ItemData item)
    {
        for (int i = 0; i < quickSlots.Length; i++)
        {
            if (quickSlots[i] == item)
            {
                quickSlots[i] = null;
                Debug.Log($"{item.itemName} removed from quick slot {i + 1}");
                RefreshQuickSlotsUI();
                return;
            }
        }
    }

    public void UseQuickSlot(int index)
    {
        if (index < 0 || index >= quickSlots.Length) return;
        if (quickSlots[index] == null) return;

        UseItem(quickSlots[index]);
        RefreshQuickSlotsUI();
    }

    public ItemData GetQuickSlotItem(int index)
    {
        if (index < 0 || index >= quickSlots.Length)
            return null;

        return quickSlots[index];
    }

    public int GetItemQuantity(ItemData item)
    {
        if (item == null) return 0;

        foreach (InventorySlot slot in slots)
        {
            if (slot.item == item)
                return slot.quantity;
        }

        return 0;
    }

    private void RefreshQuickSlotsUI()
    {
        if (quickSlotsUI != null)
            quickSlotsUI.RefreshUI();
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            UseQuickSlot(0);

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            UseQuickSlot(1);

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            UseQuickSlot(2);

        if (Keyboard.current.digit4Key.wasPressedThisFrame)
            UseQuickSlot(3);
    }
}