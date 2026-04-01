using UnityEngine;

public class ShopMusicPlayer : MonoBehaviour
{
    public static ShopMusicPlayer Instance;

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioClip shopLoop;

    void Awake()
    {
        Instance = this;
    }

    public void PlayShopMusic()
    {
        if (musicSource == null || shopLoop == null) return;

        if (musicSource.clip == shopLoop && musicSource.isPlaying)
            return;

        musicSource.Stop();
        musicSource.clip = shopLoop;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopShopMusic()
    {
        if (musicSource == null) return;

        musicSource.Stop();
        musicSource.clip = null;
    }
}