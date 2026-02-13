/*
 * Deterministic Roulette Game
 * 
 * Author Berkan Özgür 
 * 
 * This script manages UI connections and user interactions.
 * 
 * Todo: 
 * Add result tracking and statistics.
 * Add summaries to methods
 * 
 */

using System.Collections;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RouletteController : MonoBehaviour
{
    [SerializeField] private bool debugMode = false;

    [SerializeField] private RouletteWheel rouletteWheel;
    [SerializeField] private RouletteBall rouletteBall;
    [SerializeField] private GameObject resultMarker;
    [SerializeField] private Vector3 resultMarkerRestPosition;

    [SerializeField] private Button spinButton;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private BetLocationManager betLocationManager;

    [SerializeField] private TMP_InputField deterministicInput;
    [SerializeField] private Button setNumberButton;
    [SerializeField] private TMP_Text deterministicStatusText;

    [SerializeField] private WinLoseEffect winLoseEffect;

    private bool isSpinning = false;
    [SerializeField] private int? predeterminedNumber;

    void Start()
    {
        resultMarkerRestPosition = resultMarker.transform.position;
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
        //Disable inputs during spin
        isSpinning = true;
        spinButton.interactable = false;
        betLocationManager.handleInputs = false;
        resultText.text = "Spinning...";

        Coroutine wheelSpin = StartCoroutine(rouletteWheel.SpinToNumber(outcome));
        Coroutine ballSpin = StartCoroutine(rouletteBall.Spin());

        yield return wheelSpin;
        yield return ballSpin;

        // Small delay before showing result
        yield return new WaitForSeconds(0.5f);

        Vector3 resultPosition = BettingManager.Instance.GetResultPosition(outcome);
        SetResultMarker(resultPosition);

        string displayNumber = rouletteWheel.GetNumberDisplayString(outcome);
        resultText.text = $"Result: {displayNumber}";
        LogDebug($"Result: {displayNumber}");

        int winnings = BettingManager.Instance.ProcessOutcome(outcome);
        if (winnings > 0)
        {
            resultText.text += $" Won: {winnings} chips!";

            winLoseEffect.PlayWinEffect(resultPosition, winnings);
        }
        else
        {
            resultText.text += " No winnings this time.";
        }

        // Reset for next spin
        spinButton.interactable = true;
        betLocationManager.handleInputs = true;
        isSpinning = false;
    }

    private void SetResultMarker(Vector3 outcomePosition)
    {
        LogDebug($"Setting result marker at position: {outcomePosition}");
        resultMarker.transform.position = outcomePosition + Vector3.up * 0.1f;
        resultMarker.SetActive(true);
    }

    private void HideResultMarker()
    {
        resultMarker.transform.position = Vector3.zero;
        resultMarker.SetActive(false);
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

    private void LogDebug(string message)
    {
        if (debugMode)
            Debug.Log($"[{this.name}] {message}");
    }

}