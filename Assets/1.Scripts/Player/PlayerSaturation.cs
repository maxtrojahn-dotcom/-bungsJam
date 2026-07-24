using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerSaturation : MonoBehaviour
{
    [Header("Sättigung")]
    [SerializeField, Range(1, 100)]
    private int startSaturation = 20;

    [SerializeField, Min(0.1f)]
    private float secondsPerPoint = 3f;

    [Header("Übergeben")]
    [SerializeField, Range(1, 100)]
    private int pukeThreshold = 80;

    [SerializeField]
    private PlayerPuke playerPuke;

    [Header("UI")]
    [SerializeField]
    private Slider saturationSlider;

    [SerializeField]
    private TMP_Text saturationText;

    [Header("Szenen")]
    [SerializeField]
    private string gameOverSceneName = "GameOver";

    [SerializeField]
    private string winningSceneName = "WinningScreen";

    [SerializeField, Range(0, 100)]
    private int winningSaturationMin = 73;

    [SerializeField, Range(0, 100)]
    private int winningSaturationMax = 80;

    public int CurrentSaturation { get; private set; }

    private CharacterController characterController;
    private float movementTimer;
    private bool isSceneLoading;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (playerPuke == null)
            playerPuke = GetComponent<PlayerPuke>();
    }

    private void Start()
    {
        CurrentSaturation = startSaturation;

        if (saturationSlider != null)
        {
            saturationSlider.minValue = 0;
            saturationSlider.maxValue = 100;
            saturationSlider.wholeNumbers = true;
        }

        UpdateSaturationUI();
        CheckSaturationState();
    }

    private void Update()
    {
        if (isSceneLoading)
            return;

        Vector3 horizontalVelocity = characterController.velocity;
        horizontalVelocity.y = 0f;

        if (horizontalVelocity.sqrMagnitude > 0.01f)
        {
            movementTimer += Time.deltaTime;

            if (movementTimer >= secondsPerPoint)
            {
                movementTimer = 0f;
                RemoveSaturation(1);
            }
        }
    }

    public void AddSaturation(int amount)
    {
        CurrentSaturation = Mathf.Clamp(
            CurrentSaturation + amount,
            0,
            100
        );

        UpdateSaturationUI();
        CheckSaturationState();
    }

    public void RemoveSaturation(int amount)
    {
        CurrentSaturation = Mathf.Clamp(
            CurrentSaturation - amount,
            0,
            100
        );

        UpdateSaturationUI();
        CheckSaturationState();
    }

    public void SetSaturation(int value)
    {
        CurrentSaturation = Mathf.Clamp(value, 0, 100);
        movementTimer = 0f;

        UpdateSaturationUI();
        CheckSaturationState();
    }

    private void CheckSaturationState()
    {
        if (isSceneLoading)
            return;

        // Bei 0 Sättigung: Game Over
        if (CurrentSaturation <= 0)
        {
            LoadScene(gameOverSceneName, "Game-Over");
            return;
        }

        int minimum = Mathf.Min(
            winningSaturationMin,
            winningSaturationMax
        );

        int maximum = Mathf.Max(
            winningSaturationMin,
            winningSaturationMax
        );

        // Zwischen 73 und 80 einschließlich: Sieg
        if (CurrentSaturation >= minimum &&
            CurrentSaturation <= maximum)
        {
            LoadScene(winningSceneName, "Winning");
            return;
        }

        // Übergeben, falls der Gewinnbereich überschritten wurde
        if (CurrentSaturation >= pukeThreshold &&
            playerPuke != null)
        {
            playerPuke.TryPuke();
        }
    }

    private void UpdateSaturationUI()
    {
        if (saturationSlider != null)
            saturationSlider.value = CurrentSaturation;

        if (saturationText != null)
            saturationText.text = CurrentSaturation + "%";
    }

    private void LoadScene(string sceneName, string sceneType)
    {
        if (isSceneLoading)
            return;

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError(
                sceneType +
                "-Szene ist in PlayerSaturation nicht eingetragen.",
                this
            );

            return;
        }

        isSceneLoading = true;
        SceneManager.LoadScene(sceneName);
    }
}