using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] public float currentHealth = 100f;

    [Header("Combat")]
    [SerializeField] private int playerLevel = 1;
    public float damage = 10f;

    [Header("XP")]
    [SerializeField] private float xpToLevelUp = 50f;
    [SerializeField] private float currentXp = 0f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina = 100f;
    public float staminaRegenRate = 20f;

    private PlayerBlock playerBlock;

    void Awake()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        playerBlock = GetComponent<PlayerBlock>();
    }

    void Update()
    {
        if (playerBlock == null || !playerBlock.IsBlocking)
        {
            RegenerateStamina();
        }
    }

    public void TakeDamage(float incomingDamage)
    {
        float finalDamage = incomingDamage;

        if (playerBlock != null && playerBlock.IsBlocking)
        {
            float blockedAmount = playerBlock.GetBlockAmount();
            finalDamage = Mathf.Max(1f, incomingDamage - blockedAmount);
            Debug.Log("Blocked damage. Reduced from " + incomingDamage + " to " + finalDamage);
        }

        currentHealth -= finalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log("Player Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player Died");
        Destroy(gameObject);
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina <= 0f)
            return false;

        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        return currentStamina > 0f;
    }

    public void RegenerateStamina()
    {
        currentStamina += staminaRegenRate * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    public void GainXp(float amount)
    {
        currentXp += amount;

        if (currentXp >= xpToLevelUp)
        {
            Debug.Log("Leveled UP!");
            playerLevel++;
            xpToLevelUp += 10f;
            currentXp = 0f;
            maxHealth += 10f;
            currentHealth = maxHealth;
            maxStamina += 5f;
            currentStamina = maxStamina;
            damage += 5f;
        }
    }
}