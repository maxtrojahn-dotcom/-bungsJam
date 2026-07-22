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

    private void UpdateSaturationUI()
    {
        if (saturationSlider != null)
            saturationSlider.value = CurrentSaturation;
    }

    private void LoadGameOverScene()
    {
        isGameOver = true;
        SceneManager.LoadScene(gameOverSceneName);
    }
}