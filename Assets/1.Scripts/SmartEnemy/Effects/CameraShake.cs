using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("Shake Settings")]
    [SerializeField] private float duration = 0.14f;
    [SerializeField] private float strength = 0.07f;

    private Vector3 startLocalPosition;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        Instance = this;
        startLocalPosition = transform.localPosition;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void Shake()
    {
        if (duration <= 0f || strength <= 0f)
        {
            return;
        }

        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            transform.localPosition = startLocalPosition;
        }

        shakeCoroutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;
            float fadeOut = 1f - progress;

            Vector2 randomOffset =
                Random.insideUnitCircle * strength * fadeOut;

            transform.localPosition =
                startLocalPosition +
                new Vector3(
                    randomOffset.x,
                    randomOffset.y,
                    0f
                );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = startLocalPosition;
        shakeCoroutine = null;
    }
}