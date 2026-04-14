using UnityEngine;

public class DayEndManager : MonoBehaviour
{
    public static DayEndManager Instance { get; private set; }

    [Header("Panels")]
    public DebtFailedPanelUI debtFailedPanelUI;

    private bool resolvingDayEnd = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ResolveDayEnd()
    {
        if (resolvingDayEnd)
            return;

        resolvingDayEnd = true;

        if (MoneyManager.Instance == null)
        {
            Debug.LogWarning("[DayEnd] MoneyManager missing.");
            resolvingDayEnd = false;
            return;
        }

        bool paid = MoneyManager.Instance.TryPayDebtForDay();

        if (!paid)
        {
            Debug.Log("[DayEnd] Could not pay debt. Day failed.");

            if (debtFailedPanelUI != null)
                debtFailedPanelUI.Show();

            return;
        }

        Debug.Log("[DayEnd] Debt paid successfully.");

        StartNextDay();
    }

    public void StartNextDay()
    {
        if (DayNightCycle.Instance != null)
            DayNightCycle.Instance.ResetDay();

        if (MoneyManager.Instance != null)
            MoneyManager.Instance.StartNextDay();

        if (TaxiRideManager.Instance != null)
            TaxiRideManager.Instance.AdvanceUpgradeDay();

        resolvingDayEnd = false;

        Debug.Log("[DayEnd] Next day started.");
    }
}