/*
 * Deterministic Roulette Game
 * 
 * Author Berkan Özgür
 * 
 * Represents a single bet placed by the player.
 * Stores bet amount, type, numbers covered, and payout ratio.
 * 
 */


using System.Collections.Generic;
using UnityEngine;

public class Bet : MonoBehaviour
{
    public BetType betType;
    public List<BetNumber> numbers;
    public int chipAmount;
    public int payoutRatio;
    public Vector3 chipPosition;
    public int winAmount;
    public bool isProcessed;

    public Bet(BetType type, List<BetNumber> numbers, int amount, int payout, Vector3 position)
    {
        this.betType = type;
        this.numbers = new List<BetNumber>(numbers);
        this.chipAmount = amount;
        this.payoutRatio = payout;
        this.chipPosition = position;
        this.winAmount = 0;
        this.isProcessed = false;
    }

    public bool IsWinningBet(int outcomeNumber)
    {
        foreach (var betNumber in numbers)
        {
            if (betNumber.number == outcomeNumber)
                return true;
        }
        return false;
    }
    public void ProcessResult(int outcomeNumber)
    {
        if (IsWinningBet(outcomeNumber))
        {
            winAmount = CalculatePayout();
        }
        else
        {
            winAmount = 0;
        }
        isProcessed = true;
    }

    public int CalculatePayout()
    {
        return chipAmount + (chipAmount * payoutRatio);
    }

    public string GetNumbersString()
    {
        if (numbers.Count > 1)
        {
            List<string> numberStrings = new List<string>();
            foreach (var num in numbers)
            {
                numberStrings.Add(num.number.ToString());
            }
            return string.Join(", ", numberStrings);
        }
        else if (numbers.Count == 1)
        {
            return numbers[0].number.ToString();
        }
        return "";
    }
}
