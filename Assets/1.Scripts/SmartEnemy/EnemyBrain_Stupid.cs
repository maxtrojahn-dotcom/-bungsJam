using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(EnemyReferences))]
[RequireComponent(typeof(EnemyAttack))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBrain_Stupid : MonoBehaviour
{
    private enum GuardState
    {
        Patrol,
        Hunt,
        Attack
    }

    [Header("Player")]
    [SerializeField] private Transform target;
    [SerializeField] private PlayerEating playerEating;

    [Header("Patrouille")]
    [SerializeField] private Transform patrolPointsParent;
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float patrolStoppingDistance = 0.25f;
    [SerializeField] private float waitAtPointSeconds = 1.5f;

    [Header("Jagd")]
    [SerializeField] private float huntSpeed = 3.5f;
    [SerializeField] private float huntDurationSeconds = 60f;
    [SerializeField] private float pathUpdateDelay = 0.15f;

    [Header("Angriff")]
    [SerializeField] private float attackCooldownSeconds = 1.5f;
    [SerializeField] private float attackFailSafeSeconds = 2.5f;
    [SerializeField] private float turnSpeed = 8f;

    [Header("Debug")]
    [SerializeField] private GuardState state;
    [SerializeField] private float huntTimeRemaining;

    private NavMeshAgent agent;
    private Animator animator;
    private EnemyAttack enemyAttack;

    private int patrolIndex = -1;
    private bool waitingAtPatrolPoint;
    private float patrolWaitRemaining;
    private float nextPathUpdateTime;
    private float nextAttackAllowedTime;
    private float attackFailSafeTime;

    private static readonly int SpeedHash =
        Animator.StringToHash("speed");

    public Transform Target => target;
    public bool IsInAttackState => state == GuardState.Attack;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyAttack = GetComponent<EnemyAttack>();
    }

    private void Start()
    {
        if (playerEating == null && target != null)
            playerEating = target.GetComponentInParent<PlayerEating>();

        if (playerEating == null)
            playerEating = FindFirstObjectByType<PlayerEating>();

        if (target == null && playerEating != null)
            target = playerEating.transform;

        if (target == null || playerEating == null)
        {
            Debug.LogError(
                "Guard findet PlayerEating oder den Player nicht.",
                this
            );

            enabled = false;
            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError(
                "Der Guard steht nicht auf einem gebackenen NavMesh.",
                this
            );

            enabled = false;
            return;
        }

        // Der NavMeshAgent bewegt den Guard.
        animator.applyRootMotion = false;

        EnterPatrol();
    }

    private void Update()
    {
        switch (state)
        {
            case GuardState.Patrol:
                UpdatePatrol();
                break;

            case GuardState.Hunt:
                UpdateHunt();
                break;

            case GuardState.Attack:
                UpdateAttack();
                break;
        }

        UpdateAnimatorSpeed();
    }

    private void UpdatePatrol()
    {
        if (playerEating.HasNoHeartEyes)
        {
            BeginHunt();
            return;
        }

        if (patrolPointsParent == null ||
            patrolPointsParent.childCount == 0)
        {
            agent.isStopped = true;
            return;
        }

        if (agent.pathPending)
            return;

        bool reachedPoint =
            !agent.hasPath ||
            agent.remainingDistance <=
            agent.stoppingDistance + 0.1f;

        if (!reachedPoint)
            return;

        if (!waitingAtPatrolPoint)
        {
            waitingAtPatrolPoint = true;
            patrolWaitRemaining = waitAtPointSeconds;
            agent.ResetPath();
        }

        patrolWaitRemaining -= Time.deltaTime;

        if (patrolWaitRemaining <= 0f)
        {
            waitingAtPatrolPoint = false;
            GoToNextPatrolPoint();
        }
    }

    private void GoToNextPatrolPoint()
    {
        if (patrolPointsParent == null ||
            patrolPointsParent.childCount == 0)
        {
            agent.isStopped = true;
            return;
        }

        for (int i = 0; i < patrolPointsParent.childCount; i++)
        {
            patrolIndex =
                (patrolIndex + 1) %
                patrolPointsParent.childCount;

            Transform point =
                patrolPointsParent.GetChild(patrolIndex);

            if (point == null)
                continue;

            agent.isStopped = false;
            agent.SetDestination(point.position);
            return;
        }

        agent.isStopped = true;
    }

    private void BeginHunt()
    {
        huntTimeRemaining = huntDurationSeconds;
        nextAttackAllowedTime = 0f;

        ResumeHunt();
    }

    private void ResumeHunt()
    {
        state = GuardState.Hunt;

        enemyAttack.CancelSwing();

        agent.speed = huntSpeed;
        agent.stoppingDistance =
            Mathf.Max(0.1f, enemyAttack.AttackRange * 0.8f);

        agent.isStopped = false;
        nextPathUpdateTime = 0f;
    }

    private void UpdateHunt()
    {
        if (!playerEating.HasNoHeartEyes)
        {
            EnterPatrol();
            return;
        }

        if (UpdateHuntTimer())
            return;

        float distance = FlatDistanceToTarget();

        if (distance <= enemyAttack.AttackRange)
        {
            if (!agent.isStopped)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }

            LookAtTarget();

            if (Time.time >= nextAttackAllowedTime)
                EnterAttack();

            return;
        }

        agent.isStopped = false;

        if (Time.time >= nextPathUpdateTime)
        {
            nextPathUpdateTime =
                Time.time + pathUpdateDelay;

            agent.SetDestination(target.position);
        }
    }

    private void EnterAttack()
    {
        state = GuardState.Attack;

        agent.isStopped = true;
        agent.ResetPath();

        LookAtTarget();

        nextAttackAllowedTime =
            Time.time + attackCooldownSeconds;

        attackFailSafeTime =
            Time.time + attackFailSafeSeconds;

        enemyAttack.BeginSwing();
    }

    private void UpdateAttack()
    {
        if (!playerEating.HasNoHeartEyes)
        {
            EnterPatrol();
            return;
        }

        if (UpdateHuntTimer())
            return;

        LookAtTarget();

        // Verhindert ein Festhängen, falls das Event fehlt.
        if (Time.time >= attackFailSafeTime)
            ResumeHunt();
    }

    public void OnAttackResolved(bool successfulHit)
    {
        if (state != GuardState.Attack)
            return;

        if (successfulHit)
            LoseInterest();
        else
            ResumeHunt();
    }

    private bool UpdateHuntTimer()
    {
        huntTimeRemaining -= Time.deltaTime;

        if (huntTimeRemaining > 0f)
            return false;

        LoseInterest();
        return true;
    }

    private void LoseInterest()
    {
        playerEating.RestoreAllHeartEyes();
        EnterPatrol();
    }

    private void EnterPatrol()
    {
        state = GuardState.Patrol;

        enemyAttack.CancelSwing();

        agent.speed = patrolSpeed;
        agent.stoppingDistance = patrolStoppingDistance;
        agent.isStopped = false;
        agent.ResetPath();

        waitingAtPatrolPoint = false;
        huntTimeRemaining = 0f;

        GoToNextPatrolPoint();
    }

    private float FlatDistanceToTarget()
    {
        Vector3 difference =
            target.position - transform.position;

        difference.y = 0f;

        return difference.magnitude;
    }

    private void LookAtTarget()
    {
        Vector3 direction =
            target.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );
    }

    private void UpdateAnimatorSpeed()
    {
        if (animator == null || agent == null)
            return;

        float blendValue = 0f;

        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            blendValue =
                state == GuardState.Patrol
                    ? 0f       // Walking
                    : 1f;      // Running bei der Jagd
        }

        animator.SetFloat(
            SpeedHash,
            blendValue,
            0.1f,
            Time.deltaTime
        );
    }
}
