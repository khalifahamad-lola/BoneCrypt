using System.Collections;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay = 1.2f;

    [Header("Disable On Death")]
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private PlayerBlock playerBlock;
    [SerializeField] private CharacterController characterController;

    [Header("Reset")]
    [SerializeField] private EnemyResetManager enemyResetManager;

    [Header("Death Screen")]
    [SerializeField] private DeathScreenUI deathScreenUI;
    [SerializeField] private float deathScreenHoldTime = 1.2f;

    private PlayerStats playerStats;
    private bool isDead = false;

    public bool IsDead => isDead;
    public Transform CurrentRespawnPoint => respawnPoint;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();

        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (playerLook == null)
            playerLook = GetComponentInChildren<PlayerLook>();

        if (playerMove == null)
            playerMove = GetComponent<PlayerMove>();

        if (playerAttack == null)
            playerAttack = GetComponent<PlayerAttack>();

        if (playerInteract == null)
            playerInteract = GetComponent<PlayerInteract>();

        if (playerBlock == null)
            playerBlock = GetComponent<PlayerBlock>();

        if (enemyResetManager == null)
            enemyResetManager = FindFirstObjectByType<EnemyResetManager>();

        if (deathScreenUI == null)
            deathScreenUI = FindFirstObjectByType<DeathScreenUI>();
    }

    public void HandleDeath()
    {
        if (isDead)
            return;

        isDead = true;
        SetPlayerEnabledState(false);
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        if (deathScreenUI != null)
        {
            yield return StartCoroutine(deathScreenUI.PlayDeathSequence(deathScreenHoldTime));
        }
        else
        {
            yield return new WaitForSecondsRealtime(respawnDelay);
        }

        RespawnNow();

        if (deathScreenUI != null)
        {
            yield return StartCoroutine(deathScreenUI.PlayRespawnFadeOut());
        }
    }

    public void RespawnNow()
    {
        if (playerStats == null)
            return;

        if (respawnPoint == null)
        {
            Debug.LogWarning("PlayerRespawn: No respawn point assigned.");
            isDead = false;
            SetPlayerEnabledState(true);
            return;
        }

        if (enemyResetManager != null)
        {
            enemyResetManager.ResetAllEnemies();
        }

        if (characterController != null)
            characterController.enabled = false;

        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;

        if (characterController != null)
            characterController.enabled = true;

        playerStats.RestoreFullState();

        isDead = false;
        SetPlayerEnabledState(true);

        Debug.Log("Player respawned.");
    }

    public void SetRespawnPoint(Transform newRespawnPoint)
    {
        if (newRespawnPoint == null)
            return;

        respawnPoint = newRespawnPoint;
        Debug.Log("Respawn point updated to: " + newRespawnPoint.name);
    }

    private void SetPlayerEnabledState(bool enabledState)
    {
        if (playerLook != null) playerLook.enabled = enabledState;
        if (playerMove != null) playerMove.enabled = enabledState;
        if (playerAttack != null) playerAttack.enabled = enabledState;
        if (playerInteract != null) playerInteract.enabled = enabledState;
        if (playerBlock != null) playerBlock.enabled = enabledState;
    }
}