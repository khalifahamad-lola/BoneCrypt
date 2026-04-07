using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Inventory Panel")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject itemEntryPrefab;

    [Header("References")]
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private SoulsWallet wallet;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private PlayerMove playerMovement;
    [SerializeField] private VendorUI vendorUI;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private PlayerEquipment equipment;
    [SerializeField] private PlayerBlock playerBlock;

    [Header("Stats UI")]
    [SerializeField] private TextMeshProUGUI statsText;

    [Header("Equipped UI")]
    [SerializeField] private TextMeshProUGUI equippedWeaponText;
    [SerializeField] private TextMeshProUGUI equippedShieldText;
    [SerializeField] private Button unequipWeaponButton;
    [SerializeField] private Button unequipShieldButton;
    [SerializeField] private EquippedItemUI equippedWeaponSlotUI;
    [SerializeField] private EquippedItemUI equippedShieldSlotUI;

    [Header("Item Action UI")]
    [SerializeField] private GameObject itemActionPanel;
    [SerializeField] private TextMeshProUGUI selectedItemNameText;
    [SerializeField] private Button useItemButton;
    [SerializeField] private TextMeshProUGUI useItemButtonText;
    [SerializeField] private GameObject addQuickSlotButton;
    [SerializeField] private GameObject removeItemButton;

    private bool isOpen = false;
    private ItemData selectedItem;

    public bool IsOpen => isOpen;

    void Update()
    {
        if (vendorUI != null && vendorUI.IsOpen)
            return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            RefreshUI();
            DeselectAllSelections();

            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (playerLook != null) playerLook.enabled = false;
            if (playerMovement != null) playerMovement.enabled = false;
            if (playerAttack != null) playerAttack.enabled = false;
            if (playerInteract != null) playerInteract.enabled = false;
            if (playerBlock != null) playerBlock.enabled = false;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (playerLook != null) playerLook.enabled = true;
            if (playerMovement != null) playerMovement.enabled = true;
            if (playerAttack != null) playerAttack.enabled = true;
            if (playerInteract != null) playerInteract.enabled = true;
            if (playerBlock != null) playerBlock.enabled = true;
        }
    }

    void RefreshUI()
    {
        RefreshItems();
        RefreshStats();
        RefreshEquippedUI();
    }

    void RefreshItems()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (InventorySlot s in inventory.slots)
        {
            GameObject entry = Instantiate(itemEntryPrefab, contentParent);

            InventorySlotUI slotUI = entry.GetComponent<InventorySlotUI>();

            if (slotUI != null)
            {
                slotUI.Setup(s.item, s.quantity, this);
            }
        }
    }

    void RefreshStats()
    {
        if (statsText == null || playerStats == null || wallet == null) return;

        statsText.text =
            "HP: " + playerStats.currentHealth + " / " + playerStats.maxHealth + "\n" +
            "Stamina: " + Mathf.RoundToInt(playerStats.currentStamina) + " / " + Mathf.RoundToInt(playerStats.maxStamina) + "\n" +
            "Damage: " + playerStats.damage + "\n" +
            "Souls: " + wallet.GetSouls();
    }

    void RefreshEquippedUI()
    {
        if (equipment == null) return;

        if (equippedWeaponText != null)
        {
            equippedWeaponText.text = equipment.equippedWeapon != null
                ? "Weapon: " + equipment.equippedWeapon.itemName
                : "Weapon: None";
        }

        if (equippedShieldText != null)
        {
            equippedShieldText.text = equipment.equippedShield != null
                ? "Shield: " + equipment.equippedShield.itemName
                : "Shield: None";
        }

        if (equippedWeaponSlotUI != null)
        {
            equippedWeaponSlotUI.Setup(equipment.equippedWeapon, this, true);
        }

        if (equippedShieldSlotUI != null)
        {
            equippedShieldSlotUI.Setup(equipment.equippedShield, this, false);
        }
    }

    public void SelectEquippedWeapon()
    {
        DeselectInventoryItemOnly();

        if (equipment == null || equipment.equippedWeapon == null)
            return;

        if (unequipWeaponButton != null)
            unequipWeaponButton.gameObject.SetActive(true);

        if (unequipShieldButton != null)
            unequipShieldButton.gameObject.SetActive(false);
    }

    public void SelectEquippedShield()
    {
        DeselectInventoryItemOnly();

        if (equipment == null || equipment.equippedShield == null)
            return;

        if (unequipShieldButton != null)
            unequipShieldButton.gameObject.SetActive(true);

        if (unequipWeaponButton != null)
            unequipWeaponButton.gameObject.SetActive(false);
    }

    void HideUnequipButtons()
    {
        if (unequipWeaponButton != null)
            unequipWeaponButton.gameObject.SetActive(false);

        if (unequipShieldButton != null)
            unequipShieldButton.gameObject.SetActive(false);
    }

    void DeselectInventoryItemOnly()
    {
        selectedItem = null;

        if (itemActionPanel != null)
            itemActionPanel.SetActive(false);
    }

    public void DeselectAllSelections()
    {
        DeselectInventoryItemOnly();
        HideUnequipButtons();
    }

    public void SelectItem(ItemData item)
    {
        HideUnequipButtons();

        selectedItem = item;

        if (itemActionPanel != null)
            itemActionPanel.SetActive(true);

        if (selectedItemNameText != null)
            selectedItemNameText.text = item.itemName;

        bool isPotion = item is PotionItemData;
        bool isWeapon = item is WeaponItemData;
        bool isShield = item is ShieldItemData;

        if (useItemButton != null)
        {
            if (isPotion)
            {
                useItemButton.gameObject.SetActive(true);

                if (useItemButtonText != null)
                    useItemButtonText.text = "Use Item";
            }
            else if (isWeapon || isShield)
            {
                useItemButton.gameObject.SetActive(true);

                if (useItemButtonText != null)
                    useItemButtonText.text = "Equip";
            }
            else
            {
                useItemButton.gameObject.SetActive(false);
            }
        }

        if (addQuickSlotButton != null)
        {
            bool canQuickSlot = isPotion && !inventory.IsInQuickSlot(item);
            addQuickSlotButton.SetActive(canQuickSlot);
        }

        if (removeItemButton != null)
        {
            removeItemButton.SetActive(true);
        }
    }

    public void UseSelectedItem()
    {
        if (selectedItem == null) return;

        if (selectedItem is PotionItemData)
        {
            ItemData usedItem = selectedItem;

            inventory.UseItem(usedItem);
            RefreshUI();

            bool stillExists = false;
            foreach (InventorySlot s in inventory.slots)
            {
                if (s.item == usedItem)
                {
                    stillExists = true;
                    break;
                }
            }

            if (stillExists)
                SelectItem(usedItem);
            else
                DeselectAllSelections();
        }
        else if (selectedItem is ShieldItemData shield)
        {
            if (equipment != null)
            {
                equipment.EquipShield(shield);
                RefreshUI();
                DeselectAllSelections();
            }
        }
        else if (selectedItem is WeaponItemData weapon)
        {
            if (equipment != null)
            {
                equipment.EquipWeapon(weapon);
                RefreshUI();
                DeselectAllSelections();
            }
        }
    }

    public void AddSelectedItemToQuickSlot()
    {
        if (selectedItem == null) return;

        if (inventory.AssignToFirstEmptyQuickSlot(selectedItem))
        {
            RefreshUI();
            SelectItem(selectedItem);
        }
    }

    public void RemoveSelectedItem()
    {
        if (selectedItem == null) return;

        inventory.RemoveOne(selectedItem);
        selectedItem = null;

        RefreshUI();
        DeselectAllSelections();
    }

    public void UnequipWeapon()
    {
        if (equipment == null) return;

        equipment.UnequipWeapon();
        RefreshUI();
        DeselectAllSelections();
    }

    public void UnequipShield()
    {
        if (equipment == null) return;

        equipment.UnequipShield();
        RefreshUI();
        DeselectAllSelections();
    }
}