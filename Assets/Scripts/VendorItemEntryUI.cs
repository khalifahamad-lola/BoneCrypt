using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VendorItemEntryUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI stockText;
    [SerializeField] private Button buyButton;

    private int itemIndex;
    private VendorUI vendorUI;

    public void Setup(VendorEntry entry, int index, VendorUI ui)
    {
        itemIndex = index;
        vendorUI = ui;

        if (entry.item != null)
        {
            if (itemIcon != null)
            {
                itemIcon.sprite = entry.item.itemIcon;
                itemIcon.enabled = entry.item.itemIcon != null;
            }

            if (itemNameText != null)
                itemNameText.text = entry.item.itemName;

            int finalPrice = entry.priceOverride > 0 ? entry.priceOverride : entry.item.buyPrice;

            if (priceText != null)
                priceText.text = "Price: " + finalPrice;

            if (stockText != null)
                stockText.text = "Stock: " + entry.stockQuantity;
        }

        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnBuyClicked);
        }
    }

    private void OnBuyClicked()
    {
        if (vendorUI != null)
            vendorUI.TryBuy(itemIndex);
    }
}