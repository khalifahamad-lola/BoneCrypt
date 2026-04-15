using UnityEngine;
using UnityEngine.AI;

public class Sekelto : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolWaitTime = 1.5f;
    [SerializeField] private float patrolSpeed = 2.5f;

    [Header("Detection / Chase")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float giveUpRange = 15f;
    [SerializeField] private float loseSightTime = 3f;
    [SerializeField] private float chaseSpeed = 4.5f;
    [SerializeField] private float updateTargetRate = 0.1f;

    [Header("Line of Sight")]
    [SerializeField] private float eyeHeight = 1.6f;
    [SerializeField] private float playerViewHeight = 1.0f;
    [SerializeField] private LayerMask sightBlockMask = ~0;

    [Header("Attack")]
    [SerializeField] private float attackRange = 2f;

    //Sounds
    [SerializeField] private AudioSource enemySound;
    private bool hasPlayedAlertSound = false;


    private NavMeshAgent agent;
    private Transform player;
    private SkeletonAttack attack;
    private Animator animator;
    private PlayerStats playerStats;

    private int currentPatrolIndex;
    private float patrolWaitTimer;
    private float loseTimer;
    private float nextUpdateTime;

    private enum State
    {
        Patrol,
        Chase
    }

    private State currentState = State.Patrol;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        attack = GetComponent<SkeletonAttack>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {



        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerStats = playerObj.GetComponent<PlayerStats>();
        }

        if (agent != null)
        {
            agent.speed = patrolSpeed;
            agent.stoppingDistance = 0f;
            agent.autoBraking = true;
        }

        GoToNextPatrolPoint();
    }

    private void Update()
    {
        if (player == null || agent == null || animator == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        bool inAttack = animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
        bool isHit = animator.GetCurrentAnimatorStateInfo(0).IsName("HitReact");

        animator.SetBool("isStunned", isHit);

        if (isHit)
        {
            agent.isStopped = true;
            SetMovementAnim(false, false);
            return;
        }

        if (inAttack)
        {
            agent.isStopped = true;
            SetMovementAnim(false, false);
            return;
        }

        agent.isStopped = false;

        switch (currentState)
        {
            case State.Patrol:
                UpdatePatrol(distance);
                break;

            case State.Chase:
                UpdateChase(distance);
                break;
        }
    }

    private void UpdatePatrol(float distance)
    {
        agent.speed = patrolSpeed;

        if (CanSeePlayer(distance))
        {
            currentState = State.Chase;
            loseTimer = 0f;
            patrolWaitTimer = 0f;

            if (!hasPlayedAlertSound && enemySound != null)
            {
                enemySound.PlayOneShot(enemySound.clip, 1.5f); //Sounds

                hasPlayedAlertSound = true;
            }

            return;
        }

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            SetMovementAnim(false, false);
            return;
        }

        bool reachedPoint =
            !agent.pathPending &&
            agent.pathStatus == NavMeshPathStatus.PathComplete &&
            agent.remainingDistance <= agent.stoppingDistance + 2f;

        if (reachedPoint)
        {
            patrolWaitTimer += Time.deltaTime;
            SetMovementAnim(false, false);

            if (patrolWaitTimer >= patrolWaitTime)
            {
                GoToNextPatrolPoint();
                patrolWaitTimer = 0f;
            }
        }
        else
        {
            SetMovementAnim(true, false);
        }
    }

    private void UpdateChase(float distance)
    {
        agent.speed = chaseSpeed;

        if (distance <= attackRange)
        {
            agent.ResetPath();

            Vector3 lookPos = player.position;
            lookPos.y = transform.position.y;
            transform.LookAt(lookPos);

            SetMovementAnim(false, false);

            if (playerStats != null)
                attack.TryAttack(playerStats);

            return;
        }

        if (!CanSeePlayer(distance))
        {
            loseTimer += Time.deltaTime;

            if (loseTimer >= loseSightTime)
            {
                currentState = State.Patrol;
                loseTimer = 0f;
                patrolWaitTimer = 0f;
                agent.speed = patrolSpeed;

                hasPlayedAlertSound = false; //Sounds

                GoToNextPatrolPoint();
                return;
            }
        }
        else
        {
            loseTimer = 0f;
        }

        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateTargetRate;
            agent.SetDestination(player.position);
        }

        SetMovementAnim(false, true);
    }

    private bool CanSeePlayer(float distance)
    {
        if (player == null) return false;

        if (distance > detectionRange)
            return false;

        Vector3 enemyEyePos = transform.position + Vector3.up * eyeHeight;
        Vector3 playerTargetPos = player.position + Vector3.up * playerViewHeight;

        Vector3 dirToPlayer = (playerTargetPos - enemyEyePos).normalized;
        Vector3 flatDirToPlayer = dirToPlayer;
        flatDirToPlayer.y = 0f;

        Vector3 forward = transform.forward;
        forward.y = 0f;

        float angleToPlayer = Vector3.Angle(forward, flatDirToPlayer);

        if (angleToPlayer > viewAngle * 0.5f)
            return false;

        float rayDistance = Vector3.Distance(enemyEyePos, playerTargetPos);

        if (Physics.Raycast(enemyEyePos, dirToPlayer, out RaycastHit hit, rayDistance, sightBlockMask))
        {
            if (hit.collider.CompareTag("Player") || hit.collider.GetComponentInParent<PlayerStats>() != null)
            {
                return true;
            }

            return false;
        }

        return false;
    }

    public void AlertToPlayer()
    {
        if (player == null || agent == null)
            return;

        currentState = State.Chase;
        loseTimer = 0f;
        patrolWaitTimer = 0f;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        agent.ResetPath();
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    private void SetMovementAnim(bool walking, bool running)
    {
        animator.SetBool("isWalking", walking);
        animator.SetBool("isRunning", running);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, giveUpRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle * 0.5f, 0) * transform.forward;

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position + Vector3.up * eyeHeight, leftBoundary * detectionRange);
        Gizmos.DrawRay(transform.position + Vector3.up * eyeHeight, rightBoundary * detectionRange);

        if (player != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(
                transform.position + Vector3.up * eyeHeight,
                player.position + Vector3.up * playerViewHeight
            );
        }
    }
}