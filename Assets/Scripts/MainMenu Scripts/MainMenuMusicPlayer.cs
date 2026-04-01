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