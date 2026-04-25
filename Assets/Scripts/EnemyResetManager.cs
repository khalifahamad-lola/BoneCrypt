using UnityEngine;

public class EnemyResetManager : MonoBehaviour
{
    private EnemySpawnPoint[] spawnPoints;

    private void Awake()
    {
        spawnPoints = FindObjectsByType<EnemySpawnPoint>(FindObjectsSortMode.None);
    }

    public void ResetAllEnemies()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("EnemyResetManager: No enemy spawn points found.");
            return;
        }

        foreach (EnemySpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
            {
                spawnPoint.ResetEnemyFully();
            }
        }

        Debug.Log("EnemyResetManager: Fully reset all enemies.");
    }
}