using UnityEngine;

public class SwordAnimeEvent : MonoBehaviour
{
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private TrailRenderer slashTrail;

    public void EnableSlashTrail()
    {
        if (slashTrail != null)
            slashTrail.emitting = true;
    }

    public void DisableSlashTrail()
    {
        if (slashTrail != null)
            slashTrail.emitting = false;
    }

    public void EnableDamageWindow()
    {
        if (playerAttack != null)
            playerAttack.EnableDamageWindow();
    }

    public void DisableDamageWindow()
    {
        if (playerAttack != null)
            playerAttack.DisableDamageWindow();
    }

    public void EnableComboWindow()
    {
        if (playerAttack != null)
            playerAttack.EnableComboWindow();
    }

    public void DisableComboWindow()
    {
        if (playerAttack != null)
            playerAttack.DisableComboWindow();
    }

    public void ContinueComboOrReset()
    {
        if (playerAttack != null)
            playerAttack.ContinueComboOrReset();
    }

    public void ResetAttack()
    {
        if (playerAttack != null)
            playerAttack.ResetAttack();
    }
}