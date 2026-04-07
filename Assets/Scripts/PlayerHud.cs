using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider staminaSlider;

    private void Start()
    {
        RefreshBars();
    }

    private void Update()
    {
        RefreshBars();
    }

    private void RefreshBars()
    {
        if (playerStats == null) return;

        if (healthSlider != null)
        {
            healthSlider.maxValue = playerStats.maxHealth;
            healthSlider.value = playerStats.currentHealth;
        }

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = playerStats.maxStamina;
            staminaSlider.value = playerStats.currentStamina;
        }
    }
}