using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEating : MonoBehaviour
{
    [Header("Eating")]
    [SerializeField] private float eatingDuration = 2f;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerSaturation playerSaturation;
    [SerializeField] private PlayerPuke playerPuke;
    [SerializeField] private Animator animator;

    [Header("Eating Progress")]
    [SerializeField] private GameObject eatingProgressObject;
    [SerializeField] private Slider eatingProgressSlider;

    [Header("Eye Hearts")]
    [SerializeField] private GameObject[] eyeHeartObjects;
    [SerializeField] private GameObject targetObject;

    public bool IsEating { get; private set; }

    // Wird vom Guard abgefragt.
    public bool HasNoHeartEyes => currentEyeHearts <= 0;

    private int currentEyeHearts;
    private bool lostEyeDuringCurrentMeal;
    private Coroutine eatingCoroutine;

    private void Awake()
    {
        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        if (playerSaturation == null)
            playerSaturation = GetComponent<PlayerSaturation>();

        if (playerPuke == null)
            playerPuke = GetComponent<PlayerPuke>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        currentEyeHearts =
            eyeHeartObjects != null && eyeHeartObjects.Length > 0
                ? eyeHeartObjects.Length
                : 3;

        if (targetObject != null)
            targetObject.SetActive(false);

        if (eatingProgressSlider != null)
        {
            eatingProgressSlider.minValue = 0f;
            eatingProgressSlider.maxValue = 1f;
            eatingProgressSlider.value = 0f;
        }

        SetEatingProgressVisible(false);
        UpdateEyeHearts();
    }

    public bool StartEating(
        int saturationAmount,
        GameObject foodObject,
        bool pukeAfterEating = false)
    {
        if (IsEating)
            return false;

        eatingCoroutine = StartCoroutine(
            EatingRoutine(
                saturationAmount,
                foodObject,
                pukeAfterEating
            )
        );

        return true;
    }

    private IEnumerator EatingRoutine(
        int saturationAmount,
        GameObject foodObject,
        bool pukeAfterEating)
    {
        IsEating = true;
        lostEyeDuringCurrentMeal = false;

        if (playerController != null)
            playerController.enabled = false;

        if (animator != null)
        {
            animator.SetBool("isMoving", false);
            animator.SetFloat("speed", 0f);
            animator.SetBool("isEating", true);
        }

        SetEatingProgressVisible(true);

        float eatingTimer = 0f;

        while (eatingTimer < eatingDuration)
        {
            eatingTimer += Time.deltaTime;

            if (eatingProgressSlider != null)
            {
                eatingProgressSlider.value =
                    Mathf.Clamp01(eatingTimer / eatingDuration);
            }

            yield return null;
        }

        if (playerSaturation != null)
            playerSaturation.AddSaturation(saturationAmount);

        if (foodObject != null)
            Destroy(foodObject);

        FinishEating();

        // Wird beispielsweise bei der 15-%-Trash-Chance verwendet.
        if (pukeAfterEating && playerPuke != null)
            playerPuke.TryPuke();
    }

    public void InterruptEating()
    {
        if (!IsEating)
            return;

        if (eatingCoroutine != null)
        {
            StopCoroutine(eatingCoroutine);
            eatingCoroutine = null;
        }

        // Bei einer Unterbrechung:
        // keine Sättigung, kein Zerstören, kein Übergeben.
        FinishEating();
    }

    private void FinishEating()
    {
        IsEating = false;
        eatingCoroutine = null;

        if (animator != null)
            animator.SetBool("isEating", false);

        if (playerController != null)
            playerController.enabled = true;

        SetEatingProgressVisible(false);
    }

    private void SetEatingProgressVisible(bool visible)
    {
        if (!visible && eatingProgressSlider != null)
            eatingProgressSlider.value = 0f;

        if (eatingProgressObject != null)
            eatingProgressObject.SetActive(visible);
    }

    public void SeenByEnemy()
    {
        if (!IsEating || lostEyeDuringCurrentMeal)
            return;

        if (currentEyeHearts <= 0)
            return;

        // Pro Essvorgang kann nur ein Herz verloren gehen.
        lostEyeDuringCurrentMeal = true;
        currentEyeHearts--;

        UpdateEyeHearts();

        if (currentEyeHearts <= 0 && targetObject != null)
            targetObject.SetActive(true);
    }

    // Wird vom Guard aufgerufen, wenn die Jagd beendet wird.
    public void RestoreAllHeartEyes()
    {
        currentEyeHearts =
            eyeHeartObjects != null && eyeHeartObjects.Length > 0
                ? eyeHeartObjects.Length
                : 3;

        lostEyeDuringCurrentMeal = false;

        UpdateEyeHearts();

        if (targetObject != null)
            targetObject.SetActive(false);
    }

    private void UpdateEyeHearts()
    {
        if (eyeHeartObjects == null)
            return;

        for (int i = 0; i < eyeHeartObjects.Length; i++)
        {
            if (eyeHeartObjects[i] != null)
            {
                eyeHeartObjects[i].SetActive(
                    i < currentEyeHearts
                );
            }
        }
    }

    private void OnDisable()
    {
        if (eatingCoroutine != null)
        {
            StopCoroutine(eatingCoroutine);
            eatingCoroutine = null;
        }

        IsEating = false;

        if (playerController != null)
            playerController.enabled = true;
    }
}