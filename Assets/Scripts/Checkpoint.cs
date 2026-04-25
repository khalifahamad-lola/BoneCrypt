using UnityEngine;

public class Checkpoint : MonoBehaviour, IInteractable
{
    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;

    [Header("Effects")]
    [SerializeField] private GameObject fireEffect;

    [Header("Manager")]
    [SerializeField] private CheckpointManager checkpointManager;

    [Header("Feedback")]
    [SerializeField] private CheckpointFeedbackUI checkpointFeedbackUI;
    [SerializeField] private string activationMessage = "Checkpoint Activated";

    [Header("Options")]
    [SerializeField] private bool healPlayerOnActivate = true;

    private bool isActivated = false;

    private void Start()
    {
        if (checkpointManager == null)
            checkpointManager = FindFirstObjectByType<CheckpointManager>();

        if (checkpointFeedbackUI == null)
            checkpointFeedbackUI = FindFirstObjectByType<CheckpointFeedbackUI>();

        UpdateVisualState();
    }

    public void Interact(GameObject player)
    {
        PlayerRespawn playerRespawn = player.GetComponent<PlayerRespawn>();
        PlayerStats playerStats = player.GetComponent<PlayerStats>();

        if (playerRespawn == null)
        {
            Debug.LogWarning("Checkpoint: PlayerRespawn not found on player.");
            return;
        }

        if (respawnPoint == null)
        {
            Debug.LogWarning("Checkpoint: No respawn point assigned.");
            return;
        }

        playerRespawn.SetRespawnPoint(respawnPoint);

        if (checkpointManager != null)
        {
            checkpointManager.ActivateCheckpoint(this);
        }
        else
        {
            SetActiveState(true);
        }

        if (healPlayerOnActivate && playerStats != null)
            playerStats.RestoreFullState();

        if (checkpointFeedbackUI != null)
        {
            checkpointFeedbackUI.ShowMessage(activationMessage);
        }

        Debug.Log("Checkpoint activated: " + gameObject.name);
    }

    public void SetActiveState(bool active)
    {
        isActivated = active;
        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        if (fireEffect != null)
            fireEffect.SetActive(isActivated);
    }
}