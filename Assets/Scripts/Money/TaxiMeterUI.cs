using TMPro;
using UnityEngine;

public class TaxiMeterUI : MonoBehaviour
{
    [Header("Text")]
    public TMP_Text goalText;
    public TMP_Text dayText;

    [Header("Colors")]
    public Color goalNotMetColor = Color.red;
    public Color goalMetColor = Color.green;

    [Header("Meter Emission")]
    public Renderer meterRenderer;
    public string emissionColorProperty = "_EmissionColor";
    public Color debtNotMetEmission = new Color(0.25f, 0.03f, 0.03f); // maroon-ish red
    public Color debtMetEmission = new Color(0.03f, 0.20f, 0.08f);    // dark green
    [Range(0f, 10f)] public float emissionIntensity = 2.0f;

    private Material meterMaterialInstance;

    private void Start()
    {
        if (meterRenderer != null)
        {
            meterMaterialInstance = meterRenderer.material;
            meterMaterialInstance.EnableKeyword("_EMISSION");
        }
    }

    private void Update()
    {
        if (MoneyManager.Instance == null)
            return;

        float cash = MoneyManager.Instance.currentCash;
        float goal = MoneyManager.Instance.currentDebtGoal;
        int day = MoneyManager.Instance.currentDay;
        bool metGoal = cash >= goal;

        if (goalText != null)
        {
            goalText.text = "DUE $" + goal.ToString("0.00");
            goalText.color = metGoal ? goalMetColor : goalNotMetColor;
        }

        if (dayText != null)
        {
            dayText.text = "DAY " + day;
            dayText.color = metGoal ? goalMetColor : goalNotMetColor;
        }

        UpdateEmission(metGoal);
    }

    private void UpdateEmission(bool metGoal)
    {
        if (meterMaterialInstance == null)
            return;

        Color baseColor = metGoal ? debtMetEmission : debtNotMetEmission;
        Color finalEmission = baseColor * emissionIntensity;

        if (meterMaterialInstance.HasProperty(emissionColorProperty))
        {
            meterMaterialInstance.SetColor(emissionColorProperty, finalEmission);
        }
    }
}