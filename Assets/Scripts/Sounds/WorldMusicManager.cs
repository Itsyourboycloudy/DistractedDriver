using System.Collections;
using UnityEngine;

public class WorldMusicManager : MonoBehaviour
{
    public static WorldMusicManager Instance { get; private set; }

    [Header("Music Stems")]
    public AudioSource[] stemSources;

    [Header("Fade")]
    public float stopFadeDuration = 1f;
    public float phoneFadeDuration = 0.5f;
    [Range(0f, 1f)] public float worldVolumeWhenPhoneOpen = 0.2f;

    private Coroutine fadeRoutine;
    private Coroutine phoneMixRoutine;
    private float[] defaultVolumes;
    private int activePhoneMusicCount = 0;

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
        defaultVolumes = new float[stemSources.Length];

        for (int i = 0; i < stemSources.Length; i++)
        {
            if (stemSources[i] != null)
                defaultVolumes[i] = stemSources[i].volume;
            else
                defaultVolumes[i] = 1f;
        }

        StartAllMusicSynced();
    }

    public void StartAllMusicSynced()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        if (phoneMixRoutine != null)
            StopCoroutine(phoneMixRoutine);

        activePhoneMusicCount = 0;

        double startTime = AudioSettings.dspTime + 0.2;

        for (int i = 0; i < stemSources.Length; i++)
        {
            AudioSource source = stemSources[i];
            if (source == null || source.clip == null)
                continue;

            source.Stop();
            source.volume = defaultVolumes[i];
            source.loop = true;
            source.PlayScheduled(startTime);
        }
    }

    public void StopAllMusicImmediate()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        if (phoneMixRoutine != null)
            StopCoroutine(phoneMixRoutine);

        activePhoneMusicCount = 0;

        for (int i = 0; i < stemSources.Length; i++)
        {
            AudioSource source = stemSources[i];
            if (source == null)
                continue;

            source.Stop();
            source.volume = defaultVolumes[i];
        }
    }

    public void FadeOutAndStopMusic()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        if (phoneMixRoutine != null)
            StopCoroutine(phoneMixRoutine);

        activePhoneMusicCount = 0;
        fadeRoutine = StartCoroutine(FadeOutAndStopCoroutine());
    }

    public void NotifyPhoneMusicStarted()
    {
        activePhoneMusicCount++;
        StartPhoneBlendRoutine(true);
    }

    public void NotifyPhoneMusicStopped()
    {
        activePhoneMusicCount--;
        if (activePhoneMusicCount < 0)
            activePhoneMusicCount = 0;

        if (activePhoneMusicCount == 0)
            StartPhoneBlendRoutine(false);
    }

    private void StartPhoneBlendRoutine(bool phoneOpen)
    {
        if (phoneMixRoutine != null)
            StopCoroutine(phoneMixRoutine);

        phoneMixRoutine = StartCoroutine(FadeWorldForPhone(phoneOpen));
    }

    private IEnumerator FadeWorldForPhone(bool phoneOpen)
    {
        float targetMultiplier = phoneOpen ? worldVolumeWhenPhoneOpen : 1f;
        float[] startVolumes = new float[stemSources.Length];
        float[] targetVolumes = new float[stemSources.Length];

        for (int i = 0; i < stemSources.Length; i++)
        {
            if (stemSources[i] != null)
            {
                startVolumes[i] = stemSources[i].volume;
                targetVolumes[i] = defaultVolumes[i] * targetMultiplier;
            }
        }

        float time = 0f;

        while (time < phoneFadeDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / phoneFadeDuration);

            for (int i = 0; i < stemSources.Length; i++)
            {
                if (stemSources[i] != null)
                    stemSources[i].volume = Mathf.Lerp(startVolumes[i], targetVolumes[i], t);
            }

            yield return null;
        }

        for (int i = 0; i < stemSources.Length; i++)
        {
            if (stemSources[i] != null)
                stemSources[i].volume = targetVolumes[i];
        }

        phoneMixRoutine = null;
    }

    private IEnumerator FadeOutAndStopCoroutine()
    {
        float[] startVolumes = new float[stemSources.Length];

        for (int i = 0; i < stemSources.Length; i++)
        {
            if (stemSources[i] != null)
                startVolumes[i] = stemSources[i].volume;
        }

        float time = 0f;

        while (time < stopFadeDuration)
        {
            time += Time.deltaTime;
            float t = time / stopFadeDuration;

            for (int i = 0; i < stemSources.Length; i++)
            {
                if (stemSources[i] != null)
                    stemSources[i].volume = Mathf.Lerp(startVolumes[i], 0f, t);
            }

            yield return null;
        }

        for (int i = 0; i < stemSources.Length; i++)
        {
            AudioSource source = stemSources[i];
            if (source == null)
                continue;

            source.Stop();
            source.volume = defaultVolumes[i];
        }

        fadeRoutine = null;
    }
}