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

    public Bet(BetType type, List<BetNumber> numbers, int amount, int payout, Vector3 position)
    {
        this.betType = type;
        this.numbers = new List<BetNumber>(numbers);
        this.chipAmount = amount;
        this.payoutRatio = payout;
        this.chipPosition = position;
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

    public int CalculatePayout()
    {
        return chipAmount + (chipAmount * payoutRatio);
    }
}
