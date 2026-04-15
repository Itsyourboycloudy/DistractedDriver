using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;

public class MusicZoneTrigger : MonoBehaviour
{
    [Header("Player")]
    public string playerTag = "Player";

    [Header("Music Snapshot")]
    public AudioMixerSnapshot zoneSnapshot;
    public float snapshotTransitionTime = 1.5f;

    [Header("Optional Snapshot On Exit")]
    public AudioMixerSnapshot exitSnapshot;
    public float exitTransitionTime = 1.5f;

    [Header("Optional Local Ambience")]
    public AudioSource zoneAudio;
    public float fadeDuration = 1.5f;
    public float targetVolume = 0.45f;

    [Header("UI")]
    public TextMeshProUGUI zoneText;
    public CanvasGroup zoneTextCanvasGroup;
    public string zoneMessage = "Entering Zone";
    public float textShowTime = 2f;
    public float textFadeDuration = 0.75f;

    [Header("Optional Exit UI Message")]
    public bool showExitZoneMessage = false;
    public string exitZoneMessage = "Entering Parent Zone";

    private Coroutine fadeRoutine;
    private Coroutine textRoutine;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (zoneSnapshot != null)
            zoneSnapshot.TransitionTo(snapshotTransitionTime);

        if (zoneAudio != null)
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeAudio(zoneAudio.volume, targetVolume));
        }

        ShowMessage(zoneMessage);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (exitSnapshot != null)
            exitSnapshot.TransitionTo(exitTransitionTime);

        if (zoneAudio != null)
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeAudio(zoneAudio.volume, 0f));
        }

        if (showExitZoneMessage)
            ShowMessage(exitZoneMessage);
    }

    private void ShowMessage(string message)
    {
        if (zoneText == null || zoneTextCanvasGroup == null)
            return;

        if (textRoutine != null)
            StopCoroutine(textRoutine);

        textRoutine = StartCoroutine(ShowZoneText(message));
    }

    private IEnumerator FadeAudio(float start, float end)
    {
        if (zoneAudio == null)
            yield break;

        if (!zoneAudio.isPlaying)
            zoneAudio.Play();

        float time = 0f;
        zoneAudio.volume = start;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            zoneAudio.volume = Mathf.Lerp(start, end, time / fadeDuration);
            yield return null;
        }

        zoneAudio.volume = end;

        if (Mathf.Approximately(end, 0f))
            zoneAudio.Stop();
    }

    private IEnumerator ShowZoneText(string message)
    {
        zoneText.text = message;
        zoneTextCanvasGroup.alpha = 0f;

        float fadeInTime = 0.2f;
        float time = 0f;

        while (time < fadeInTime)
        {
            time += Time.deltaTime;
            zoneTextCanvasGroup.alpha = Mathf.Lerp(0f, 1f, time / fadeInTime);
            yield return null;
        }

        zoneTextCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(textShowTime);

        time = 0f;
        while (time < textFadeDuration)
        {
            time += Time.deltaTime;
            zoneTextCanvasGroup.alpha = Mathf.Lerp(1f, 0f, time / textFadeDuration);
            yield return null;
        }

        zoneTextCanvasGroup.alpha = 0f;
    }
}