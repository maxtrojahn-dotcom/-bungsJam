using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(EnemyBrain_Stupid))]
public class EnemyAttack : MonoBehaviour
{
    [Header("Trefferbereich")]

  
    [SerializeField, Min(0.1f)]
    private float attackRange = 2.6f;

    [SerializeField, Range(1f, 360f)]
    private float attackAngle = 84f;

    [Header("Sättigungsschaden")]

    [SerializeField, Range(0, 100)]
    private int minimumSaturationDamage = 20;

    [SerializeField, Range(0, 100)]
    private int maximumSaturationDamage = 45;

    [Header("Schlag-Sound")]

    [SerializeField]
    private AudioSource attackAudioSource;

    [SerializeField]
    private AudioClip attackSound;

    [SerializeField, Range(0f, 1f)]
    private float attackSoundVolume = 1f;

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

        // Sucht automatisch eine AudioSource auf diesem Objekt.
        if (attackAudioSource == null)
            attackAudioSource = GetComponent<AudioSource>();

        // Falls noch keine vorhanden ist, wird automatisch eine erstellt.
        if (attackAudioSource == null)
        {
            attackAudioSource =
                gameObject.AddComponent<AudioSource>();

            attackAudioSource.playOnAwake = false;
        }
    }

    public void BeginSwing()
    {
        swingCanHit = true;

        if (animator != null)
            animator.SetBool(AttackHash, true);
    }

    public void CancelSwing()
    {
        swingCanHit = false;

        if (animator != null)
            animator.SetBool(AttackHash, false);
    }

    // Wird durch das Animation Event "AttackHit" aufgerufen.
    public void AttackHit()
    {
        if (!swingCanHit)
            return;

        
        swingCanHit = false;

        if (animator != null)
            animator.SetBool(AttackHash, false);

        if (brain == null ||
            !brain.IsInAttackState ||
            brain.Target == null)
        {
            return;
        }

       
        PlayAttackSound();

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
                int minimumDamage = Mathf.Min(
                    minimumSaturationDamage,
                    maximumSaturationDamage
                );

                int maximumDamage = Mathf.Max(
                    minimumSaturationDamage,
                    maximumSaturationDamage
                );

                
                int saturationDamage = Random.Range(
                    minimumDamage,
                    maximumDamage + 1
                );

                saturation.RemoveSaturation(saturationDamage);

                Debug.Log(
                    "Enemy-Treffer: " +
                    saturationDamage +
                    "% Sättigung abgezogen. Übrig: " +
                    saturation.CurrentSaturation +
                    "%",
                    this
                );
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

    private void PlayAttackSound()
    {
        if (attackAudioSource == null ||
            attackSound == null)
        {
            return;
        }

        attackAudioSource.PlayOneShot(
            attackSound,
            attackSoundVolume
        );
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

        
        if (attackAngle >= 359.9f)
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

        if (attackAngle >= 359.9f)
            return;

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