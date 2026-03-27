using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DopamineManager : MonoBehaviour
{
    public static DopamineManager Instance { get; private set; }

    [Header("Dopamine Settings")]
    public float maxDopamine = 100f;
    public float startDopamine = 0f;
    public float decayPerSecond = 5f;
    public float gainPerSecond = 10f;

    [Header("Phone Game Multiplier")]
    public float phoneGameMultiplier = 1f;
    public float multiplierPerSwatter = 0.25f;
    public float maxPhoneMultiplier = 5f;

    [Header("HUD UI")]
    public Slider barSlider;
    public TextMeshProUGUI percentText;
    public Image barFillImage;
    public Image portraitImage;

    [Header("Bar Colors")]
    public Color calmColor = new Color(0.3f, 0.7f, 1f);
    public Color aggressiveColor = new Color(1f, 0.2f, 0.2f);

    [Header("Portrait Sprites (4 Stages)")]
    public Sprite stage4Sprite;
    public Sprite stage3Sprite;
    public Sprite stage2Sprite;
    public Sprite stage1Sprite;

    private float currentDopamine;
    private bool isPlayingOnPhone = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        currentDopamine = Mathf.Clamp(startDopamine, 0f, maxDopamine);
        phoneGameMultiplier = 1f;
        UpdateUI();

        Debug.Log("[Dopamine] Start current = " + currentDopamine);
    }

    void Update()
    {
        float dt = Time.deltaTime;

        float normalized = currentDopamine / maxDopamine;
        float dynamicDecay = Mathf.Lerp(decayPerSecond * 0.3f, decayPerSecond, normalized);
        currentDopamine -= dynamicDecay * dt;

        if (isPlayingOnPhone)
        {
            currentDopamine += (gainPerSecond * phoneGameMultiplier) * dt;
        }

        currentDopamine = Mathf.Clamp(currentDopamine, 0f, maxDopamine);
        UpdateUI();
    }

    void UpdateUI()
    {
        float normalized = currentDopamine / maxDopamine;

        if (barSlider != null)
            barSlider.value = normalized;

        if (percentText != null)
        {
            int percent = Mathf.RoundToInt(normalized * 100f);
            percentText.text = percent + "%";
        }

        if (barFillImage != null)
            barFillImage.color = Color.Lerp(calmColor, aggressiveColor, normalized);

        UpdatePortrait(normalized);
    }

    void UpdatePortrait(float normalized)
    {
        if (portraitImage == null) return;

        if (normalized > 0.75f && stage4Sprite != null)
            portraitImage.sprite = stage4Sprite;
        else if (normalized > 0.50f && stage3Sprite != null)
            portraitImage.sprite = stage3Sprite;
        else if (normalized > 0.25f && stage2Sprite != null)
            portraitImage.sprite = stage2Sprite;
        else if (stage1Sprite != null)
            portraitImage.sprite = stage1Sprite;
    }

    public void AddDopamine(float amount)
    {
        currentDopamine = Mathf.Clamp(currentDopamine + amount, 0f, maxDopamine);
        UpdateUI();
        Debug.Log("[Dopamine] AddDopamine -> " + currentDopamine);
    }

    public void SetPlayingOnPhone(bool value)
    {
        isPlayingOnPhone = value;
        Debug.Log("[Dopamine] SetPlayingOnPhone = " + value);
    }

    public void ResetPhoneGameMultiplier()
    {
        phoneGameMultiplier = 1f;
        Debug.Log("[Dopamine] Multiplier reset to " + phoneGameMultiplier + "x");
    }

    public void IncreasePhoneGameMultiplier()
    {
        phoneGameMultiplier += multiplierPerSwatter;
        phoneGameMultiplier = Mathf.Clamp(phoneGameMultiplier, 1f, maxPhoneMultiplier);
        Debug.Log("[Dopamine] Multiplier increased to " + phoneGameMultiplier + "x");
    }

    public float GetPhoneGameMultiplier()
    {
        return phoneGameMultiplier;
    }

    public float GetSpeedMultiplier()
    {
        float t = currentDopamine / maxDopamine;
        return Mathf.Lerp(0.6f, 1.5f, t);
    }
}