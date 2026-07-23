using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(EnemyBrain_Stupid))]
public class EnemyAttack : MonoBehaviour
{
    [Header("Treffer")]
    [SerializeField, Min(0.1f)]
    private float attackRange = 2.41f;

    [SerializeField, Range(1f, 180f)]
    private float attackAngle = 84f;

    [Header("Auswirkung")]
    [SerializeField, Range(0, 100)]
    private int hungerAfterHit = 20;

    private EnemyBrain_Stupid brain;
    private Animator animator;
    private bool swingCanHit;

    private static readonly int AttackHash =
        Animator.StringToHash("Attack");

    public float AttackRange => attackRange;

    private void Awake()
    {
        brain = GetComponent<EnemyBrain_Stupid>();
        animator = GetComponent<Animator>();
    }

    public void BeginSwing()
    {
        swingCanHit = true;
        animator.SetBool(AttackHash, true);
    }

    public void CancelSwing()
    {
        swingCanHit = false;

        if (animator != null)
            animator.SetBool(AttackHash, false);
    }

    // Diese Methode wird ausschließlich vom Animation Event aufgerufen.
    public void AttackHit()
    {
        if (!swingCanHit)
            return;

        // Selbst bei einem versehentlich doppelten Event trifft
        // derselbe Schlag dadurch nur einmal.
        swingCanHit = false;
        animator.SetBool(AttackHash, false);

        if (!brain.IsInAttackState || brain.Target == null)
            return;

        bool successfulHit = IsTargetInsideHitArea();

        if (successfulHit)
        {
            PlayerSaturation saturation =
                brain.Target.GetComponentInParent<PlayerSaturation>();

            if (saturation == null)
            {
                saturation =
                    brain.Target.GetComponentInChildren<PlayerSaturation>();
            }

            if (saturation != null)
            {
                saturation.SetSaturation(hungerAfterHit);
            }
            else
            {
                Debug.LogError(
                    "PlayerSaturation wurde am Player nicht gefunden.",
                    this
                );
            }
        }

        brain.OnAttackResolved(successfulHit);
    }

    private bool IsTargetInsideHitArea()
    {
        Vector3 direction =
            brain.Target.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude >
            attackRange * attackRange)
        {
            return false;
        }

        if (direction.sqrMagnitude <= 0.001f)
            return true;

        float angle =
            Vector3.Angle(transform.forward, direction);

        return angle <= attackAngle * 0.5f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );

        Vector3 left =
            Quaternion.Euler(
                0f,
                -attackAngle * 0.5f,
                0f
            ) * transform.forward;

        Vector3 right =
            Quaternion.Euler(
                0f,
                attackAngle * 0.5f,
                0f
            ) * transform.forward;

        Gizmos.DrawRay(
            transform.position,
            left * attackRange
        );

        Gizmos.DrawRay(
            transform.position,
            right * attackRange
        );
    }
}