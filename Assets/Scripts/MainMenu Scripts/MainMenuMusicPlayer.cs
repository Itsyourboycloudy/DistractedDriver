using UnityEngine;

public class MainMenuMusicPlayer : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioClip menuLoop;

    void Start()
    {
        PlayMenuMusic();
    }

    public void PlayMenuMusic()
    {
        if (musicSource == null || menuLoop == null) return;

        musicSource.spatialBlend = 0f;
        musicSource.volume = PlayerPrefs.GetFloat("Options_MusicVolume", 1f);
        if (musicSource.outputAudioMixerGroup != null)
            musicSource.outputAudioMixerGroup.audioMixer.SetFloat("MasterVolume",
                Mathf.Log10(Mathf.Max(0.0001f, PlayerPrefs.GetFloat("Options_MasterVolume", 1f))) * 20f);

        if (musicSource.clip == menuLoop && musicSource.isPlaying)
            return;

        musicSource.clip = menuLoop;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMenuMusic()
    {
        if (musicSource == null) return;

        musicSource.Stop();
    }
}
