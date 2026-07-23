using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class NPCDropZone : MonoBehaviour
{
    [Header("Mögliche Drops")]
    [SerializeField] private GameObject[] consumablePrefabs;

    [Header("Enthält beliebig viele Spawnpunkte")]
    [SerializeField] private Transform spawnPointsParent;

    [SerializeField, Range(0f, 100f)]
    private float dropChancePercent = 3f;

    [Tooltip("0 bedeutet: Das Objekt verschwindet nicht automatisch.")]
    [SerializeField] private float despawnTime = 60f;

    private readonly HashSet<NPCWander> npcsInside = new();
    private readonly List<Transform> spawnPoints = new();

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
        CollectSpawnPoints();
    }

    private void CollectSpawnPoints()
    {
        spawnPoints.Clear();

        if (spawnPointsParent == null)
            return;

        foreach (Transform child in spawnPointsParent)
        {
            spawnPoints.Add(child);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        NPCWander npc = other.GetComponentInParent<NPCWander>();

        // Verhindert mehrfache Auslösung durch mehrere NPC-Collider.
        if (npc == null || !npcsInside.Add(npc))
            return;

        TrySpawnDrop();
    }

    private void OnTriggerExit(Collider other)
    {
        NPCWander npc = other.GetComponentInParent<NPCWander>();

        if (npc != null)
            npcsInside.Remove(npc);
    }

    private void TrySpawnDrop()
    {
        if (consumablePrefabs == null ||
            consumablePrefabs.Length == 0)
        {
            Debug.LogWarning("Keine Consumable Prefabs eingetragen.", this);
            return;
        }

        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("Keine Spawnpunkte gefunden.", this);
            return;
        }

        if (Random.value > dropChancePercent / 100f)
            return;

        GameObject selectedPrefab =
            consumablePrefabs[
                Random.Range(0, consumablePrefabs.Length)
            ];

        Transform selectedSpawnPoint =
            spawnPoints[
                Random.Range(0, spawnPoints.Count)
            ];

        if (selectedPrefab == null)
            return;

        GameObject spawnedDrop = Instantiate(
            selectedPrefab,
            selectedSpawnPoint.position,
            selectedSpawnPoint.rotation
        );

        if (despawnTime > 0f)
            Destroy(spawnedDrop, despawnTime);
    }
}