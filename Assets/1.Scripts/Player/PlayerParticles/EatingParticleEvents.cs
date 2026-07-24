using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class EatingParticleEvents : MonoBehaviour
{
    [Header("Food Particles")]
    [SerializeField] private ParticleSystem foodParticles;

    [SerializeField, Range(1, 20)]
    private int particlesPerBite = 8;

    [SerializeField] private AudioClip eatingSound;

    [SerializeField, Range(0f, 1f)]
    private float volume = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    // Animation Event
    public void PlayEatingSound()
    {
        if (eatingSound != null)
            audioSource.PlayOneShot(eatingSound, volume);
    }


    // Wird vom Animation Event aufgerufen.
    public void EmitFoodCrumbs()
    {
        if (foodParticles == null)
        {
            Debug.LogWarning(
                "Food Particles wurden nicht zugewiesen.",
                this
            );

            return;
        }

        foodParticles.Emit(particlesPerBite);
    }

    // Optional: beispielsweise bei Abbruch des Essens.
    public void ClearFoodCrumbs()
    {
        if (foodParticles == null)
            return;

        foodParticles.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear
        );
    }
}