using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class TimeStopManager : MonoBehaviour
{
    public static TimeStopManager Instance { get; private set; }

    [Header("Use")]
    public KeyCode activateKey = KeyCode.E;
    public float stopDuration = 10f;
    public bool oneUsePerDay = true;

    [Header("Visual Effect")]
    public GameObject blackWhiteEffectObject;

    [Header("Audio")]
    public AudioSource activationSfxSource;
    public AudioClip activationClip;

    public AudioSource timeStopMusicSource;
    public AudioClip timeStopLoopClip;

    [Tooltip("Normal gameplay snapshot")]
    public AudioMixerSnapshot normalSnapshot;

    [Tooltip("Snapshot where almost everything is muted except timestop music")]
    public AudioMixerSnapshot timeStopSnapshot;

    [Header("Audio Timing")]
    public float snapshotDelayAfterActivation = 0.08f;
    public float musicStartDelay = 0.10f;

    [Header("State")]
    public bool isTimeStopped = false;
    public bool canUseTimeStop = true;

    private Coroutine stopRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(activateKey))
            TryActivateTimeStop();
    }

    public void TryActivateTimeStop()
    {
        if (isTimeStopped) return;
        if (oneUsePerDay && !canUseTimeStop) return;

        if (PlayerUpgradeState.Instance == null) return;
        if (!PlayerUpgradeState.Instance.hasTimeStop) return;

        if (stopRoutine != null)
            StopCoroutine(stopRoutine);

        stopRoutine = StartCoroutine(TimeStopRoutine());
    }

    IEnumerator TimeStopRoutine()
    {
        isTimeStopped = true;

        if (oneUsePerDay)
            canUseTimeStop = false;

        Debug.Log("[TimeStop] TIME STOP STARTED");

        if (blackWhiteEffectObject != null)
            blackWhiteEffectObject.SetActive(true);

        // play activation sound once
        if (activationSfxSource != null && activationClip != null)
            activationSfxSource.PlayOneShot(activationClip);

        // let the activation sound breathe for a moment
        if (snapshotDelayAfterActivation > 0f)
            yield return new WaitForSecondsRealtime(snapshotDelayAfterActivation);

        // swap mixer
        if (timeStopSnapshot != null)
            timeStopSnapshot.TransitionTo(0.03f);

        // slight delay before song
        if (musicStartDelay > 0f)
            yield return new WaitForSecondsRealtime(musicStartDelay);

        // play looping song
        if (timeStopMusicSource != null && timeStopLoopClip != null)
        {
            timeStopMusicSource.Stop();
            timeStopMusicSource.clip = timeStopLoopClip;
            timeStopMusicSource.loop = true;
            timeStopMusicSource.Play();
        }

        PauseSystems(true);

        float timer = 0f;
        while (timer < stopDuration)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        PauseSystems(false);

        // stop song when time stop ends
        if (timeStopMusicSource != null)
            timeStopMusicSource.Stop();

        if (normalSnapshot != null)
            normalSnapshot.TransitionTo(0.08f);

        if (blackWhiteEffectObject != null)
            blackWhiteEffectObject.SetActive(false);

        isTimeStopped = false;
        stopRoutine = null;

        Debug.Log("[TimeStop] TIME STOP ENDED");
    }

    void PauseSystems(bool paused)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SetTimeStopPaused(paused);

        if (DayNightCycle.Instance != null)
            DayNightCycle.Instance.SetTimeStopPaused(paused);
    }

    public void ResetTimeStopUse()
    {
        canUseTimeStop = true;
    }
}