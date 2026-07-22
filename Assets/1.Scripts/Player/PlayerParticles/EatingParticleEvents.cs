using UnityEngine;

public class EatingParticleEvents : MonoBehaviour
{
    [Header("Food Particles")]
    [SerializeField] private ParticleSystem foodParticles;

    [SerializeField, Range(1, 20)]
    private int particlesPerBite = 8;

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