/*
 * Deterministic Roulette Game
 * 
 * Author Berkan Özgür
 * 
 * Manages all betting operations: placing bets, tracking active bets,
 * calculating payouts, and managing player chips.
 * 
 */

using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class BettingManager : MonoBehaviour
{
    [SerializeField] private bool debugMode;
    public static BettingManager Instance { get; private set; }

    [Header("Player State")]
    [SerializeField] private int startingChipValue = 1000;
    private int playerChipValue = 1000;
    private int totalSpins = 0;
    private int capital;
    private int totalWinAmount;
    private int totalLoseAmount;
    [SerializeField] private Chip currentChip;

    [Header("UI elements")]
    [SerializeField] private TMP_Text bankAmountText;
    [SerializeField] private TMP_Text totalBetAmountText;
    [SerializeField] private TMP_Text chipAmountText;
    [SerializeField] private TMP_Text activeBetListElement; //Prefab for the active bet list, not a scene reference
    [SerializeField] private Transform activeBetList;
    [SerializeField] private GameObject noActiveBetMarker;

    private List<Bet> activeBets = new List<Bet>();
    private List<GameObject> chipVisuals = new List<GameObject>();

    [SerializeField] private List<BetNumber> allNumbers;
    [SerializeField] private BetNumber lastWinningNumber;
    [SerializeField] private BetHistory betHistory;


    public int GetTotalSpins() => totalSpins;
    public int GetCapital() => capital;
    public int GetProfitAmount() => totalWinAmount - totalLoseAmount;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (chipAmountText == null || bankAmountText == null || totalBetAmountText == null || activeBetList == null)
        {
            Debug.LogError("UI elements not assigned in the inspector!");
        }
        if (currentChip == null)
        {
            Debug.LogError("Select a default chip in the inspector"); //TODO Add a default chip and load 
        }

        LoadBank();
        UpdateActiveChipAmount();
        UpdateTotalBetAmount();
        LoadCapital();
        LoadWinAmount();
        LoadSpinCounter();
        LoadLoseAmount();

        if (betHistory == null)
        {
            betHistory = gameObject.AddComponent<BetHistory>();
        }
        betHistory.LoadHistory();
    }

    public void PlaceBet(BetLocation location)
    {
        if (playerChipValue < currentChip.value)
        {
            Debug.LogWarning("Not enough chips!");
            return;
        }

        Bet newBet = location.CreateBet(currentChip.value);
        AddNewBet(newBet);

        playerChipValue -= currentChip.value;

        UpdateBankAmount();

        SpawnChipVisual(newBet.chipPosition);

        LogDebug($"Placed {currentChip.value} chips on {location.GetBetType()}");
    }

    private void AddNewBet(Bet bet)
    {
        activeBets.Add(bet);

        if (noActiveBetMarker.activeSelf)
            noActiveBetMarker.SetActive(false);

        TMP_Text betText = Instantiate(activeBetListElement, activeBetList);

        betText.text = $"{bet.chipAmount}$ bet on {bet.betType} : {bet.GetNumbersString()}";

        UpdateTotalBetAmount();
    }

    private void ClearActiveBetList()
    {
        activeBetList.GetComponentsInChildren<TMP_Text>().ToList().ForEach(text => Destroy(text.gameObject));
        noActiveBetMarker.SetActive(true);
    }

    public void SetChip(Chip chip)
    {
        currentChip = chip;
        UpdateActiveChipAmount();
        LogDebug($"Chip value set to: {chip.value}");
    }

    public void ClearBets(bool returnChips)
    {
        if (returnChips)
        {
            foreach (var bet in activeBets)
            {
                playerChipValue += bet.chipAmount;
            }
        }
        activeBets.Clear();

        foreach (var chip in chipVisuals)
        {
            Destroy(chip);
        }
        chipVisuals.Clear();

        UpdateTotalBetAmount();
        UpdateBankAmount();

        ClearActiveBetList();
        LogDebug("All bets cleared");
    }

    public int ProcessOutcome(int outcomeNumber)
    {
        int totalWinnings = 0;
        int totalBetAmount = GetTotalBetAmount();
        foreach (var bet in activeBets)
        {
            bet.ProcessResult(outcomeNumber);

            if (bet.winAmount > 0)
            {

                totalWinnings += bet.winAmount;
                LogDebug($"Won {bet.winAmount} on {bet.betType}!");
            }
        }

        betHistory.AddMultipleEntries(activeBets, outcomeNumber);

        // Add winnings to player chips
        playerChipValue += totalWinnings;
        SaveBank();

        SaveSpinCounter();

        if (totalWinAmount > 0)
        {
            SaveWinAmount(totalWinnings);
        }
        SaveLoseAmount(totalBetAmount - totalWinnings);

        // Clear bets after outcome
        ClearBets(false);

        return totalWinnings;
    }

    private void UpdateActiveChipAmount()
    {
        chipAmountText.text = "Chip Value: " + currentChip.value.ToString() + " $";
    }

    private void UpdateBankAmount()
    {
        bankAmountText.text = playerChipValue.ToString() + " $";
    }

    private void UpdateTotalBetAmount()
    {
        if (isThereActiveBets())
            totalBetAmountText.text = GetTotalBetAmount().ToString() + " $";
        else
            totalBetAmountText.text = "0 $";
    }

    private bool isThereActiveBets()
    {
        return activeBets.Count > 0;
    }

    private int GetTotalBetAmount()
    {
        int totalAmount = 0;
        foreach (var bet in activeBets)
        {
            totalAmount += bet.chipAmount;
        }
        return totalAmount;
    }

    private void SpawnChipVisual(Vector3 position)
    {
        if (currentChip != null)
        {
            GameObject chip = Instantiate(currentChip.gameObject, position, Quaternion.identity);
            chipVisuals.Add(chip);
        }
    }

    public Vector3 GetResultPosition(int outcome)
    {
        BetNumber winningNumber = allNumbers.Find(num => num.number == outcome);
        lastWinningNumber = winningNumber;
        LogDebug($"Winning number: {winningNumber.number} at position {winningNumber.transform.localPosition}");
        Vector3 position = winningNumber.transform.GetChild(0).position;
        return new Vector3(position.x - 0.6f, position.y, position.z + 0.4f); //The roulette table asset pivot points are broken and not centered on the number, so we need to get the position of the child object which is a bit beetter aligned.
    }

    public void InjectMoney(int amount)
    {
        playerChipValue += amount;
        UpdateCapital(amount);
        UpdateBankAmount();
        LogDebug($"Injecting {amount} chips for testing.");
    }
    #region BetHistory
    public void PopulateHistoryPanel(Transform historyPanel, TMP_Text historyEntryPrefab, int maxEntries = 20)
    {
        // Clear existing entries
        foreach (Transform child in historyPanel)
        {
            Destroy(child.gameObject);
        }

        // Get recent history (most recent first)
        List<BetHistoryEntry> history = betHistory.GetRecentHistory(maxEntries);
        history.Reverse(); // Show most recent at top

        if (history.Count == 0)
        {
            TMP_Text emptyText = Instantiate(historyEntryPrefab, historyPanel);
            emptyText.text = "No bet history yet";
            emptyText.color = Color.gray;
            return;
        }

        // Create UI entry for each history item
        foreach (var entry in history)
        {
            TMP_Text entryText = Instantiate(historyEntryPrefab, historyPanel);
            entryText.text = entry.GetDisplayString();

            // Color code: green for wins, red for losses
            if (entry.winAmount > 0)
                entryText.color = Color.green;
            else
                entryText.color = Color.red;
        }
    }
    public void ClearBetHistory()
    {
        betHistory.ClearHistory();
        LogDebug("Bet history cleared");
    }
    public (int totalBets, int wins, int losses, int totalWagered, int totalWon, int netProfit) GetHistoryStats()
    {
        var stats = betHistory.GetStatistics();
        int netProfit = stats.totalWon - stats.totalWagered;
        return (stats.totalBets, stats.wins, stats.losses, stats.totalWagered, stats.totalWon, netProfit);
    }
    #endregion
    #region Persistent Player Stats
    private void SaveBank()
    {
        PlayerPrefs.SetInt("PlayerChips", playerChipValue);
    }

    private void LoadBank()
    {
        if (PlayerPrefs.HasKey("PlayerChips"))
        {
            playerChipValue = PlayerPrefs.GetInt("PlayerChips");
        }
        else
        {
            //First opening of the game, set starting chips
            playerChipValue = startingChipValue;
            PlayerPrefs.SetInt("PlayerChips", playerChipValue);
            UpdateCapital(startingChipValue);
        }
        UpdateBankAmount();
    }

    private void LoadCapital()
    {
        capital = PlayerPrefs.GetInt("PlayerCapital", 0);
    }

    private void UpdateCapital(int amount)
    {
        capital = PlayerPrefs.GetInt("PlayerCapital", 0);
        capital += amount;
        PlayerPrefs.SetInt("PlayerCapital", capital);
    }
    private void SaveWinAmount(int amount)
    {
        totalWinAmount += amount;
        PlayerPrefs.SetInt("TotalWinAmount", totalWinAmount);
    }

    private void LoadWinAmount()
    {
        if (PlayerPrefs.HasKey("TotalWinAmount"))
        {
            totalWinAmount = PlayerPrefs.GetInt("TotalWinAmount");
        }
        else
        {
            totalWinAmount = 0;
            PlayerPrefs.SetInt("TotalWinAmount", totalWinAmount);
        }
    }

    private void SaveLoseAmount(int amount)
    {
        totalLoseAmount += amount;
        PlayerPrefs.SetInt("TotalLoseAmount", totalLoseAmount);
    }

    private void LoadLoseAmount()
    {
        if (PlayerPrefs.HasKey("TotalLoseAmount"))
        {
            totalLoseAmount = PlayerPrefs.GetInt("TotalLoseAmount");
        }
        else
        {
            totalLoseAmount = 0;
            PlayerPrefs.SetInt("TotalLoseAmount", totalLoseAmount);
        }
    }

    private void SaveSpinCounter()
    {
        totalSpins++;
        PlayerPrefs.SetInt("TotalSpins", totalSpins);
    }

    private void LoadSpinCounter()
    {
        if (PlayerPrefs.HasKey("TotalSpins"))
        {
            totalSpins = PlayerPrefs.GetInt("TotalSpins");
        }
        else
        {
            totalSpins = 0;
            PlayerPrefs.SetInt("TotalSpins", totalSpins);
        }
    }

    #endregion

    private void LogDebug(string message)
    {
        if (debugMode)
            Debug.Log($"[{this.name}] {message}");
    }
}
