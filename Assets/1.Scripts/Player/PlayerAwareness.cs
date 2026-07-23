using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAwareness : MonoBehaviour
{
    [Serializable]
    private class IndicatorSlot
    {
        public RectTransform watchPivot;
        public RectTransform eyeGraphic;

        [HideInInspector]
        public Quaternion eyeStartRotation;
    }

    [Header("Sichtziel")]
    [SerializeField] private Transform sightTarget;

    [Header("Watched Indicator")]
    [SerializeField] private GameObject watchedIndicator;
    [SerializeField] private Transform directionCamera;

    [SerializeField, Range(1, 5)]
    private int maxIndicators = 5;

    [SerializeField]
    private IndicatorSlot[] indicatorSlots =
        new IndicatorSlot[5];

    [SerializeField] private float rotationSpeed = 12f;

    private readonly HashSet<NPCVision> watchingNPCs = new();
    private readonly List<NPCVision> sortedWatchers = new();

    public Transform SightTarget =>
        sightTarget != null ? sightTarget : transform;

    public bool IsBeingWatched =>
        watchingNPCs.Count > 0;

    private void Awake()
    {
        if (directionCamera == null && Camera.main != null)
            directionCamera = Camera.main.transform;

        foreach (IndicatorSlot slot in indicatorSlots)
        {
            if (slot != null && slot.eyeGraphic != null)
                slot.eyeStartRotation =
                    slot.eyeGraphic.localRotation;
        }

        HideAllIndicators();
        SetIndicatorVisible(false);
    }

    private void LateUpdate()
    {
        watchingNPCs.RemoveWhere(npc =>
            npc == null || !npc.isActiveAndEnabled);

        sortedWatchers.Clear();

        foreach (NPCVision npc in watchingNPCs)
        {
            if (npc != null)
                sortedWatchers.Add(npc);
        }

        // Die nächstgelegenen NPCs zuerst anzeigen.
        sortedWatchers.Sort((a, b) =>
        {
            float distanceA =
                (a.transform.position - transform.position)
                .sqrMagnitude;

            float distanceB =
                (b.transform.position - transform.position)
                .sqrMagnitude;

            return distanceA.CompareTo(distanceB);
        });

        int activeCount = Mathf.Min(
            sortedWatchers.Count,
            maxIndicators,
            indicatorSlots.Length
        );

        SetIndicatorVisible(activeCount > 0);

        if (activeCount == 0)
        {
            HideAllIndicators();
            return;
        }

        if (directionCamera == null && Camera.main != null)
            directionCamera = Camera.main.transform;

        for (int i = 0; i < indicatorSlots.Length; i++)
        {
            IndicatorSlot slot = indicatorSlots[i];

            if (slot == null || slot.watchPivot == null)
                continue;

            bool shouldBeVisible = i < activeCount;

            if (slot.watchPivot.gameObject.activeSelf != shouldBeVisible)
            {
                slot.watchPivot.gameObject.SetActive(
                    shouldBeVisible
                );
            }

            if (shouldBeVisible)
            {
                UpdateIndicatorDirection(
                    slot,
                    sortedWatchers[i].transform.position
                );
            }
        }
    }

    public void SetWatchedBy(NPCVision npc, bool watched)
    {
        if (npc == null)
            return;

        if (watched)
            watchingNPCs.Add(npc);
        else
            watchingNPCs.Remove(npc);
    }

    private void UpdateIndicatorDirection(
        IndicatorSlot slot,
        Vector3 watcherPosition)
    {
        if (slot.watchPivot == null ||
            directionCamera == null)
        {
            return;
        }

        Vector3 direction =
            watcherPosition - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        direction.Normalize();

        Vector3 cameraForward =
            Vector3.ProjectOnPlane(
                directionCamera.forward,
                Vector3.up
            );

        if (cameraForward.sqrMagnitude < 0.001f)
            return;

        cameraForward.Normalize();

        Vector3 cameraRight =
            Vector3.Cross(
                Vector3.up,
                cameraForward
            ).normalized;

        float rightAmount =
            Vector3.Dot(direction, cameraRight);

        float forwardAmount =
            Vector3.Dot(direction, cameraForward);

        float targetAngle =
            -Mathf.Atan2(
                rightAmount,
                forwardAmount
            ) * Mathf.Rad2Deg;

        float currentAngle =
            slot.watchPivot.localEulerAngles.z;

        float angle;

        if (rotationSpeed <= 0f)
        {
            angle = targetAngle;
        }
        else
        {
            float smoothing =
                1f - Mathf.Exp(
                    -rotationSpeed *
                    Time.unscaledDeltaTime
                );

            angle = Mathf.LerpAngle(
                currentAngle,
                targetAngle,
                smoothing
            );
        }

        slot.watchPivot.localRotation =
            Quaternion.Euler(0f, 0f, angle);

        // Auge gegenrotieren, damit es gerade bleibt.
        if (slot.eyeGraphic != null)
        {
            slot.eyeGraphic.localRotation =
                Quaternion.Euler(0f, 0f, -angle) *
                slot.eyeStartRotation;
        }
    }

    private void HideAllIndicators()
    {
        foreach (IndicatorSlot slot in indicatorSlots)
        {
            if (slot != null &&
                slot.watchPivot != null)
            {
                slot.watchPivot.gameObject.SetActive(false);
            }
        }
    }

    private void SetIndicatorVisible(bool visible)
    {
        if (watchedIndicator != null &&
            watchedIndicator.activeSelf != visible)
        {
            watchedIndicator.SetActive(visible);
        }
    }

    private void OnDisable()
    {
        watchingNPCs.Clear();
        sortedWatchers.Clear();

        HideAllIndicators();
        SetIndicatorVisible(false);
    }
}