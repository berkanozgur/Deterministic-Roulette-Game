/*
 * Deterministic Roulette Game - Bet Types
 * 
 * Author Berkan Özgür
 * 
 * Defines all possible bet types in American Roulette.
 * Source for bet types and payout ratios: https://readwrite.com/gambling/guides/roulette-odds/
 * 
 * ToDo
 * European Roulette bet types
 * 
 */

public enum BetType
{
    // Inside Bets
    Straight,      // Single number (35:1)
    Split,         // Two adjacent numbers (17:1)
    Street,        // Three numbers in a row (11:1)
    Corner,        // Four numbers in a square (8:1)
    SixLine,       // Six numbers (two rows) (5:1)

    // American-specific
    Basket,        // 0, 00, 1, 2, 3 (6:1)

    // Outside Bets
    Red,           // All red numbers (1:1)
    Black,         // All black numbers (1:1)
    Even,          // All even numbers (1:1)
    Odd,           // All odd numbers (1:1)
    Low,           // 1-18 (1:1)
    High,          // 19-36 (1:1)
    Dozen1,        // 1-12 (2:1)
    Dozen2,        // 13-24 (2:1)
    Dozen3,        // 25-36 (2:1)
    Column1,       // First column (2:1)
    Column2,       // Second column (2:1)
    Column3        // Third column (2:1)
}
