using TMPro;
using UnityEngine;
using System.Collections;

public class HUDCashUI : MonoBehaviour
{
    [Header("Main Cash Text")]
    public TMP_Text cashText;
    public Color normalCashColor = Color.green;
    public Color flashCashColor = new Color(0.6f, 1f, 0.6f);

    [Header("Popup Text")]
    public TMP_Text cashPopupText;
    public Vector2 popupStartAnchoredPosition = new Vector2(0f, -28f);
    public Vector2 popupEndAnchoredPosition = new Vector2(0f, -8f);
    public float popupDuration = 0.6f;

    [Header("Bounce")]
    public RectTransform cashRect;
    public float pulseScale = 1.18f;
    public float bounceDuration = 0.18f;

    [Header("Flash")]
    public float flashDuration = 0.18f;

    private Vector3 baseScale;
    private Coroutine pulseRoutine;
    private Coroutine flashRoutine;
    private Coroutine popupRoutine;
    private RectTransform popupRect;

    private void Start()
    {
        if (cashRect == null && cashText != null)
            cashRect = cashText.rectTransform;

        if (cashRect != null)
            baseScale = cashRect.localScale;

        if (cashPopupText != null)
        {
            popupRect = cashPopupText.rectTransform;
            cashPopupText.gameObject.SetActive(false);
            popupRect.anchoredPosition = popupStartAnchoredPosition;
        }

        if (MoneyManager.Instance != null)
            MoneyManager.Instance.OnCashAdded += HandleCashAdded;
    }

    private void OnDestroy()
    {
        if (MoneyManager.Instance != null)
            MoneyManager.Instance.OnCashAdded -= HandleCashAdded;
    }

    private void Update()
    {
        if (MoneyManager.Instance == null || cashText == null)
            return;

        cashText.text = "CASH $" + MoneyManager.Instance.currentCash.ToString("0.00");
    }

    private void HandleCashAdded(float amountAdded)
    {
        if (cashText != null)
        {
            if (flashRoutine != null)
                StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(FlashCashText());
        }

        if (cashRect != null)
        {
            if (pulseRoutine != null)
                StopCoroutine(pulseRoutine);
            pulseRoutine = StartCoroutine(BounceCashText());
        }

        if (cashPopupText != null)
        {
            if (popupRoutine != null)
                StopCoroutine(popupRoutine);
            popupRoutine = StartCoroutine(ShowCashPopup(amountAdded));
        }
    }

    private IEnumerator FlashCashText()
    {
        cashText.color = flashCashColor;

        float t = 0f;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            float lerp = t / flashDuration;
            cashText.color = Color.Lerp(flashCashColor, normalCashColor, lerp);
            yield return null;
        }

        cashText.color = normalCashColor;
        flashRoutine = null;
    }

    private IEnumerator BounceCashText()
    {
        float half = bounceDuration * 0.5f;
        Vector3 bigScale = baseScale * pulseScale;

        float t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float lerp = t / half;
            cashRect.localScale = Vector3.Lerp(baseScale, bigScale, lerp);
            yield return null;
        }

        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float lerp = t / half;
            cashRect.localScale = Vector3.Lerp(bigScale, baseScale, lerp);
            yield return null;
        }

        cashRect.localScale = baseScale;
        pulseRoutine = null;
    }

    private IEnumerator ShowCashPopup(float amountAdded)
    {
        if (popupRect == null)
            yield break;

        cashPopupText.gameObject.SetActive(true);
        cashPopupText.text = "+$" + amountAdded.ToString("0.00");
        cashPopupText.color = flashCashColor;

        popupRect.anchoredPosition = popupStartAnchoredPosition;

        Color startColor = flashCashColor;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        float t = 0f;
        while (t < popupDuration)
        {
            t += Time.deltaTime;
            float lerp = t / popupDuration;

            popupRect.anchoredPosition = Vector2.Lerp(popupStartAnchoredPosition, popupEndAnchoredPosition, lerp);
            cashPopupText.color = Color.Lerp(startColor, endColor, lerp);

            yield return null;
        }

        cashPopupText.gameObject.SetActive(false);
        popupRoutine = null;
    }
}