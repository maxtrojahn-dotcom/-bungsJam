using System.Collections.Generic;
using UnityEngine;

public class NPCRandomAppearance : MonoBehaviour
{
    [Header("Modelle")]
    [SerializeField] private Transform modelsParent;
    [SerializeField] private string modelNamePrefix = "SM_Chr_";

    [Header("Debug")]
    [SerializeField] private bool logSelectedModel;

    private readonly List<GameObject> modelVariants = new();

    private void Awake()
    {
        PickRandomAppearance();
    }

    [ContextMenu("Pick Random Appearance")]
    public void PickRandomAppearance()
    {
        Transform container =
            modelsParent != null ? modelsParent : transform;

        modelVariants.Clear();

        foreach (Transform child in container)
        {
            // Root wird dadurch nicht ausgewählt.
            if (child.name.StartsWith(modelNamePrefix))
                modelVariants.Add(child.gameObject);
        }

        if (modelVariants.Count == 0)
        {
            Debug.LogWarning(
                "Keine NPC-Modelle mit dem Präfix " +
                modelNamePrefix + " gefunden.",
                this
            );

            return;
        }

        foreach (GameObject model in modelVariants)
            model.SetActive(false);

        GameObject selectedModel =
            modelVariants[Random.Range(0, modelVariants.Count)];

        selectedModel.SetActive(true);

        if (logSelectedModel)
            Debug.Log("NPC-Modell gewählt: " + selectedModel.name, this);
    }
}
