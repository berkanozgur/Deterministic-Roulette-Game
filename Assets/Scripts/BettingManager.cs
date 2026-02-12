/*
 * Deterministic Roulette Game
 * 
 * Author Berkan Özgür
 * 
 * Manages all betting operations: placing bets, tracking active bets,
 * calculating payouts, and managing player chips.
 * 
 */

using System;
using UnityEngine;

public class BettingManager : MonoBehaviour
{
    public static BettingManager Instance { get; private set; }

    internal void PlaceBet(BetLocation betLocation)
    {
        throw new NotImplementedException();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

}
