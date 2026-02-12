/*
 * Deterministic Roulette Game
 * 
 * Author Berkan Özgür 
 * 
 * This script manages the ball's visual spinning animation.
 * The ball spins counter-clockwise around the rim, then settles at 12 o'clock position.
 * The parent container rotation provides the visual randomness.
 * 
 * ToDo
 * Im not happy with the ball bounce yet, it needs tweaking and maybe some sound effects.
 * Add summaries to methods
 * 
 */
using System.Collections;
using UnityEngine;

public class RouletteBall : MonoBehaviour
{
    [SerializeField] private bool debugMode;

    [SerializeField] private Transform ballTransform;
    [SerializeField] private Transform wheelCenter;

    [SerializeField] private float rimRadius = 2.5f;
    [SerializeField] private float pocketRadius = 1.8f;
    [SerializeField] private float ballHeight = 0.2f;

    [SerializeField] private float spinDuration = 4f;
    [SerializeField] private float settlingDuration = 1f;
    [SerializeField] private int minBallRotations = 6;
    [SerializeField] private int maxBallRotations = 9;

    //Curves to control the animation of the ball, its easier to test and tweak the animation with curves rather than hardcoding values in code.s
    [SerializeField] private AnimationCurve radiusCurve; //Ball should start at the rim and starts spinning inward
    [SerializeField] private AnimationCurve heightCurve; //Ball bounce etc. 
    [SerializeField] private AnimationCurve ballSpinCurve; //Ball should slow down at the end.

    private const float LANDING_ANGLE = 0f;


    public IEnumerator Spin()
    {
        const float START_ANGLE = 0f;
        const float END_ANGLE = 0f;

        int ballRotations = Random.Range(minBallRotations, maxBallRotations);

        float totalBallRotation = (360f * ballRotations);

        LogDebug($"Ball spinning {ballRotations} full rotations counter-clockwise");
        LogDebug($"Starting at {START_ANGLE}°, ending at {END_ANGLE}°");

        float elapsedTime = 0f;

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

            yield return null;
        }

        Vector3 finalPosition = new Vector3(
            0f,              // X = 0 (12 o'clock)
            0.1f,          // Y = just above pocket floor
            pocketRadius    // Z = positive (12 o'clock)
        );

        ballTransform.localPosition = finalPosition;

        yield return SettleInPocket(); //??
         
        LogDebug($"Ball landed at 12 o'clock position: {finalPosition}");
    }

    /// <summary>
    /// This has no effect just leaving it here for future improvements, maybe some small random drift in the pocket or a bounce sound effect.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SettleInPocket()
    {
        float elapsedTime = 0f;
        Vector3 targetPosition = new Vector3(0f, 0.05f, pocketRadius);

        float driftRadius = 0.15f; 
        float driftAngle = 30f;

        while (elapsedTime < settlingDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / settlingDuration;

            float easeValue = 1f - Mathf.Pow(1f - t, 3f);
            float currentDrift = driftRadius * (1f - easeValue);

            // Circular motion that spirals inward
            float currentAngle = driftAngle * (1f - easeValue);
            float angleRad = currentAngle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(
                Mathf.Sin(angleRad) * currentDrift,
                0f,
                Mathf.Cos(angleRad) * currentDrift
            );

            ballTransform.localPosition = targetPosition + offset;

            yield return null;
        }

        ballTransform.localPosition = targetPosition;
    }
    void LogDebug(string message)
    {
        if (debugMode)
            Debug.Log($"[{this.name}] {message}");
    }
}