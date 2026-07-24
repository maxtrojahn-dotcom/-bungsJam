using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerFootsteps : MonoBehaviour
{
    [SerializeField] private AudioClip leftFootSound;
    [SerializeField] private AudioClip rightFootSound;

    [SerializeField, Range(0f, 1f)]
    private float volume = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    // Animation Event für den linken Fuß
    public void FootstepLeft()
    {
        if (leftFootSound != null)
            audioSource.PlayOneShot(leftFootSound, volume);
    }

    // Animation Event für den rechten Fuß
    public void FootstepRight()
    {
        if (rightFootSound != null)
            audioSource.PlayOneShot(rightFootSound, volume);
    }
}