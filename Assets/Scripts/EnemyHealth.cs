using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour, IDamagable
{


    [SerializeField] private float maxHealth = 50;
    [SerializeField] private float hitStunTime = 0.15f;
    [SerializeField] private int soulsAmount = 100;

    [Header("Health Bar")]
    [SerializeField] private EnemyHealthBarUI healthBarPrefab;
    [SerializeField] private Transform headBarPoint;


    //Sounds
    [SerializeField] private AudioSource deathSound;


    private float currentHealth;
    private Animator animator;
    private NavMeshAgent agent;
    private SoulsWallet wallet;
    private Sekelto enemyAI;
    private float stunUntil;
    private float nextHitReactTime;
    private bool isDead;

    private EnemyHealthBarUI spawnedHealthBar;

    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        enemyAI = GetComponent<Sekelto>();
    }

    private void Start()
    {


        wallet = FindFirstObjectByType<SoulsWallet>();
        if (wallet == null)
        {
            Debug.Log("SoulsWallet not found");
        }

        if (healthBarPrefab != null)
        {
            spawnedHealthBar = Instantiate(healthBarPrefab);

            Transform barTarget = headBarPoint != null ? headBarPoint : transform;

            spawnedHealthBar.SetTarget(barTarget);
            spawnedHealthBar.SetMaxHealth(maxHealth);
            spawnedHealthBar.SetHealth(currentHealth);
        }
    }

    private void Update()
    {
        if (agent == null) return;

        agent.isStopped = Time.time < stunUntil;
    }

    public void TakeDamage(float dmg, Vector3 hitDir)
    {
        if (isDead) return;

        currentHealth -= dmg;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (spawnedHealthBar != null)
        {
            spawnedHealthBar.SetHealth(currentHealth);
        }

        if (enemyAI != null)
        {
            enemyAI.AlertToPlayer();
        }

        if (Time.time < nextHitReactTime)
        {
            if (currentHealth <= 0f) Die();
            return;
        }

        nextHitReactTime = Time.time + 0.12f;

        if (animator != null)
            animator.SetTrigger("Hit");

        stunUntil = Time.time + hitStunTime;

        if (agent != null)
        {
            Vector3 push = hitDir.sqrMagnitude > 0.001f ? hitDir.normalized : -transform.forward;
            agent.Move(push * 0.12f);
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        //Sounds
        if (deathSound != null)
        {
            deathSound.Play();
        }

        if (wallet != null)
        {
            wallet.AddSouls(soulsAmount);
        }

        if (spawnedHealthBar != null)
        {
            Destroy(spawnedHealthBar.gameObject);
        }

        Destroy(gameObject, 0.5f);
    }
}