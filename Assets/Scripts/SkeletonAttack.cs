using UnityEngine;

public class SkeletonAttack : MonoBehaviour

{
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackRange = 2f;

    private float nextAttackTime;
    private PlayerStats currentTarget;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        
    }

    public void TryAttack(PlayerStats target)
    {
        if (Time.time < nextAttackTime) { return; }
        
            nextAttackTime = Time.time + cooldown;
            currentTarget = target;
            animator.SetTrigger("Attack");       
    }

    public void DealDamage()
    {
        if (currentTarget == null) return;


        float dist = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (dist > attackRange)
        {
            return;
        }

        currentTarget.TakeDamage(damage);
    }
}
