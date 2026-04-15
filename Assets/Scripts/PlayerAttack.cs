using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float attackDistance = 5f;
    [SerializeField] private Animator swordAnimation;
    [SerializeField] private PlayerEquipment equipment;
    [SerializeField] private GameObject bloodBurstPrefab;

    //Sounds
    [SerializeField] private AudioSource hitSound;


    private PlayerStats playerStats;

    private bool canAttack = true;
    private bool canQueueNextAttack = false;
    private bool queuedNextAttack = false;
    private int comboStep = 0;

    private bool damageWindowOpen = false;
    private HashSet<IDamagable> hitTargetsThisSwing = new HashSet<IDamagable>();

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();

        if (equipment == null)
            equipment = GetComponent<PlayerEquipment>();
    }

    private void Update()
    {
        if (damageWindowOpen)
        {
            TryDamageDuringSwing();
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!enabled) return;
        if (!context.performed) return;

        if (equipment == null || !equipment.HasWeaponEquipped())
        {
            Debug.Log("No weapon equipped.");
            return;
        }

        if (canAttack)
        {
            canAttack = false;  


            comboStep = 1;
            queuedNextAttack = false;
            canQueueNextAttack = false;

            swordAnimation.ResetTrigger("Attack2");
            swordAnimation.SetTrigger("Attack1");
            return;
        }

        if (comboStep == 1 && canQueueNextAttack)
        {
            queuedNextAttack = true;
        }
    }

    private void TryDamageDuringSwing()
    {
        float damage = playerStats.damage;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, attackDistance))
        {
            var target = hit.collider.GetComponentInParent<IDamagable>();
            if (target != null && !hitTargetsThisSwing.Contains(target))
            {
                Vector3 hitDir = -playerCamera.transform.forward;
                target.TakeDamage(damage, hitDir);

                if (hitSound != null)//Sounds
                {
                    hitSound.PlayOneShot(hitSound.clip);
                }

                hitTargetsThisSwing.Add(target);
                SpawnBloodEffect(hit);
            }
        }
    }

    public void EnableDamageWindow()
    {
        damageWindowOpen = true;
        hitTargetsThisSwing.Clear();
    }

    public void DisableDamageWindow()
    {
        damageWindowOpen = false;
    }

    public void EnableComboWindow()
    {
        if (comboStep == 1)
            canQueueNextAttack = true;
    }

    public void DisableComboWindow()
    {
        canQueueNextAttack = false;
    }

    public void ContinueComboOrReset()
    {
        canQueueNextAttack = false;

        if (comboStep == 1 && queuedNextAttack)
        {
            comboStep = 2;
            queuedNextAttack = false;
            swordAnimation.SetTrigger("Attack2");
            return;
        }

        ResetAttack();
    }

    public void ResetAttack()
    {
        canAttack = true;
        canQueueNextAttack = false;
        queuedNextAttack = false;
        comboStep = 0;
        damageWindowOpen = false;
        hitTargetsThisSwing.Clear();
    }

    private void SpawnBloodEffect(RaycastHit hit)
    {
        if (bloodBurstPrefab == null) return;

        Quaternion rot = Quaternion.LookRotation(hit.normal);
        Instantiate(bloodBurstPrefab, hit.point, rot);
    }
}