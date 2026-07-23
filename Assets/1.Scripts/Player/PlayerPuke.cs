using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerPuke : MonoBehaviour
{
    [Header("Player-Referenzen")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerSaturation playerSaturation;
    [SerializeField] private Animator animator;

    [Header("Übergeben")]
    [SerializeField, Min(0.1f)]
    private float pukeDuration = 3f;

    [SerializeField, Range(0.05f, 1f)]
    private float movementMultiplier = 0.35f;

    [SerializeField, Range(0, 100)]
    private int saturationAfterPuke = 15;

    [Header("Partikel")]
    [SerializeField] private ParticleSystem pukeParticles;

    [Header("Sound")]
    [SerializeField] private AudioSource pukeAudioSource;
    [SerializeField] private AudioClip pukeSound;

    public bool IsPuking { get; private set; }

    private readonly int pukeTriggerHash =
        Animator.StringToHash("Puke");

    private Coroutine pukeCoroutine;
    private float normalMoveSpeed;
    private float normalDodgeRollSpeed;
    private bool movementIsReduced;

    private void Awake()
    {
        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        if (playerSaturation == null)
            playerSaturation = GetComponent<PlayerSaturation>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (pukeAudioSource == null)
            pukeAudioSource = GetComponent<AudioSource>();

        if (pukeParticles != null)
        {
            pukeParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }
    }

    public bool TryPuke()
    {
        if (IsPuking)
            return false;

        IsPuking = true;
        pukeCoroutine = StartCoroutine(PukeRoutine());

        return true;
    }

    private IEnumerator PukeRoutine()
    {
        // Dadurch kann zuerst die Essanimation sauber beendet werden.
        yield return null;

        if (playerSaturation != null)
            playerSaturation.SetSaturation(saturationAfterPuke);

        ReduceMovementSpeed();

        if (animator != null)
        {
            animator.ResetTrigger(pukeTriggerHash);
            animator.SetTrigger(pukeTriggerHash);
        }

        yield return new WaitForSeconds(pukeDuration);

        RestoreMovementSpeed();

        if (pukeParticles != null)
        {
            pukeParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmitting
            );
        }

        IsPuking = false;
        pukeCoroutine = null;
    }

    private void ReduceMovementSpeed()
    {
        if (playerController == null || movementIsReduced)
            return;

        normalMoveSpeed = playerController.moveSpeed;
        normalDodgeRollSpeed = playerController.dodgeRollSpeed;

        playerController.moveSpeed =
            normalMoveSpeed * movementMultiplier;

        playerController.dodgeRollSpeed =
            normalDodgeRollSpeed * movementMultiplier;

        movementIsReduced = true;
    }

    private void RestoreMovementSpeed()
    {
        if (playerController == null || !movementIsReduced)
            return;

        playerController.moveSpeed = normalMoveSpeed;
        playerController.dodgeRollSpeed = normalDodgeRollSpeed;

        movementIsReduced = false;
    }

    // Diese Methode wird durch ein Animation Event aufgerufen.
    public void PlayPukeEffects()
    {
        if (pukeParticles != null)
        {
            pukeParticles.gameObject.SetActive(true);

            pukeParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            pukeParticles.Play(true);
        }

        if (pukeAudioSource != null && pukeSound != null)
            pukeAudioSource.PlayOneShot(pukeSound);
    }

    // Optionales Animation Event am Ende der Animation.
    public void StopPukeParticles()
    {
        if (pukeParticles == null)
            return;

        pukeParticles.Stop(
            true,
            ParticleSystemStopBehavior.StopEmitting
        );
    }

    private void OnDisable()
    {
        if (pukeCoroutine != null)
        {
            StopCoroutine(pukeCoroutine);
            pukeCoroutine = null;
        }

        RestoreMovementSpeed();

        if (pukeParticles != null)
        {
            pukeParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }

        IsPuking = false;
    }
}