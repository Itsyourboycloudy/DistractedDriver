using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [Header("Starting Values")]
    public int startingDay = 1;
    public float startingCash = 0f;

    [Header("Debt Scaling")]
    public float day1Debt = 18f;
    public float debtGrowthMultiplier = 1.22f;

    [Header("Fare Scaling")]
    public float baseFareDay1 = 2.25f;
    public float fareRandomBonusMax = 0.75f;
    public float fareIncreasePerDay = 0.35f;

    [Header("Money Multipliers")]
    public float fareMultiplier = 1f;

    [Header("Runtime")]
    public int currentDay;
    public float currentCash;
    public float currentDebtGoal;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        currentDay = startingDay;
        currentCash = startingCash;
        currentDebtGoal = GetDebtForDay(currentDay);
    }

    public void StartNextDay()
    {
        currentDay++;
        currentDebtGoal = GetDebtForDay(currentDay);
    }

    public void StartSpecificDay(int day)
    {
        currentDay = Mathf.Max(1, day);
        currentDebtGoal = GetDebtForDay(currentDay);
    }

    public float GetDebtForDay(int day)
    {
        return Mathf.Round(day1Debt * Mathf.Pow(debtGrowthMultiplier, day - 1));
    }

    public float GetBaseFareForDay(int day)
    {
        return baseFareDay1 + ((day - 1) * fareIncreasePerDay);
    }

    public float GetRandomRideFare()
    {
        float baseFare = GetBaseFareForDay(currentDay);
        float randomBonus = Random.Range(0f, fareRandomBonusMax);
        float fare = (baseFare + randomBonus) * fareMultiplier;
        return RoundMoney(fare);
    }

    public float AddRideFare()
    {
        float fare = GetRandomRideFare();
        AddCash(fare);
        Debug.Log("[Money] Ride earned: $" + fare.ToString("0.00") + " | Cash now: $" + currentCash.ToString("0.00"));
        return fare;
    }

    public void AddCash(float amount)
    {
        currentCash = RoundMoney(currentCash + amount);
    }

    public bool SpendCash(float amount)
    {
        amount = RoundMoney(amount);

        if (currentCash < amount)
            return false;

        currentCash = RoundMoney(currentCash - amount);
        return true;
    }

    public bool HasMetDebtGoal()
    {
        return currentCash >= currentDebtGoal;
    }

    public bool TryPayDebtForDay()
    {
        if (currentCash < currentDebtGoal)
        {
            Debug.Log("[Money] Not enough cash to pay debt. Need $" + currentDebtGoal.ToString("0.00") + ", have $" + currentCash.ToString("0.00"));
            return false;
        }

        currentCash = RoundMoney(currentCash - currentDebtGoal);
        Debug.Log("[Money] Paid debt: $" + currentDebtGoal.ToString("0.00") + " | Remaining cash: $" + currentCash.ToString("0.00"));
        return true;
    }

    public int GetScaledUpgradePrice(int basePrice)
    {
        return Mathf.RoundToInt(basePrice * Mathf.Pow(1.18f, currentDay - 1));
    }

    public int GetScaledUpgradePriceForDay(int basePrice, int day)
    {
        return Mathf.RoundToInt(basePrice * Mathf.Pow(1.18f, day - 1));
    }

    public void AddFareMultiplier(float amount)
    {
        fareMultiplier += amount;
        fareMultiplier = Mathf.Max(0.1f, fareMultiplier);
        Debug.Log("[Money] Fare multiplier now: " + fareMultiplier.ToString("0.00") + "x");
    }

    public void SetFareMultiplier(float amount)
    {
        fareMultiplier = Mathf.Max(0.1f, amount);
        Debug.Log("[Money] Fare multiplier set to: " + fareMultiplier.ToString("0.00") + "x");
    }

    public static float RoundMoney(float value)
    {
        return Mathf.Round(value * 100f) / 100f;
    }
}