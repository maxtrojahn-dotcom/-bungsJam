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

    [Header("Spielende")]
    [SerializeField]
    private GameObject winningScreen;

    [SerializeField]
    private string gameOverSceneName = "GameOverMenu";

    [SerializeField, Range(0, 100)]
    private int winningSaturationMin = 73;

    [SerializeField, Range(0, 100)]
    private int winningSaturationMax = 80;

    public int CurrentSaturation { get; private set; }

    private CharacterController characterController;
    private float movementTimer;
    private bool gameFinished;

    private void Awake()
    {
        // Wichtig, falls das Spiel beim Winning Screen pausiert wurde.
        Time.timeScale = 1f;

        characterController = GetComponent<CharacterController>();

        if (playerPuke == null)
            playerPuke = GetComponent<PlayerPuke>();

        // Winning Screen beim Start und nach einem Restart ausblenden.
        if (winningScreen != null)
            winningScreen.SetActive(false);
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
        if (gameFinished)
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
        if (gameFinished)
            return;

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
        if (gameFinished)
            return;

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
        if (gameFinished)
            return;

        CurrentSaturation = Mathf.Clamp(value, 0, 100);
        movementTimer = 0f;

        UpdateSaturationUI();
        CheckSaturationState();
    }

    private void CheckSaturationState()
    {
        if (gameFinished)
            return;

        // Bei 0 Sättigung Game-Over-Szene laden.
        if (CurrentSaturation <= 0)
        {
            LoadGameOverScene();
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

        // Zwischen 73 und 80 einschließlich gewinnen.
        if (CurrentSaturation >= minimum &&
            CurrentSaturation <= maximum)
        {
            ShowWinningScreen();
            return;
        }

        // Übergeben, wenn der Gewinnbereich übersprungen wurde.
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

    private void ShowWinningScreen()
    {
        if (winningScreen == null)
        {
            Debug.LogError(
                "Winning Screen wurde nicht eingetragen.",
                this
            );

            return;
        }

        gameFinished = true;
        winningScreen.SetActive(true);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void LoadGameOverScene()
    {
        if (string.IsNullOrWhiteSpace(gameOverSceneName))
        {
            Debug.LogError(
                "Game-Over-Szene wurde nicht eingetragen.",
                this
            );

            return;
        }

        gameFinished = true;
        Time.timeScale = 1f;

        SceneManager.LoadScene(gameOverSceneName);
    }
}