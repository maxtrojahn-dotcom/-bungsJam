using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    [Header("Game Over")]
    [SerializeField]
    private string gameOverSceneName = "GameOver";

    public int CurrentSaturation { get; private set; }

    private CharacterController characterController;
    private float movementTimer;
    private bool isGameOver;

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
        CheckPukeThreshold();
    }

    private void Update()
    {
        if (isGameOver)
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
        CheckPukeThreshold();
    }

    public void RemoveSaturation(int amount)
    {
        CurrentSaturation = Mathf.Clamp(
            CurrentSaturation - amount,
            0,
            100
        );

        UpdateSaturationUI();

        if (CurrentSaturation <= 0)
            LoadGameOverScene();
    }

    public void SetSaturation(int value)
    {
        CurrentSaturation = Mathf.Clamp(value, 0, 100);
        movementTimer = 0f;

        UpdateSaturationUI();

        if (CurrentSaturation <= 0)
        {
            LoadGameOverScene();
        }
        else
        {
            CheckPukeThreshold();
        }
    }

    private void CheckPukeThreshold()
    {
        if (isGameOver)
            return;

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
    }

    private void LoadGameOverScene()
    {
        if (isGameOver)
            return;

        isGameOver = true;
        SceneManager.LoadScene(gameOverSceneName);
    }
}