using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickSlotUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private GameObject emptyVisual;

    public void SetSlot(ItemData item, int quantity)
    {
        if (item != null)
        {
            if (itemIcon != null)
            {
                itemIcon.sprite = item.itemIcon;
                itemIcon.enabled = item.itemIcon != null;
            }

            if (quantityText != null)
            {
                quantityText.text = quantity > 1 ? quantity.ToString() : "";
            }

            if (emptyVisual != null)
            {
                emptyVisual.SetActive(false);
            }
        }
        else
        {
            if (itemIcon != null)
            {
                itemIcon.sprite = null;
                itemIcon.enabled = false;
            }

            if (quantityText != null)
            {
                quantityText.text = "";
            }

            if (emptyVisual != null)
            {
                emptyVisual.SetActive(true);
            }
        }
    }
}