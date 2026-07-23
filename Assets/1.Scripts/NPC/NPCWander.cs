using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCWander : MonoBehaviour
{
    [Header("Wanderpunkte")]
    [SerializeField] private Transform wanderPointsParent;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip lookAroundAnimation;
    [SerializeField] private float fallbackLookDuration = 2f;

    [Header("Wartezeit")]
    [SerializeField] private float minimumWaitTime = 1f;
    [SerializeField] private float maximumWaitTime = 3f;

    private NavMeshAgent agent;
    private int lastPointIndex = -1;

    private static readonly int IsWalking =
        Animator.StringToHash("isWalking");

    private static readonly int IsLookingAround =
        Animator.StringToHash("isLookingAround");

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponent<Animator>();

        agent.updateRotation = true;
    }

    private IEnumerator Start()
    {
        // NavMesh einen Frame laden lassen.
        yield return null;

        if (!agent.isOnNavMesh)
        {
            Debug.LogError(
                "Der NPC steht nicht auf einem gebackenen NavMesh.",
                this
            );

            yield break;
        }

        if (wanderPointsParent == null ||
            wanderPointsParent.childCount == 0)
        {
            Debug.LogError("Keine Wanderpunkte eingetragen.", this);
            yield break;
        }

        while (true)
        {
            // Am aktuellen Ort anhalten.
            agent.isStopped = true;
            SetWalking(false);

            yield return new WaitForSeconds(
                Random.Range(minimumWaitTime, maximumWaitTime)
            );

            // Erst vollständig umschauen.
            if (animator != null)
                animator.SetBool(IsLookingAround, true);

            float lookDuration = lookAroundAnimation != null
                ? lookAroundAnimation.length
                : fallbackLookDuration;

            yield return new WaitForSeconds(lookDuration);

            if (animator != null)
                animator.SetBool(IsLookingAround, false);

            // Erst nach der Animation ein neues Ziel wählen.
            Transform target = GetRandomPoint();

            agent.isStopped = false;

            if (!agent.SetDestination(target.position))
                continue;

            SetWalking(true);

            while (agent.pathPending)
                yield return null;

            if (agent.pathStatus != NavMeshPathStatus.PathComplete)
            {
                agent.ResetPath();
                SetWalking(false);
                continue;
            }

            while (agent.hasPath &&
                   agent.remainingDistance >
                   agent.stoppingDistance + 0.05f)
            {
                yield return null;
            }

            agent.ResetPath();
        }
    }

    private Transform GetRandomPoint()
    {
        int pointCount = wanderPointsParent.childCount;

        if (pointCount == 1)
            return wanderPointsParent.GetChild(0);

        int newIndex;

        do
        {
            newIndex = Random.Range(0, pointCount);
        }
        while (newIndex == lastPointIndex);

        lastPointIndex = newIndex;
        return wanderPointsParent.GetChild(newIndex);
    }

    private void SetWalking(bool walking)
    {
        if (animator != null)
            animator.SetBool(IsWalking, walking);
    }
}
