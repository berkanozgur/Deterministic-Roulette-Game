/*
 * Deterministic Roulette Game
 * 
 * Author Berkan Özgür 
 * 
 * This script manages UI connections and user interactions.
 * 
 * Todo: 
 * Add result tracking and statistics.
 * 
 */

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RouletteController : MonoBehaviour
{
    [SerializeField] private bool debugMode = false;

    [SerializeField] private RouletteWheel rouletteWheel;

    [SerializeField] private Button spinButton;
    [SerializeField] private TMP_Text resultText;

    [SerializeField] private TMP_InputField deterministicInput;
    [SerializeField] private Button setNumberButton;
    [SerializeField] private TMP_Text deterministicStatusText;

    private bool isSpinning = false;
    [SerializeField] private int? predeterminedNumber;

    void Start()
    {
        spinButton.onClick.AddListener(OnSpinButtonPressed);
        setNumberButton.onClick.AddListener(OnSetDeterministicNumber);

        UpdateDeterministicStatus();
    }

    public void OnSpinButtonPressed()
    {
        if (isSpinning)
        {
            LogDebug("Already spinning, ignoring input.");
            return;
        }


        int outcome;
        if (predeterminedNumber.HasValue)
        {
            outcome = predeterminedNumber.Value;
            predeterminedNumber = null;
            UpdateDeterministicStatus();
        }
        else
        {
            outcome = rouletteWheel.GetRandomOutcome();
        }

        StartCoroutine(SpinSequence(outcome));
    }

    private void OnSetDeterministicNumber()
    {
        string input = deterministicInput.text.Trim().ToUpper();

        if (input == "00")
        {
            predeterminedNumber = -1;
            UpdateDeterministicStatus();
            LogDebug("Next spin set to: 00");
            return;
        }

        if (int.TryParse(input, out int number))
        {
            if (rouletteWheel.IsValidNumber(number))
            {
                predeterminedNumber = number;
                UpdateDeterministicStatus();
                LogDebug($"Next spin set to: {number}");
            }
            else
            {
                LogDebug("Invalid number! Enter 0-36 or 00");
            }
        }
        else
        {
            LogDebug("Invalid input! Enter 0-36 or 00");
        }
    }

    private IEnumerator SpinSequence(int outcome)
    {
        isSpinning = true;
        spinButton.interactable = false;
        resultText.text = "Spinning...";

        yield return StartCoroutine(rouletteWheel.SpinToNumber(outcome));

        yield return new WaitForSeconds(0.5f);

        string displayNumber = rouletteWheel.GetNumberDisplayString(outcome);
        resultText.text = $"Result: {displayNumber}";
        LogDebug($"Result: {displayNumber}");

        // Reset for next spin
        spinButton.interactable = true;
        isSpinning = false;
    }

    private void UpdateDeterministicStatus()
    {
        if (predeterminedNumber.HasValue)
        {
            string displayNumber = rouletteWheel.GetNumberDisplayString(predeterminedNumber.Value);
            deterministicStatusText.text = $"Next: {displayNumber}";
            deterministicStatusText.color = Color.yellow;
        }
        else
        {
            deterministicStatusText.text = "Next: Random";
            deterministicStatusText.color = Color.white;
        }
    }

    public void ClearDeterministicNumber()
    {
        predeterminedNumber = null;
        UpdateDeterministicStatus();
    }

    void LogDebug(string message)
    {
        if (debugMode)
            Debug.Log($"[{this.name}] {message}");
    }

}