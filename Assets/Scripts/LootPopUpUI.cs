using UnityEngine;
using TMPro;
using System.Collections;

public class LootPopupUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI lootText;
    [SerializeField] private float showTime = 2f;

    private Coroutine currentRoutine;

    private void Start()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void ShowLoot(string itemName, ItemRarity rarity)
    {
        if (lootText != null)
        {
            lootText.text = "Found: " + itemName;
            lootText.color = GetRarityColor(rarity);
        }

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        if (panel != null)
            panel.SetActive(true);

        yield return new WaitForSeconds(showTime);

        if (panel != null)
            panel.SetActive(false);

        currentRoutine = null;
    }

    private Color GetRarityColor(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Rare:
                return Color.cyan;

            case ItemRarity.Legendary:
                return new Color(1f, 0.84f, 0f);

            default:
                return Color.white;
        }
    }
}