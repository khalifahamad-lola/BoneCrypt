using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquippedItemUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image itemIcon;

    private ItemData currentItem;
    private InventoryUI inventoryUI;
    private bool isWeaponSlot;

    public void Setup(ItemData item, InventoryUI ui, bool weaponSlot)
    {
        currentItem = item;
        inventoryUI = ui;
        isWeaponSlot = weaponSlot;

        if (itemIcon != null)
        {
            if (item != null && item.itemIcon != null)
            {
                itemIcon.sprite = item.itemIcon;
                itemIcon.enabled = true;
            }
            else
            {
                itemIcon.sprite = null;
                itemIcon.enabled = false;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (inventoryUI == null || currentItem == null)
            return;

        if (isWeaponSlot)
            inventoryUI.SelectEquippedWeapon();
        else
            inventoryUI.SelectEquippedShield();
    }
}