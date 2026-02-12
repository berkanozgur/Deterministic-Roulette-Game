/*
 * Deterministic Roulette Game
 * 
 * Author Berkan Özgür
 * 
 * Represents a single number on the roulette wheel.
 * Used to track which numbers are included in different bet types.
 *
 */

using UnityEngine;

public class BetNumber : MonoBehaviour
{
    public int number;
    public Renderer numberRenderer;

    private void Awake()
    {
        if (numberRenderer == null)
            numberRenderer = GetComponent<Renderer>();
    }

    public BetNumber(int number)
    {
        this.number = number;
    }
}
