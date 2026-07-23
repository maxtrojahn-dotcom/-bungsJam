using System.Collections.Generic;
using UnityEngine;

public class PlayerAwareness : MonoBehaviour
{
    [SerializeField] private Transform sightTarget;
    [SerializeField] private bool isBeingWatched;

    private readonly HashSet<NPCVision> watchingNPCs = new();

    public bool IsBeingWatched => isBeingWatched;

    public Transform SightTarget =>
        sightTarget != null ? sightTarget : transform;

    public void SetWatchedBy(NPCVision npc, bool watched)
    {
        if (watched)
            watchingNPCs.Add(npc);
        else
            watchingNPCs.Remove(npc);

        isBeingWatched = watchingNPCs.Count > 0;
    }

    private void OnDisable()
    {
        watchingNPCs.Clear();
        isBeingWatched = false;
    }
}