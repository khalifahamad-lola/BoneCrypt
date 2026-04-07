using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI quantityText;

    private ItemData currentItem;
    private InventoryUI inventoryUI;

    public void Setup(ItemData item, int quantity, InventoryUI ui)
    {
        currentItem = item;
        inventoryUI = ui;

        if (item != null)
        {
            itemIcon.sprite = item.itemIcon;
            itemIcon.enabled = item.itemIcon != null;
        }
        else
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }

        if (quantity > 1)
            quantityText.text = quantity.ToString();
        else
            quantityText.text = "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            Debug.Log("Clicked item: " + currentItem.itemName);
        }

        if (inventoryUI != null && currentItem != null)
        {
            inventoryUI.SelectItem(currentItem);
        }
    }
}