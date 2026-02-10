/*
 * Deterministic Roulette Game
 * 
 * Author Berkan Özgür 
 * 
 * This script manages the roulette wheel's behavior, including spinning to a specific number and determining the outcome of a spin. 
 * It uses an animation curve to control the spin's acceleration and deceleration, ensuring a smooth and visually appealing spin. 
 * 
 * Notes:
 * The wheel layout is based on the American roulette configuration for now, with -1 representing "00".
 * It works like a wheel of fortune for now, target number always lands on top.
 * 
 * Todo:
 * Add ball animation and connect it here.
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

    [SerializeField] private AnimationCurve spinCurve;
    [SerializeField] private float spinDuration = 4f;
    [SerializeField] private int minExtraRotations = 4;
    [SerializeField] private int maxExtraRotations = 6;

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
        int pocketsToSpin;

        if (currentIndex == -2)
        {
            pocketsToSpin = targetIndex;
        }
        else if (targetIndex > currentIndex)
        {
            LogDebug("Target index is in front of new index");
            pocketsToSpin = targetIndex - currentIndex;
        }
        else // if (targetIndex <= currentIndex)
        {
            LogDebug("Target index before new index");

            pocketsToSpin = wheelLayout.Length - (currentIndex - targetIndex);
        }

        currentIndex = targetIndex;

        float targetAngle = pocketsToSpin * DEGREES_PER_POCKET;
        LogDebug($"Wheel needs to spin {targetAngle} degrees");


        int extraRotations = UnityEngine.Random.Range(minExtraRotations, maxExtraRotations);
        float totalRotation = targetAngle /* + (360f * extraRotations)*/;

        float startRotation = wheelTransform.eulerAngles.y;

        float elapsedTime = 0f;
        while (elapsedTime < spinDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / spinDuration;

            float curveValue = spinCurve.Evaluate(t);
            float currentRotation = startRotation - (totalRotation * curveValue);

            wheelTransform.rotation = Quaternion.Euler(0f, currentRotation, 0f);
            handleTransform.rotation = wheelTransform.rotation;
            yield return null;
        }
        wheelTransform.rotation = Quaternion.Euler(0f, startRotation - totalRotation, 0f);
        handleTransform.rotation = wheelTransform.rotation;

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
