/*
 * Deterministic Roulette Game
 * 
 * Author Berkan Özgür 
 * 
 * This script manages UI connections and user interactions.
 * 
 * 
 */

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RouletteController : MonoBehaviour
{
    [SerializeField] private bool debugMode = false;

    [Header("Roulette Elements")]
    [SerializeField] private RouletteWheel rouletteWheel;
    [SerializeField] private RouletteBall rouletteBall;
    [SerializeField] private GameObject resultMarker;
    [SerializeField] private Vector3 resultMarkerRestPosition;

    [Header("UI Components")]
    [SerializeField] private Button spinButton;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private BetLocationManager betLocationManager;
    [SerializeField] private TMP_InputField deterministicInput;
    [SerializeField] private Button setNumberButton;
    [SerializeField] private TMP_Text deterministicStatusText;
    [SerializeField] private Color winResultColor = Color.green;
    [SerializeField] private Color loseResultColor = Color.red;
    [SerializeField] private Color defaultResultColor = Color.white;

    [Header("Result Marker Animation")]
    [SerializeField] private float markerAnimationDuration = 0.5f;
    [SerializeField] private AnimationCurve markerMoveCurve; // Optional: for easing
    [SerializeField] private float markerHoverHeight = 1.6f;

    private Coroutine currentMarkerAnimation;

    [SerializeField] private WinLoseEffect winLoseEffect;

    private bool isSpinning = false;
    private int? predeterminedNumber = null;

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
        HideResultMarker();

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

        bool hasActiveBet = BettingManager.Instance.HasActiveBets();
        SetResultText(rouletteWheel.GetNumberDisplayString(outcome), BettingManager.Instance.GetTotalBetAmount(), BettingManager.Instance.ProcessOutcome(outcome), resultPosition, hasActiveBet);

        // Reset for next spin
        spinButton.interactable = true;
        betLocationManager.handleInputs = true;
        isSpinning = false;
    }

    private void SetResultText(string displayNumber, int betAmount, int winnings, Vector3 resultPosition, bool hasActiveBet)
    {
        resultText.text = $"Result: {displayNumber}";

        if (winnings > 0)
        {
            if (betAmount < winnings) //Profit this round
            {
                resultText.color = winResultColor;
                resultText.text += $"\n Bet:{betAmount}$ and Won: {winnings}$ !";
                winLoseEffect.PlayWinEffect(resultPosition, WinLoseEffect.OutcomeTier.win);
            }
            else if (betAmount == winnings) //Won back bet amount, no profit
            {
                resultText.color = defaultResultColor;
                resultText.text += $"\n Bet:{betAmount}$ and Won: {winnings}$";
                winLoseEffect.PlayWinEffect(resultPosition, WinLoseEffect.OutcomeTier.even);
            }
            else //Won something but less than bet amount, net loss
            {
                resultText.color = loseResultColor;
                resultText.text += $"\n Bet: {betAmount}$ and Won: {winnings}$";
                winLoseEffect.PlayWinEffect(resultPosition, WinLoseEffect.OutcomeTier.winWithLose);
            }
        }
        else
        {
            if (betAmount > 0)
            {
                resultText.color = loseResultColor;
                resultText.text += $"\nYou lost {betAmount}$ bet";
            }
            else
            {
                resultText.color = defaultResultColor;
                resultText.text += "\nPlace a bet for next spin!";
            }
        }
        LogDebug($"Result: {displayNumber}");
    }

    #region DollyMarker

    private void SetResultMarker(Vector3 outcomePosition)
    {
        LogDebug($"Setting result marker at position: {outcomePosition}");

        // Stop any ongoing animation
        if (currentMarkerAnimation != null)
            StopCoroutine(currentMarkerAnimation);

        Vector3 targetPosition = outcomePosition + Vector3.up * markerHoverHeight;
        currentMarkerAnimation = StartCoroutine(AnimateResultMarker(targetPosition));
    }

    private void HideResultMarker()
    {
        // Stop any ongoing animation
        if (currentMarkerAnimation != null)
            StopCoroutine(currentMarkerAnimation);

        currentMarkerAnimation = StartCoroutine(AnimateResultMarker(resultMarkerRestPosition));
    }

    private IEnumerator AnimateResultMarker(Vector3 targetPosition)
    {
        // Make sure marker is active during animation
        if (!resultMarker.activeSelf)
            resultMarker.SetActive(true);

        Vector3 startPosition = resultMarker.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < markerAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / markerAnimationDuration;

            // Apply curve if assigned, otherwise linear
            float curveValue = markerMoveCurve != null ? markerMoveCurve.Evaluate(t) : t;

            resultMarker.transform.position = Vector3.Lerp(startPosition, targetPosition, curveValue);

            yield return null;
        }

        // Ensure exact final position
        resultMarker.transform.position = targetPosition;

        currentMarkerAnimation = null;
    }
    #endregion


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