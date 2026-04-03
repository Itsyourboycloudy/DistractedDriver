using TMPro;
using UnityEngine;

public class TaxiMeterUI : MonoBehaviour
{
    [Header("Text")]
    public TMP_Text cashText;
    public TMP_Text goalText;
    public TMP_Text dayText;
    public TMP_Text combinedText;

    [Header("Colors")]
    public Color cashColor = Color.green;
    public Color goalNotMetColor = Color.red;
    public Color goalMetColor = Color.green;

    private void Update()
    {
        if (MoneyManager.Instance == null)
            return;

        float cash = MoneyManager.Instance.currentCash;
        float goal = MoneyManager.Instance.currentDebtGoal;
        int day = MoneyManager.Instance.currentDay;
        bool metGoal = MoneyManager.Instance.HasMetDebtGoal();

        if (cashText != null)
        {
            cashText.text = "$" + cash.ToString("0.00");
            cashText.color = cashColor;
        }

        if (goalText != null)
        {
            goalText.text = "$" + goal.ToString("0.00");
            goalText.color = metGoal ? goalMetColor : goalNotMetColor;
        }

        if (dayText != null)
        {
            dayText.text = "Day " + day;
        }

        if (combinedText != null)
        {
            combinedText.text = "$" + cash.ToString("0.00") + " / $" + goal.ToString("0.00");
            combinedText.color = metGoal ? goalMetColor : goalNotMetColor;
        }
    }
}