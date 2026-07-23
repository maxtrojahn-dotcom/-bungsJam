using System.Collections.Generic;
using UnityEngine;

public class PlayerAwareness : MonoBehaviour
{
    [SerializeField] private Transform sightTarget;
    [SerializeField] private bool isBeingWatched;
    [SerializeField] private GameObject watchedIndicator;

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

        if (watchedIndicator != null)
            watchedIndicator.SetActive(isBeingWatched);
    }
    private void Awake()
    {
        if (watchedIndicator != null)
            watchedIndicator.SetActive(false);
    }

    private void OnDisable()
    {
        watchingNPCs.Clear();
        isBeingWatched = false;

        if (watchedIndicator != null)
            watchedIndicator.SetActive(false);
    }
}