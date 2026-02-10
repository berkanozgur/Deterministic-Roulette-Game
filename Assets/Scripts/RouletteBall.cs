/*
 * Deterministic Roulette Game - Ball Component
 * 
 * Author Berkan Özgür 
 * 
 * This script manages the ball's visual spinning animation.
 * The ball spins counter-clockwise around the rim, then settles at 12 o'clock position.
 * The parent container rotation provides the visual randomness.
 * 
 * ToDo
 * Make ball animation more interesting (e.g. add bouncing, spiraling inward).
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
    [SerializeField] private int minBallRotations = 6;
    [SerializeField] private int maxBallRotations = 9;
    [SerializeField] private AnimationCurve radiusCurve; // Ball spirals inward
    [SerializeField] private AnimationCurve heightCurve; // Ball bounces

    private const float LANDING_ANGLE = 0f;


    public IEnumerator Spin()
    {
        int ballRotations = Random.Range(minBallRotations, maxBallRotations);
        float totalBallRotation = -(360f * ballRotations); 

        float startAngle = Random.Range(0f, 360f);

        float rotationToLanding = LANDING_ANGLE - startAngle;

        while (rotationToLanding > 180f) rotationToLanding -= 360f;
        while (rotationToLanding < -180f) rotationToLanding += 360f;

        totalBallRotation += rotationToLanding;

        LogDebug($"Ball starting at {startAngle}°, spinning {ballRotations} rotations counter-clockwise");
        LogDebug($"Total rotation: {totalBallRotation}°, landing at {LANDING_ANGLE}°");

        float elapsedTime = 0f;

        while (elapsedTime < spinDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / spinDuration;

            float currentAngle = startAngle + (totalBallRotation * t);
            float angleRad = currentAngle * Mathf.Deg2Rad;

            float currentRadius = Mathf.Lerp(rimRadius, pocketRadius, radiusCurve.Evaluate(t));

            float currentHeight = ballHeight * heightCurve.Evaluate(t);

            Vector3 localPosition = new Vector3(
                Mathf.Cos(angleRad) * currentRadius,
                currentHeight,
                Mathf.Sin(angleRad) * currentRadius
            );

            ballTransform.localPosition = localPosition;

            yield return null;
        }

        float finalAngleRad = LANDING_ANGLE * Mathf.Deg2Rad;
        Vector3 finalPosition = new Vector3(
            Mathf.Cos(finalAngleRad) * pocketRadius,
            0.05f,
            Mathf.Sin(finalAngleRad) * pocketRadius
        );

        ballTransform.localPosition = finalPosition;

        LogDebug($"Ball landed at {LANDING_ANGLE}° (12 o'clock position)");
    }

    void LogDebug(string message)
    {
        if (debugMode)
            Debug.Log($"[{this.name}] {message}");
    }
}