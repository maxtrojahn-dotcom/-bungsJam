using UnityEngine;

public class EnemyImpactVFX : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem groundImpactParticles;

    public void PlayGroundImpact()
    {
        if (groundImpactParticles != null)
        {
            groundImpactParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            groundImpactParticles.Play(true);
        }

        CameraShake.Instance?.Shake();
    }
}