using UnityEngine;

public class PlayerFootstepVFX : MonoBehaviour
{
    [Header("Footstep Particle Systems")]
    [SerializeField] private ParticleSystem leftFootParticles;
    [SerializeField] private ParticleSystem rightFootParticles;

    public void LeftFootstep()
    {
        PlayParticles(leftFootParticles);
    }

    public void RightFootstep()
    {
        PlayParticles(rightFootParticles);
    }

    private void PlayParticles(ParticleSystem particles)
    {
        if (particles == null)
        {
            return;
        }

        particles.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear
        );

        particles.Play(true);
    }
}
