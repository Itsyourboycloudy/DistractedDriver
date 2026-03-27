using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.VFX;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle Instance { get; private set; }

    [Header("Day Length")]
    public float dayLengthSeconds = 960f; // 16 minutes

    [Header("Sun Orbit")]
    public Transform sunPivot;
    public float sunriseAngle = 15f;
    public float sunsetAngle = 165f;
    public float compassYaw = 0f;

    [Header("Sun Light")]
    public Light sunLight;
    public Gradient sunColorOverDay;
    public AnimationCurve sunIntensityOverDay = AnimationCurve.Linear(0f, 0.3f, 1f, 1f);

    [Header("Pre-Fade Day End FX")]
    public VisualEffect inCarWinVFX;
    public AudioSource inCarWinAudioSource;
    public float preFadeDelay = 2f;

    [Header("Day Over UI")]
    public GameObject dayOverPanel;
    public Image dayOverBackground;
    public TMP_Text dayOverText;
    public float fadeDuration = 3f;

    [Header("Day Count")]
    public int currentDay = 1;

    private float dayTimer = 0f;
    private bool dayEnded = false;
    private bool isFading = false;
    private float fadeTimer = 0f;
    private bool endingSequenceStarted = false;

    public float DayProgress01 => Mathf.Clamp01(dayTimer / dayLengthSeconds);
    public bool DayEnded => dayEnded;

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
        if (dayOverPanel != null)
            dayOverPanel.SetActive(false);

        SetDayOverAlpha(0f);
        ApplySun(0f);

        // Make sure VFX starts disabled
        if (inCarWinVFX != null)
            inCarWinVFX.gameObject.SetActive(false);

        // Make sure sound does not auto-play
        if (inCarWinAudioSource != null)
            inCarWinAudioSource.Stop();
    }

    void Update()
    {
        if (!dayEnded)
        {
            dayTimer += Time.deltaTime;

            if (dayTimer >= dayLengthSeconds && !endingSequenceStarted)
            {
                dayTimer = dayLengthSeconds;
                StartCoroutine(EndDaySequence());
            }

            ApplySun(DayProgress01);
        }
        else if (isFading)
        {
            fadeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(fadeTimer / fadeDuration);

            SetDayOverAlpha(t);

            if (t >= 1f)
            {
                isFading = false;
                Debug.Log("[DayNight] Fade complete.");
            }
        }
    }

    void ApplySun(float t)
    {
        if (sunPivot != null)
        {
            float angle = Mathf.Lerp(sunriseAngle, sunsetAngle, t);
            sunPivot.rotation = Quaternion.Euler(angle, compassYaw, 0f);
        }

        if (sunLight != null)
        {
            if (sunColorOverDay != null)
                sunLight.color = sunColorOverDay.Evaluate(t);

            if (sunIntensityOverDay != null)
                sunLight.intensity = sunIntensityOverDay.Evaluate(t);
        }
    }

    IEnumerator EndDaySequence()
    {
        if (endingSequenceStarted)
            yield break;

        endingSequenceStarted = true;
        dayEnded = true;

        Debug.Log("[DayNight] Day ended. Playing in-car FX.");

        // Enable + restart + play VFX
        if (inCarWinVFX != null)
        {
            inCarWinVFX.gameObject.SetActive(true);
            inCarWinVFX.Reinit();
            inCarWinVFX.Play();
        }

        // Play sound through your mixer-routed AudioSource
        if (inCarWinAudioSource != null)
        {
            inCarWinAudioSource.Stop();
            inCarWinAudioSource.Play();
        }

        // Let the player see/hear it before fade
        yield return new WaitForSeconds(preFadeDelay);

        if (dayOverPanel != null)
            dayOverPanel.SetActive(true);

        if (dayOverText != null)
            dayOverText.text = "Day " + currentDay + " Over";

        fadeTimer = 0f;
        isFading = true;
        SetDayOverAlpha(0f);

        Debug.Log("[DayNight] Starting fade to black.");
    }

    void SetDayOverAlpha(float alpha)
    {
        if (dayOverBackground != null)
        {
            Color bg = dayOverBackground.color;
            bg.a = alpha;
            dayOverBackground.color = bg;
        }

        if (dayOverText != null)
        {
            Color txt = dayOverText.color;
            txt.a = alpha;
            dayOverText.color = txt;
        }
    }

    public void ResetDay()
    {
        dayTimer = 0f;
        dayEnded = false;
        isFading = false;
        fadeTimer = 0f;
        endingSequenceStarted = false;

        currentDay++;

        if (dayOverPanel != null)
            dayOverPanel.SetActive(false);

        SetDayOverAlpha(0f);
        ApplySun(0f);

        // Stop + disable VFX so it does not hang around
        if (inCarWinVFX != null)
        {
            inCarWinVFX.Stop();
            inCarWinVFX.gameObject.SetActive(false);
        }

        // Stop audio too
        if (inCarWinAudioSource != null)
            inCarWinAudioSource.Stop();

        Debug.Log("[DayNight] Day reset. Current day is now " + currentDay);
    }
}