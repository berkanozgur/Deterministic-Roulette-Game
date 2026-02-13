/*
 * Deterministic Roulette Game
 * 
 * Author Berkan Özgür
 * 
 * Manages bet history storage and retrieval using JSON and PlayerPrefs.
 */

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BetHistoryData
{
    public List<BetHistoryEntry> entries = new List<BetHistoryEntry>();
}

public class BetHistory : MonoBehaviour
{
    private const string SAVE_KEY = "BetHistory";
    private const int MAX_HISTORY_ENTRIES = 100; // Limit history size

    private BetHistoryData historyData = new BetHistoryData();

    public void AddHistoryEntry(Bet bet, int outcome)
    {
        BetHistoryEntry entry = new BetHistoryEntry(bet, outcome);
        historyData.entries.Add(entry);

        // Trim old entries if exceeding max
        if (historyData.entries.Count > MAX_HISTORY_ENTRIES)
        {
            historyData.entries.RemoveAt(0);
        }

        SaveHistory();
    }

    public void AddMultipleEntries(List<Bet> bets, int outcome)
    {
        foreach (var bet in bets)
        {
            AddHistoryEntry(bet, outcome);
        }
    }

    public List<BetHistoryEntry> GetHistory()
    {
        return new List<BetHistoryEntry>(historyData.entries);
    }

    public List<BetHistoryEntry> GetRecentHistory(int count)
    {
        int startIndex = Mathf.Max(0, historyData.entries.Count - count);
        int actualCount = Mathf.Min(count, historyData.entries.Count);

        List<BetHistoryEntry> recent = new List<BetHistoryEntry>();
        for (int i = startIndex; i < historyData.entries.Count; i++)
        {
            recent.Add(historyData.entries[i]);
        }

        return recent;
    }

    public void ClearHistory()
    {
        historyData.entries.Clear();
        SaveHistory();
    }

    private void SaveHistory()
    {
        string json = JsonUtility.ToJson(historyData);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    public void LoadHistory()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            historyData = JsonUtility.FromJson<BetHistoryData>(json);

            if (historyData == null)
                historyData = new BetHistoryData();
        }
        else
        {
            historyData = new BetHistoryData();
        }
    }

    /// <summary>
    /// Get statistics from history
    /// </summary>
    public (int totalBets, int wins, int losses, int totalWagered, int totalWon) GetStatistics()
    {
        int totalBets = historyData.entries.Count;
        int wins = 0;
        int losses = 0;
        int totalWagered = 0;
        int totalWon = 0;

        foreach (var entry in historyData.entries)
        {
            totalWagered += entry.chipAmount;

            if (entry.winAmount > 0)
            {
                wins++;
                totalWon += entry.winAmount;
            }
            else
            {
                losses++;
            }
        }

        return (totalBets, wins, losses, totalWagered, totalWon);
    }
}

[Serializable]
public class BetHistoryEntry
{
    public string betType;
    public string numbers;
    public int chipAmount;
    public int winAmount;
    public string timestamp;
    public int outcomeNumber;

    public BetHistoryEntry(Bet bet, int outcome)
    {
        this.betType = bet.betType.ToString();
        this.numbers = bet.GetNumbersString();
        this.chipAmount = bet.chipAmount;
        this.winAmount = bet.winAmount;
        this.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.outcomeNumber = outcome;
    }

    /// <summary>
    /// Format entry for display
    /// </summary>
    public string GetDisplayString()
    {
        string result = winAmount > 0 ? $"WON {winAmount}$" : $"LOST {chipAmount}$";
        return $"[{timestamp}] {betType} on {numbers} - {result} (Result: {GetOutcomeDisplayString()})";
    }

    private string GetOutcomeDisplayString()
    {
        return outcomeNumber == -1 ? "00" : outcomeNumber.ToString();
    }
}
