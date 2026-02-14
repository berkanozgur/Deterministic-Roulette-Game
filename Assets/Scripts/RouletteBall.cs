/*
 * Deterministic Roulette Game
 * 
 * Author Berkan Özgür 
 * 
 * This script manages the ball's visual spinning animation.
 * The ball spins counter-clockwise around the rim, then settles at 12 o'clock position.
 * The parent container rotation provides the visual randomness.
 * 
 * 
 */
using System.Collections;
using UnityEngine;

public class RouletteBall : MonoBehaviour
{
    [SerializeField] private bool debugMode;

    [Header("Roulette Elements")]
    [SerializeField] private Transform ballTransform;
    [SerializeField] private Transform wheelCenter;

    [Header("Spin Settings")]
    [SerializeField] private float rimRadius = 2.5f;
    [SerializeField] private float pocketRadius = 1.8f;
    [SerializeField] private float ballHeight = 0.2f;

    [SerializeField] private float spinDuration = 4f;
    [SerializeField] private float settlingDuration = 1f;
    [SerializeField] private int minBallRotations = 6;
    [SerializeField] private int maxBallRotations = 9;

    [Header("Animation Curves")]
    //Curves to control the animation of the ball, its easier to test and tweak the animation with curves rather than hardcoding values in code.s
    [Tooltip("Radius change over time to simulate ball placing in the pocket")]
    [SerializeField] private AnimationCurve radiusCurve; //Ball should start at the rim and starts spinning inward
    [Tooltip("Height change over time to simulate height difference from rim to pocket. (Can also be used for bounce)")]
    [SerializeField] private AnimationCurve heightCurve; //Ball bounce etc. 
    [SerializeField] private AnimationCurve ballSpinCurve; //Ball should slow down at the end.
    [Tooltip("The pitch decrease over time to simulate ball spinning speed decrease")]
    [SerializeField] private AnimationCurve pitchCurve; //Slow decrease at first, then faster decrease at the end to simulate ball losing speed.

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip ballRolling;
    [SerializeField] private AudioClip ballInPocket;
    [SerializeField] private float startPitch = 1.0f;
    [SerializeField] private float endPitch = 0.7f;

    private const float LANDING_ANGLE = 0f;

    /// <summary>
    /// Spin the ball around the rim counter-clockwise, then settle it at 12 o'clock position. 
    /// The spin is controlled by animation curves for a smooth and visually appealing effect. 
    /// The ball's position is updated in local space relative to the wheel center, allowing for easy adjustments to the animation. 
    /// </summary>
    /// <returns></returns>
    public IEnumerator Spin()
    {
        const float START_ANGLE = 0f;
        const float END_ANGLE = 0f;

        int ballRotations = Random.Range(minBallRotations, maxBallRotations);

        float totalBallRotation = (360f * ballRotations);

        LogDebug($"Ball spinning {ballRotations} full rotations counter-clockwise");
        LogDebug($"Starting at {START_ANGLE}°, ending at {END_ANGLE}°");

        PlayAudio(true, ballRolling);

        float elapsedTime = 0f;
        bool pocketSoundPlayed = false;

        while (elapsedTime < spinDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / spinDuration;

            float spinCurveValue = ballSpinCurve.Evaluate(t);

            float currentAngle = START_ANGLE + (totalBallRotation * spinCurveValue);
            float angleRad = currentAngle * Mathf.Deg2Rad;

            float currentRadius = Mathf.Lerp(rimRadius, pocketRadius, radiusCurve.Evaluate(t));

            // Bounce effect
            float currentHeight = ballHeight * heightCurve.Evaluate(t);

            // X = forward/back (12 o'clock is positive Z in Unity's default)
            // Z = left/right
            // Y = up/down
            Vector3 localPosition = new Vector3(
                Mathf.Sin(angleRad) * currentRadius,  // X axis
                currentHeight,                         // Y axis (height)
                Mathf.Cos(angleRad) * currentRadius   // Z axis (12 o'clock = positive Z)
            );

            ballTransform.localPosition = localPosition;

            if (audioSource != null)
            {
                float pitchCurveValue = pitchCurve != null ? pitchCurve.Evaluate(t) : t;
                audioSource.pitch = Mathf.Lerp(startPitch, endPitch, pitchCurveValue);
            }

            if (t >= 0.95f && !pocketSoundPlayed)
            {
                // Stop spin sound and play pocket sound
                PlayAudio(false, ballInPocket);
                pocketSoundPlayed = true;
            }

            yield return null;
        }

        Vector3 finalPosition = new Vector3(
            0f,              // X = 0 (12 o'clock)
            0.1f,          // Y = just above pocket floor
            pocketRadius    // Z = positive (12 o'clock)
        );

        ballTransform.localPosition = finalPosition;

        // Reset pitch to normal
        if (audioSource != null)
            audioSource.pitch = 1.0f;

        LogDebug($"Ball landed at 12 o'clock position: {finalPosition}");
    }

    /// <summary>
    /// Manage audio playback for the ball. Looping for spinning and one shot for landing.
    /// </summary>
    /// <param name="isLooping"></param>
    /// <param name="clip"></param>
    private void PlayAudio(bool isLooping, AudioClip clip)
    {
        if (audioSource == null || clip == null)
        {
            LogDebug("AudioSource or AudioClip is missing, cannot play audio.");
            return;
        }

        audioSource.Stop();
        if (isLooping)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            audioSource.loop = false;
            audioSource.PlayOneShot(clip);
        }
    }
    void LogDebug(string message)
    {
        if (debugMode)
            Debug.Log($"[{this.name}] {message}");
    }
}