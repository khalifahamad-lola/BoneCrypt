using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBlock : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerEquipment playerEquipment;
    [SerializeField] private Animator shieldAnimator;

    [Header("Blocking")]
    [SerializeField] private float staminaDrainPerSecond = 25f;

    public bool IsBlocking { get; private set; }

    private void Awake()
    {
        if (playerStats == null)
            playerStats = GetComponent<PlayerStats>();

        if (playerEquipment == null)
            playerEquipment = GetComponent<PlayerEquipment>();
    }

    private void Update()
    {
        HandleBlocking();
        UpdateShieldAnimation();
    }

    private void HandleBlocking()
    {
        bool blockHeld = Mouse.current != null && Mouse.current.rightButton.isPressed;

        if (playerStats == null || playerEquipment == null)
        {
            IsBlocking = false;
            return;
        }

        if (!playerEquipment.HasShieldEquipped())
        {
            IsBlocking = false;
            return;
        }

        if (!blockHeld)
        {
            IsBlocking = false;
            return;
        }

        if (playerStats.currentStamina <= 0f)
        {
            IsBlocking = false;
            return;
        }

        playerStats.UseStamina(staminaDrainPerSecond * Time.deltaTime);
        IsBlocking = true;
    }

    private void UpdateShieldAnimation()
    {
        if (shieldAnimator != null)
            shieldAnimator.SetBool("isBlocking", playerEquipment != null && playerEquipment.HasShieldEquipped() && IsBlocking);
    }

    public float GetBlockAmount()
    {
        if (playerEquipment == null) return 0f;
        return playerEquipment.GetShieldBlockAmount();
    }
}