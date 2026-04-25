using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] patrolPoints;

    private GameObject currentEnemyInstance;

    private void Start()
    {
        SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("EnemySpawnPoint: No enemyPrefab assigned on " + gameObject.name);
            return;
        }

        if (currentEnemyInstance != null)
            return;

        currentEnemyInstance = Instantiate(enemyPrefab, transform.position, transform.rotation);

        EnemyHealth enemyHealth = currentEnemyInstance.GetComponentInChildren<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.SetSpawnPoint(this);
        }

        Sekelto enemyAI = currentEnemyInstance.GetComponentInChildren<Sekelto>();
        if (enemyAI != null && patrolPoints != null && patrolPoints.Length > 0)
        {
            enemyAI.SetPatrolPoints(patrolPoints);
        }
    }

    public void ClearCurrentEnemy()
    {
        currentEnemyInstance = null;
    }

    public void ResetEnemyFully()
    {
        if (currentEnemyInstance != null)
        {
            Destroy(currentEnemyInstance);
            currentEnemyInstance = null;
        }

        SpawnEnemy();
    }
}