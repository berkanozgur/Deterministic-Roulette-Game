/*
 * Deterministic Roulette Game
 * 
 * Author Berkan Özgür
 * 
 * Represents a clickable area on the betting table.
 * Handles hover effects, bet placement, and chip visualization.
 * 
 */

using System.Collections.Generic;
using UnityEngine;

public class BetLocation : MonoBehaviour
{
    [Header("Bet Configuration")]
    [SerializeField] private BetType betType;
    [SerializeField] private List<BetNumber> coveredNumbers = new List<BetNumber>();
    [SerializeField] private int payoutRatio;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject hoverHighlight;
    [SerializeField] private Transform chipSpawnPoint;
    [SerializeField] private Material betTableHighlight;

    private List<Renderer> renderers = new List<Renderer>();

    // To store original materials for slot 1 not good for scaling but its structured based on our current asset.
    // If we had more complex assets with more material slots, we would need a more robust system to track which slot is being replaced and restore it correctly.
    private Dictionary<Renderer, Material> originalMatSlot1 = new Dictionary<Renderer, Material>(); 
    private bool isHovered = false;

    void Awake()
    {
        if (chipSpawnPoint == null)
            chipSpawnPoint = transform;
        if (hoverHighlight != null)
            hoverHighlight.SetActive(false);
    }
    private void Start()
    {
        foreach (BetNumber number in coveredNumbers)
        {
            if (number.numberRenderer != null)
                renderers.Add(number.numberRenderer);
        }
    }

    public void OnHoverEnter()
    {
        isHovered = true;
        ShowHoverEffect();
    }

    public void OnHoverExit()
    {
        isHovered = false;
        HideHoverEffect();
    }
    public void OnClick()
    {
        BettingManager.Instance.PlaceBet(this);
    }

    private void ShowHoverEffect()
    {
        if (hoverHighlight != null)
            hoverHighlight.SetActive(true);
        AddHighlight(renderers);
    }

    private void HideHoverEffect()
    {
        if (hoverHighlight != null)
            hoverHighlight.SetActive(false);
        RemoveHighlight(renderers);
    }

    public void AddHighlight(List<Renderer> renderers)
    {
        foreach (var rend in renderers)
        {
            if (rend == null) continue;
            if (originalMatSlot1.ContainsKey(rend)) continue;

            var mats = rend.sharedMaterials;

            // Safety check
            if (mats.Length < 2)
            {
                Debug.LogWarning($"Renderer {rend.name} has less than 2 materials.");
                continue;
            }

            // Save original
            originalMatSlot1[rend] = mats[1];

            // Replace slot 1
            mats[1] = betTableHighlight;
            rend.sharedMaterials = mats;
        }
    }

    public void RemoveHighlight(List<Renderer> renderers)
    {
        foreach (var rend in renderers)
        {
            if (rend == null) continue;

            if (!originalMatSlot1.TryGetValue(rend, out var original))
                continue;

            var mats = rend.sharedMaterials;

            if (mats.Length < 2)
                continue;

            // Restore original material
            mats[1] = original;
            rend.sharedMaterials = mats;

            originalMatSlot1.Remove(rend);
        }
    }

    public Bet CreateBet(int chipAmount)
    {
        Vector3 chipPosition = chipSpawnPoint != null ? chipSpawnPoint.position : transform.position;
        return new Bet(betType, coveredNumbers, chipAmount, payoutRatio, chipPosition);
    }

    public void Configure(BetType type, List<BetNumber> numbers, int payout)
    {
        betType = type;
        coveredNumbers = new List<BetNumber>(numbers);
        payoutRatio = payout;
    }

}
