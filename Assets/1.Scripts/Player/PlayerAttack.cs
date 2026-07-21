using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack")]
    public Transform attackPoint;
    public float attackRange = 1.5f;
    public int damage = 5;
    public LayerMask enemyLayer;

    [Header("Combo")]
    public float timeBetweenAttacks = 0.4f;
    public float comboResetTime = 1.0f;

    private Animator animator;
    private float lastAttackTime;
    private int comboStep = 0;
    private bool canAttack = true;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (Time.time - lastAttackTime > comboResetTime)
        {
            comboStep = 0;
        }

        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            Attack();
        }
    }

    private void Attack()
    {
        canAttack = false;
        lastAttackTime = Time.time;

        comboStep++;

        if (comboStep > 2)
            comboStep = 1;

        animator.SetInteger("ComboStep", comboStep);
        animator.SetTrigger("Attack");

        Invoke(nameof(EnableNextAttack), timeBetweenAttacks);
    }

    public void DealDamage()
    {
        Debug.Log("Attack ausgelöst");

        Collider[] enemies = Physics.OverlapSphere(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        Debug.Log("Gefundene Gegner: " + enemies.Length);

        foreach (Collider enemy in enemies)
        {
            Debug.Log("Getroffen: " + enemy.name);

            EnemyHealth health = enemy.GetComponentInParent<EnemyHealth>();

            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }

    private void EnableNextAttack()
    {
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

}