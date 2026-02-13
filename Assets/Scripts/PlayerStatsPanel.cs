using TMPro;
using UnityEngine;

public class PlayerStatsPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text totalSpins;
    [SerializeField] private TMP_Text totalCapital;
    [SerializeField] private TMP_Text totalProfit;
    [SerializeField] private TMP_Text historyEntryPrefab;
    [SerializeField] private Transform historyEntryParent;

    void OnEnable()
    {
        UpdateStats();
        RefreshHistory();
    }
    public void RefreshHistory()
    {
        BettingManager.Instance.PopulateHistoryPanel(historyEntryParent, historyEntryPrefab, 20);
    }
    private void UpdateStats()
    {
        totalSpins.text = $"Total Spins: {BettingManager.Instance.GetTotalSpins()}";
        totalCapital.text = $"Capital Invested: {BettingManager.Instance.GetCapital()}";
        int profit = BettingManager.Instance.GetProfitAmount();
        if (profit > 0)
        {
            totalProfit.text = $"Profit Earned: {profit}";
            totalProfit.color = Color.green;
        }
        else if (profit < 0)
        {
            totalProfit.text = $"Loss Incurred: {-profit}";
            totalProfit.color = Color.red;
        }
        else
        {
            totalProfit.text = "No Profit or Loss";
            totalProfit.color = Color.white;
        }
    }

}
