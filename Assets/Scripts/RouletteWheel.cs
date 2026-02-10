/*
 * Deterministic Roulette Game
 * 
 * Author Berkan Özgür 
 * 
 * This script manages the roulette wheel's behavior, including spinning to a specific number and determining the outcome of a spin. 
 * It uses an animation curve to control the spin's acceleration and deceleration, ensuring a smooth and visually appealing spin. 
 * The correct number settles at 12 o'clock position in local rotation.
 * 
 * Notes:
 * The wheel layout is based on the American roulette configuration for now, with -1 representing "00".
 * 
 * Todo:
 * MAke ball animation more interesting.
 * Add european variation, different pocket per angle and wheel texture.
 * Add sound effects.
 * 
 * 
 */

using System;
using System.Collections;
using UnityEngine;

public class RouletteWheel : MonoBehaviour
{
    [SerializeField] private bool debugMode;

    [SerializeField] private Transform wheelTransform;
    [SerializeField] private Transform handleTransform;
    [SerializeField] private Transform rouletteContainer;

    [SerializeField] private AnimationCurve spinCurve;
    [SerializeField] private float spinDuration = 4f;
    [SerializeField] private int minExtraRotations = 4;
    [SerializeField] private int maxExtraRotations = 6;

    // Track current pocket position (-2 indicates initial state)
    [SerializeField] private int currentIndex = -2;


    /// <summary>
    /// American wheel layout, where -1 represents "00"
    /// </summary>
    private readonly int[] wheelLayout = new int[]
{
        0, 28, 9, 26, 30, 11, 7, 20, 32, 17, 5, 22, 34, 15, 3, 24, 36, 13, 1,
        -1, 27, 10, 25, 29, 12, 8, 19, 31, 18, 6, 21, 33, 16, 4, 23, 35, 14, 2
};

    private const int TOTAL_POCKETS = 38;
    private const float DEGREES_PER_POCKET = 360f / TOTAL_POCKETS;

    public int GetRandomOutcome()
    {
        int index = UnityEngine.Random.Range(0, TOTAL_POCKETS);
        return wheelLayout[index];
    }

    public IEnumerator SpinToNumber(int targetNumber)
    {
        int targetIndex = Array.IndexOf(wheelLayout, targetNumber);
        int pocketsToSpin = CalculatePocketsToSpin(targetIndex);

        int extraRotations = UnityEngine.Random.Range(minExtraRotations, maxExtraRotations);
        float extraRotationDegrees = 360f * extraRotations;

        float baseDegrees = pocketsToSpin * DEGREES_PER_POCKET;
        float totalRotation = baseDegrees + extraRotationDegrees;

        float randomContainerOffset = UnityEngine.Random.Range(0f, 360f);

        float startWheelRotation = wheelTransform.localEulerAngles.y;
        float startContainerRotation = rouletteContainer.eulerAngles.y;


        float elapsedTime = 0f;
        while (elapsedTime < spinDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / spinDuration;

            float curveValue = spinCurve.Evaluate(t);
            float currentRotation = startWheelRotation - (totalRotation * curveValue);

            wheelTransform.localRotation = Quaternion.Euler(0f, currentRotation, 0f);
            handleTransform.localRotation = wheelTransform.localRotation;

            float currentContainerRotation = startContainerRotation + (randomContainerOffset * curveValue);
            rouletteContainer.rotation = Quaternion.Euler(0f, currentContainerRotation, 0f);

            yield return null;
        }
        wheelTransform.localRotation = Quaternion.Euler(0f, startWheelRotation - totalRotation, 0f);
        handleTransform.localRotation = wheelTransform.localRotation;

        currentIndex = targetIndex;

    }

    private int CalculatePocketsToSpin(int targetIndex)
    {
        // First spin - go directly to target
        if (currentIndex == -2)
        {
            LogDebug("First spin - going directly to target");
            return targetIndex;
        }

        // Target is ahead (clockwise) - spin forward
        if (targetIndex > currentIndex)
        {
            LogDebug("Target is ahead");
            return targetIndex - currentIndex;
        }

        // Target is behind - wrap around
        LogDebug("Target is behind - wrapping around");
        return TOTAL_POCKETS - (currentIndex - targetIndex);
    }

    public string GetNumberDisplayString(int number)
    {
        if (number == -1)
            return "00";
        return number.ToString();
    }

    public bool IsValidNumber(int number)
    {
        return number == -1 || (number >= 0 && number <= 36);
    }
    void LogDebug(string message)
    {
        if (debugMode)
            Debug.Log($"[{this.name}] {message}");
    }
}
