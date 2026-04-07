using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private Transform chestTop;

    [Header("Loot Pools")]
    [SerializeField] private ItemData[] commonLoot;
    [SerializeField] private ItemData[] rareLoot;
    [SerializeField] private ItemData[] legendaryLoot;

    [Header("Rarity Chances")]
    [SerializeField][Range(0, 100)] private int commonChance = 70;
    [SerializeField][Range(0, 100)] private int rareChance = 25;
    [SerializeField][Range(0, 100)] private int legendaryChance = 5;

    [Header("Open Settings")]
    [SerializeField] private Vector3 openRotation = new Vector3(-110f, 0f, 0f);
    [SerializeField] private float openSpeed = 5f;
    [SerializeField] private bool openOnceOnly = true;

    private bool isOpen = false;
    private bool rewardGiven = false;

    private Quaternion closedRot;
    private Quaternion targetRot;

    private LootPopupUI lootPopupUI;

    private void Start()
    {
        if (chestTop == null)
        {
            Debug.LogError("Chest: chestTop is not assigned.");
            enabled = false;
            return;
        }

        closedRot = chestTop.localRotation;
        targetRot = closedRot;

        lootPopupUI = FindFirstObjectByType<LootPopupUI>();
    }

    public void Interact(GameObject player)
    {
        if (openOnceOnly && isOpen)
            return;

        isOpen = true;
        targetRot = closedRot * Quaternion.Euler(openRotation);

        if (!rewardGiven)
        {
            GiveRandomReward(player);
            rewardGiven = true;
        }
    }

    private void GiveRandomReward(GameObject player)
    {
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory == null)
        {
            Debug.LogWarning("Chest: No PlayerInventory found on player.");
            return;
        }

        ItemData reward = RollRandomLoot();

        if (reward == null)
        {
            Debug.Log("Chest: No valid reward found.");
            return;
        }

        inventory.Add(reward);
        Debug.Log("Chest gave: " + reward.itemName + " [" + reward.itemRarity + "]");

        if (lootPopupUI != null)
        {
            lootPopupUI.ShowLoot(reward.itemName, reward.itemRarity);
        }
    }

    private ItemData RollRandomLoot()
    {
        int totalChance = commonChance + rareChance + legendaryChance;

        if (totalChance <= 0)
        {
            Debug.LogWarning("Chest: Total rarity chance is 0.");
            return null;
        }

        int roll = Random.Range(1, totalChance + 1);

        if (roll <= commonChance)
            return GetRandomItemFromPool(commonLoot);

        if (roll <= commonChance + rareChance)
            return GetRandomItemFromPool(rareLoot);

        return GetRandomItemFromPool(legendaryLoot);
    }

    private ItemData GetRandomItemFromPool(ItemData[] pool)
    {
        if (pool == null || pool.Length == 0)
            return null;

        int index = Random.Range(0, pool.Length);
        return pool[index];
    }

    private void Update()
    {
        if (chestTop == null) return;

        chestTop.localRotation = Quaternion.Lerp(
            chestTop.localRotation,
            targetRot,
            Time.deltaTime * openSpeed
        );
    }
}