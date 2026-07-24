using TMPro;
using UnityEngine;

public enum FoodQuality
{
    Green,
    Yellow,
    Orange,
    Red
}

public class FoodInteractable : MonoBehaviour, ImInteractible
{
    [Header("Food")]
    [SerializeField] private string foodName = "Burger";

    [SerializeField] private FoodQuality quality;
    [SerializeField] private int saturationAmount;

    [Header("Highlight")]
    [SerializeField] private GameObject outlineObject;
    [SerializeField] private Renderer[] outlineRenderers;

    [Header("Nametag")]
    [SerializeField] private Canvas nameCanvas;
    [SerializeField] private TMP_Text nameText;

    private static readonly int OutlineColorID =
        Shader.PropertyToID("_OutlineColor");

    private MaterialPropertyBlock propertyBlock;
    private Camera mainCamera;
    private Color qualityColor;
    private bool isFocused;

    [Header("Trash")]
    [SerializeField, Range(0f, 1f)]
    private float trashPukeChance = 0.15f;

    public string InteractionPrompt => "Essen";

    public FoodQuality Quality => quality;
    public int SaturationAmount => saturationAmount;

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
        mainCamera = Camera.main;

        if ((outlineRenderers == null ||
             outlineRenderers.Length == 0) &&
            outlineObject != null)
        {
            outlineRenderers =
                outlineObject.GetComponentsInChildren<Renderer>(true);
        }
    }

    private void OnEnable()
    {
        // Wird bei jedem Spawn neu ausgewürfelt.
        RandomizeQuality();
        SetFocused(false);
    }

    private void RandomizeQuality()
    {
        // Gesamtgewicht: 20 + 12 + 9 + 5 = 46
        int randomValue = Random.Range(0, 46);

        if (randomValue < 20)
        {
            quality = FoodQuality.Green;
            saturationAmount = 40;
            qualityColor = new Color(0.1f, 1f, 0.15f);
        }
        else if (randomValue < 32) // 20 + 12
        {
            quality = FoodQuality.Yellow;
            saturationAmount = 21;
            qualityColor = new Color(1f, 0.85f, 0.05f);
        }
        else if (randomValue < 41) // 20 + 12 + 9
        {
            quality = FoodQuality.Orange;
            saturationAmount = 9;
            qualityColor = new Color(1f, 0.3f, 0.02f);
        }
        else
        {
            quality = FoodQuality.Red;
            saturationAmount = 5;
            qualityColor = new Color(1f, 0.05f, 0.05f);
        }

        UpdateOutlineColor();
        UpdateNametag();
    }

    public void SetFocused(bool focused)
    {
        isFocused = focused;

        if (outlineObject != null)
            outlineObject.SetActive(focused);

        if (nameCanvas != null)
            nameCanvas.gameObject.SetActive(focused);

        if (focused)
        {
            UpdateOutlineColor();
            UpdateNametag();
        }
    }

    private void UpdateOutlineColor()
    {
        if (outlineRenderers == null)
            return;

        foreach (Renderer outlineRenderer in outlineRenderers)
        {
            if (outlineRenderer == null)
                continue;

            propertyBlock.Clear();

            outlineRenderer.GetPropertyBlock(propertyBlock);

            propertyBlock.SetColor(
                OutlineColorID,
                qualityColor
            );

            outlineRenderer.SetPropertyBlock(propertyBlock);
        }
    }

    private void UpdateNametag()
    {
        if (nameText == null)
            return;

        nameText.text = foodName;
        nameText.color = qualityColor;
    }

    private void LateUpdate()
    {
        if (!isFocused || nameCanvas == null)
            return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera != null)
        {
            nameCanvas.transform.rotation =
                mainCamera.transform.rotation;
        }
    }

    public bool Interactor(Interactor interactor)
    {
        PlayerEating playerEating =
            interactor.GetComponentInParent<PlayerEating>();

        if (playerEating == null)
        {
            Debug.LogWarning(
                "PlayerEating wurde am Player nicht gefunden.",
                this
            );

            return false;
        }

        bool pukeAfterEating =
            quality == FoodQuality.Red &&
            Random.value < trashPukeChance;

        return playerEating.StartEating(
            saturationAmount,
            gameObject,
            pukeAfterEating
        );
    }

    private void OnDisable()
    {
        SetFocused(false);
    }
}
