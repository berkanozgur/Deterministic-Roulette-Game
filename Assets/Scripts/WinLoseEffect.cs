/*
 * Deterministic Roulette Game
 * 
 * Author Berkan Özgür
 * 
 * Simple visual and audio feedback for spin outcomes.
 */

using UnityEngine;

public class WinLoseEffect : MonoBehaviour
{
    [Header("Particle Effect")]
    [SerializeField] private ParticleSystem winParticles;
    [SerializeField] private Transform particleParent;
    [SerializeField] private float particleDuration;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip winSound_t1;
    [SerializeField] private AudioClip winSound_t2;
    [SerializeField] private AudioClip winSound_t3;

    public enum OutcomeTier
    {
        winWithLose,
        even,
        win
    }
    public void PlayWinEffect(Vector3 position, OutcomeTier tier)
    {
        // Spawn and play particles
        if (winParticles != null)
        {
            ParticleSystem instance = Instantiate(winParticles, particleParent);
            instance.Play();
            Destroy(instance.gameObject, particleDuration);
        }

        AudioClip winSound;
        switch (tier)
        {
            case OutcomeTier.winWithLose:
                winSound = winSound_t1;
                break;
            case OutcomeTier.even:
                winSound = winSound_t2;
                break;
            case OutcomeTier.win:
                winSound = winSound_t3;
                break;
            default:
                winSound = winSound_t1;
                break;
        }

        // Play sound
        if (audioSource != null && winSound != null)
        {
            audioSource.PlayOneShot(winSound);
        }
    }
}