using UnityEngine;
using UnityEngine.Audio;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance { get; private set; }

    [Header("Clips")]
    public AudioClip phoneClick;
    public AudioClip rideAccept;

    [Header("Audio")]
    public AudioSource uiSource;
    public AudioMixerGroup uiMixerGroup;

    [Header("Pitch")]
    [Range(0.5f, 1.5f)] public float normalPitch = 1f;
    [Range(0.5f, 1.5f)] public float backPitch = 0.85f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (uiSource == null)
        {
            uiSource = gameObject.AddComponent<AudioSource>();
            uiSource.playOnAwake = false;
        }

        if (uiMixerGroup != null)
            uiSource.outputAudioMixerGroup = uiMixerGroup;
    }

    public void PlayClick()
    {
        PlayClipWithPitch(phoneClick, normalPitch);
    }

    public void PlayBackClick()
    {
        PlayClipWithPitch(phoneClick, backPitch);
    }

    // accept sound can stay OneShot (or also use PlayClipWithPitch if you want)
    public void PlayRideAccept()
    {
        if (rideAccept == null) return;
        uiSource.pitch = 1f;
        uiSource.PlayOneShot(rideAccept);
    }

    private void PlayClipWithPitch(AudioClip clip, float pitch)
    {
        if (clip == null || uiSource == null) return;

        uiSource.Stop();        // stop previous UI click if it's still playing
        uiSource.pitch = pitch;
        uiSource.clip = clip;
        uiSource.Play();
    }
}
