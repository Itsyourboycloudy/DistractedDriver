using UnityEngine;
using UnityEngine.Audio;

public class DopamineAudioDriver : MonoBehaviour
{
    [Header("References")]
    public AudioMixer masterMixer;
    public AudioMixer worldMusicMixer;
    public AudioSource[] musicSources;
    public DopamineManager dopamineManager;

    [Header("Exposed Parameters")]
    public string masterDopamineParameter = "MasterDopamineLowpassCutoff";
    public string worldMusicDopamineParameter = "MusicLowpassCutoff";

    [Header("Lowpass Range")]
    public float lowDopamineCutoff = 1200f;
    public float highDopamineCutoff = 22000f;
    public float cutoffLerpSpeed = 5f;

    [Header("Music Speed / Pitch")]
    public float minPitch = 1f;
    public float maxPitch = 1.15f;
    public float pitchLerpSpeed = 4f;

    private float currentMasterCutoff;
    private float currentWorldMusicCutoff;

    private void Start()
    {
        currentMasterCutoff = highDopamineCutoff;
        currentWorldMusicCutoff = highDopamineCutoff;

        if (masterMixer != null)
            masterMixer.SetFloat(masterDopamineParameter, currentMasterCutoff);

        if (worldMusicMixer != null)
            worldMusicMixer.SetFloat(worldMusicDopamineParameter, currentWorldMusicCutoff);
    }

    private void Update()
    {
        if (dopamineManager == null)
            return;

        float dopamine01 = dopamineManager.GetDopamineNormalized();

        UpdateMixerLowpass(dopamine01);
        UpdateMusicPitch(dopamine01);
    }

    private void UpdateMixerLowpass(float dopamine01)
    {
        float targetCutoff = Mathf.Lerp(lowDopamineCutoff, highDopamineCutoff, dopamine01);

        if (masterMixer != null)
        {
            currentMasterCutoff = Mathf.Lerp(
                currentMasterCutoff,
                targetCutoff,
                Time.unscaledDeltaTime * cutoffLerpSpeed
            );

            masterMixer.SetFloat(masterDopamineParameter, currentMasterCutoff);
        }

        if (worldMusicMixer != null)
        {
            currentWorldMusicCutoff = Mathf.Lerp(
                currentWorldMusicCutoff,
                targetCutoff,
                Time.unscaledDeltaTime * cutoffLerpSpeed
            );

            worldMusicMixer.SetFloat(worldMusicDopamineParameter, currentWorldMusicCutoff);
        }
    }

    private void UpdateMusicPitch(float dopamine01)
    {
        if (musicSources == null || musicSources.Length == 0)
            return;

        float targetPitch = Mathf.Lerp(minPitch, maxPitch, dopamine01);

        for (int i = 0; i < musicSources.Length; i++)
        {
            if (musicSources[i] == null)
                continue;

            musicSources[i].pitch = Mathf.Lerp(
                musicSources[i].pitch,
                targetPitch,
                Time.unscaledDeltaTime * pitchLerpSpeed
            );
        }
    }
}