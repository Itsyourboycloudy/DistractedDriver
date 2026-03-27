using System.Collections;
using UnityEngine;
using TMPro;

public class CityZoneTrigger : MonoBehaviour
{
    [Header("Player Tag")]
    public string playerTag = "Player";

    [Header("Audio")]
    public AudioSource cityAudio;
    public float fadeDuration = 1.5f;
    public float targetVolume = 0.45f;

    [Header("UI")]
    public TextMeshProUGUI zoneText;
    public string zoneMessage = "Entering City Congo";
    public float textShowTime = 2.5f;

    private Coroutine fadeRoutine;
    private Coroutine textRoutine;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeAudio(0f, targetVolume));

        if (textRoutine != null) StopCoroutine(textRoutine);
        textRoutine = StartCoroutine(ShowZoneText());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeAudio(cityAudio.volume, 0f));
    }

    IEnumerator FadeAudio(float start, float end)
    {
        if (cityAudio == null) yield break;

        if (!cityAudio.isPlaying)
            cityAudio.Play();

        float time = 0f;
        cityAudio.volume = start;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            cityAudio.volume = Mathf.Lerp(start, end, time / fadeDuration);
            yield return null;
        }

        cityAudio.volume = end;

        if (Mathf.Approximately(end, 0f))
            cityAudio.Stop();
    }

    IEnumerator ShowZoneText()
    {
        if (zoneText == null) yield break;

        zoneText.text = zoneMessage;
        zoneText.gameObject.SetActive(true);

        yield return new WaitForSeconds(textShowTime);

        zoneText.text = "";
        zoneText.gameObject.SetActive(false);
    }
}