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
    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem winParticles;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip winSound_t1;
    [SerializeField] private AudioClip winSound_t2;
    [SerializeField] private AudioClip winSound_t3;


    public void PlayWinEffect(Vector3 position, int amount)
    {
        // Spawn and play particles
        if (winParticles != null)
        {
            ParticleSystem instance = Instantiate(winParticles, position, Quaternion.identity);
            instance.Play();
            Destroy(instance.gameObject, instance.main.duration + instance.main.startLifetime.constantMax);
        }

        AudioClip winSound;
        switch (amount)
        {
            case <50:
                winSound = winSound_t1;
                break;
            case <100:
                winSound = winSound_t2;
                break;
            case >100:
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