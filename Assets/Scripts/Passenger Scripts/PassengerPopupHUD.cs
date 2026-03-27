using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class PassengerPopupHUD : MonoBehaviour
{
    [Header("UI")]
    public RectTransform panel;      // the popup container (recommended)
    public TMP_Text popupText;       // message text
    public Image backgroundImage;    // optional background image for color ramp
    public CanvasGroup canvasGroup;  // for fade (optional but nice)

    [Header("Timing")]
    public float showSeconds = 1.6f;
    public float slideInSeconds = 0.25f;
    public float slideOutSeconds = 0.2f;

    [Header("Motion")]
    public Vector2 hiddenOffset = new Vector2(0f, 120f);   // start above
    public float pulseScale = 1.06f;
    public float pulseSpeed = 10f; // higher = faster pulse

    [Header("Urgency Color")]
    public Color startColor = new Color(1f, 0.2f, 0.2f);   // red
    public Color endColor = new Color(1f, 0.9f, 0.2f);     // yellow

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip popupClip;

    Coroutine routine;
    Vector2 shownPos;
    Vector2 hiddenPos;

    void Awake()
    {
        if (panel == null) panel = GetComponent<RectTransform>();
        if (popupText == null) popupText = GetComponentInChildren<TMP_Text>();
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

        shownPos = panel.anchoredPosition;
        hiddenPos = shownPos + hiddenOffset;

        // start hidden but ACTIVE (so coroutines work)
        panel.anchoredPosition = hiddenPos;
        panel.localScale = Vector3.one;
        if (canvasGroup != null) canvasGroup.alpha = 0f;

        if (backgroundImage != null) backgroundImage.color = startColor;

        gameObject.SetActive(true);
        SetVisible(false);
    }

    void SetVisible(bool visible)
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = visible;
    }

    public void Show(string msg)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(ShowRoutine(msg));
    }

    IEnumerator ShowRoutine(string msg)
    {
        // Set text
        if (popupText != null) popupText.text = msg;

        // sound
        if (audioSource != null && popupClip != null)
            audioSource.PlayOneShot(popupClip);

        // prep
        SetVisible(true);

        // slide in + fade in
        yield return SlideFade(hiddenPos, shownPos, 0f, 1f, slideInSeconds);

        // hold with pulse + urgency ramp
        float t = 0f;
        while (t < showSeconds)
        {
            t += Time.deltaTime;

            // pulse
            float pulse = 1f + (Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f) * (pulseScale - 1f);
            panel.localScale = new Vector3(pulse, pulse, 1f);

            // color ramp
            float u = Mathf.Clamp01(t / showSeconds);
            Color c = Color.Lerp(startColor, endColor, u);
            if (backgroundImage != null) backgroundImage.color = c;
            if (popupText != null) popupText.color = Color.Lerp(Color.white, Color.black, u * 0.35f); // optional readability tweak

            yield return null;
        }

        // slide out + fade out
        panel.localScale = Vector3.one;
        yield return SlideFade(shownPos, hiddenPos, 1f, 0f, slideOutSeconds);

        SetVisible(false);
        routine = null;
    }

    IEnumerator SlideFade(Vector2 fromPos, Vector2 toPos, float fromA, float toA, float seconds)
    {
        float t = 0f;
        while (t < seconds)
        {
            t += Time.deltaTime;
            float u = (seconds <= 0f) ? 1f : Mathf.Clamp01(t / seconds);

            // ease-out (snappy)
            float eased = 1f - Mathf.Pow(1f - u, 3f);

            panel.anchoredPosition = Vector2.Lerp(fromPos, toPos, eased);
            if (canvasGroup != null) canvasGroup.alpha = Mathf.Lerp(fromA, toA, eased);

            yield return null;
        }

        panel.anchoredPosition = toPos;
        if (canvasGroup != null) canvasGroup.alpha = toA;
    }
}
