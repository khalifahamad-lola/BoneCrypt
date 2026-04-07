using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private PlayerStats playerStats;

    [Header("Visuals")]
    [SerializeField] private GameObject weaponPlaceholder;
    [SerializeField] private GameObject shieldPlaceholder;

    public WeaponItemData equippedWeapon;
    public ShieldItemData equippedShield;

    private float baseDamage;

    private void Awake()
    {
        if (inventory == null)
            inventory = GetComponent<PlayerInventory>();

        if (playerStats == null)
            playerStats = GetComponent<PlayerStats>();

        if (playerStats != null)
            baseDamage = playerStats.damage;

        UpdateVisuals();
        RecalculateStats();
    }

    public void EquipWeapon(WeaponItemData newWeapon)
    {
        if (newWeapon == null || inventory == null || playerStats == null) return;

        if (equippedWeapon == newWeapon)
        {
            Debug.Log("Weapon already equipped: " + newWeapon.itemName);
            return;
        }

        WeaponItemData oldWeapon = equippedWeapon;

        inventory.RemoveOne(newWeapon);
        equippedWeapon = newWeapon;

        if (oldWeapon != null)
        {
            inventory.Add(oldWeapon);
        }

        RecalculateStats();
        UpdateVisuals();

        Debug.Log("Equipped weapon: " + newWeapon.itemName);
    }

    public void EquipShield(ShieldItemData newShield)
    {
        if (newShield == null || inventory == null) return;

        if (equippedShield == newShield)
        {
            Debug.Log("Shield already equipped: " + newShield.itemName);
            return;
        }

        ShieldItemData oldShield = equippedShield;

        inventory.RemoveOne(newShield);
        equippedShield = newShield;

        if (oldShield != null)
        {
            inventory.Add(oldShield);
        }

        RecalculateStats();
        UpdateVisuals();

        Debug.Log("Equipped shield: " + newShield.itemName);
    }

    public void UnequipWeapon()
    {
        if (equippedWeapon == null || inventory == null) return;

        inventory.Add(equippedWeapon);
        Debug.Log("Unequipped weapon: " + equippedWeapon.itemName);

        equippedWeapon = null;

        RecalculateStats();
        UpdateVisuals();
    }

    public void UnequipShield()
    {
        if (equippedShield == null || inventory == null) return;

        inventory.Add(equippedShield);
        Debug.Log("Unequipped shield: " + equippedShield.itemName);

        equippedShield = null;

        RecalculateStats();
        UpdateVisuals();
    }

    public void RecalculateStats()
    {
        if (playerStats == null) return;

        playerStats.damage = baseDamage;

        if (equippedWeapon != null)
            playerStats.damage += equippedWeapon.damage;
    }

    public bool HasWeaponEquipped()
    {
        return equippedWeapon != null;
    }

    public bool HasShieldEquipped()
    {
        return equippedShield != null;
    }

    public float GetShieldBlockAmount()
    {
        if (equippedShield == null) return 0f;
        return equippedShield.blockAmount;
    }

    public void UpdateVisuals()
    {
        if (weaponPlaceholder != null)
            weaponPlaceholder.SetActive(equippedWeapon != null);

        if (shieldPlaceholder != null)
            shieldPlaceholder.SetActive(equippedShield != null);
    }
}