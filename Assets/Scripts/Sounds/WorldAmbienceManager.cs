using System.Collections;
using UnityEngine;

public class WorldAmbienceManager : MonoBehaviour
{
    public static WorldAmbienceManager Instance { get; private set; }

    [Header("Ambience Sources")]
    public AudioSource[] ambienceSources;

    [Header("Fade")]
    public float fadeOutDuration = 1f;

    private Coroutine fadeRoutine;
    private float[] defaultVolumes;

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
        defaultVolumes = new float[ambienceSources.Length];

        for (int i = 0; i < ambienceSources.Length; i++)
        {
            if (ambienceSources[i] != null)
                defaultVolumes[i] = ambienceSources[i].volume;
            else
                defaultVolumes[i] = 1f;
        }
    }

    public void StopAllAmbienceImmediate()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        for (int i = 0; i < ambienceSources.Length; i++)
        {
            AudioSource source = ambienceSources[i];
            if (source == null)
                continue;

            source.Stop();
            source.volume = 0f;
        }
    }

    public void FadeOutAndStopAllAmbience()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeOutAndStopRoutine());
    }

    public void RestoreDefaultVolumes()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        for (int i = 0; i < ambienceSources.Length; i++)
        {
            AudioSource source = ambienceSources[i];
            if (source == null)
                continue;

            source.volume = defaultVolumes[i];
        }
    }

    private IEnumerator FadeOutAndStopRoutine()
    {
        float[] startVolumes = new float[ambienceSources.Length];

        for (int i = 0; i < ambienceSources.Length; i++)
        {
            if (ambienceSources[i] != null)
                startVolumes[i] = ambienceSources[i].volume;
        }

        float time = 0f;

        while (time < fadeOutDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / fadeOutDuration);

            for (int i = 0; i < ambienceSources.Length; i++)
            {
                if (ambienceSources[i] != null)
                    ambienceSources[i].volume = Mathf.Lerp(startVolumes[i], 0f, t);
            }

            yield return null;
        }

        for (int i = 0; i < ambienceSources.Length; i++)
        {
            AudioSource source = ambienceSources[i];
            if (source == null)
                continue;

            source.volume = 0f;
            source.Stop();
        }

        fadeRoutine = null;
    }
}