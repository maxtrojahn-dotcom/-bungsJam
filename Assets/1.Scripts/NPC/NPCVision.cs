using UnityEngine;

public class NPCVision : MonoBehaviour
{
    [Header("Sicht")]
    [SerializeField] private Transform eyes;
    [SerializeField] private float viewDistance = 8f;

    [SerializeField, Range(0f, 180f)]
    private float viewAngle = 90f;

    [SerializeField] private LayerMask visionMask = ~0;

    private PlayerAwareness playerAwareness;
    private PlayerEating playerEating;

    private bool wasEatingAndSeen;

    private void Start()
    {
        playerAwareness =
            FindFirstObjectByType<PlayerAwareness>();

        if (playerAwareness == null)
        {
            Debug.LogError(
                "PlayerAwareness wurde nicht gefunden.",
                this
            );

            enabled = false;
            return;
        }

        playerEating =
            playerAwareness.GetComponent<PlayerEating>();

        if (playerEating == null)
        {
            playerEating =
                playerAwareness
                .GetComponentInChildren<PlayerEating>();
        }

        if (playerEating == null)
        {
            Debug.LogError(
                "PlayerEating wurde am Player nicht gefunden.",
                this
            );
        }
    }

    private void Update()
    {
        if (playerAwareness == null)
            return;

        bool canSeePlayer = CanSeePlayer();

        playerAwareness.SetWatchedBy(
            this,
            canSeePlayer
        );

        bool eatingAndSeen =
            canSeePlayer &&
            playerEating != null &&
            playerEating.IsEating;

        if (eatingAndSeen && !wasEatingAndSeen)
        {
            playerEating.SeenByEnemy();
        }

        wasEatingAndSeen = eatingAndSeen;
    }

    private bool CanSeePlayer()
    {
        if (eyes == null || playerAwareness == null)
            return false;

        Transform target =
            playerAwareness.SightTarget;

        Vector3 direction =
            target.position - eyes.position;

        float distance = direction.magnitude;

        if (distance > viewDistance ||
            distance <= 0.01f)
        {
            return false;
        }

        float angle = Vector3.Angle(
            eyes.forward,
            direction
        );

        if (angle > viewAngle * 0.5f)
            return false;

        if (!Physics.Raycast(
                eyes.position,
                direction.normalized,
                out RaycastHit hit,
                distance,
                visionMask,
                QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        PlayerAwareness hitPlayer =
            hit.collider
                .GetComponentInParent<PlayerAwareness>();

        return hitPlayer == playerAwareness;
    }

    private void OnDisable()
    {
        if (playerAwareness != null)
        {
            playerAwareness.SetWatchedBy(
                this,
                false
            );
        }

        wasEatingAndSeen = false;
    }

    private void OnDrawGizmosSelected()
    {
        Transform origin =
            eyes != null ? eyes : transform;

        Gizmos.color = Color.yellow;

        Vector3 left =
            Quaternion.Euler(
                0f,
                -viewAngle * 0.5f,
                0f
            ) * origin.forward;

        Vector3 right =
            Quaternion.Euler(
                0f,
                viewAngle * 0.5f,
                0f
            ) * origin.forward;

        Gizmos.DrawRay(
            origin.position,
            left * viewDistance
        );

        Gizmos.DrawRay(
            origin.position,
            right * viewDistance
        );
    }
}