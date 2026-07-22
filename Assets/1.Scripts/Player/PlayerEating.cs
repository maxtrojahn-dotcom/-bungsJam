using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEating : MonoBehaviour
{
    [Header("Eating")]
    [SerializeField] private float eatingDuration = 2f;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerSaturation playerSaturation;
    [SerializeField] private Animator animator;


    [Header("Eating Progress UI")]
    [SerializeField] private GameObject eatingProgressObject;
    [SerializeField] private Slider eatingProgressSlider;

    [Header("Eye Hearts")]
    [SerializeField] private GameObject[] eyeHeartObjects;
    [SerializeField] private GameObject targetObject;

    public bool IsEating { get; private set; }

    private int currentEyeHearts = 3;
    private bool lostEyeDuringCurrentMeal;
    private Coroutine eatingCoroutine;

    private void Awake()
    {
        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        if (playerSaturation == null)
            playerSaturation = GetComponent<PlayerSaturation>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

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

    public bool StartEating(int saturationAmount, GameObject foodObject)
    {
        if (IsEating)
            return false;

        eatingCoroutine = StartCoroutine(
            EatingRoutine(saturationAmount, foodObject)
        );

        return true;
    }

    private IEnumerator EatingRoutine(
        int saturationAmount,
        GameObject foodObject)
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

        // Essen wurde vollständig abgeschlossen.
        if (playerSaturation != null)
            playerSaturation.AddSaturation(saturationAmount);

        if (foodObject != null)
            Destroy(foodObject);

        FinishEating();
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

        // Keine Sättigung und kein Zerstören des Essens.
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
        if (eatingProgressSlider != null && !visible)
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

        lostEyeDuringCurrentMeal = true;
        currentEyeHearts--;

        UpdateEyeHearts();

        if (currentEyeHearts <= 0 && targetObject != null)
            targetObject.SetActive(true);
    }

    private void UpdateEyeHearts()
    {
        for (int i = 0; i < eyeHeartObjects.Length; i++)
        {
            if (eyeHeartObjects[i] != null)
                eyeHeartObjects[i].SetActive(i < currentEyeHearts);
        }
    }
}