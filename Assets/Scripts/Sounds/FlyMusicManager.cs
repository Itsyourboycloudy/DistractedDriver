using UnityEngine;
using System.Collections;

public class FlyMusicManager : MonoBehaviour
{
    public static FlyMusicManager Instance { get; private set; }

    [Header("Audio Source")]
    public AudioSource musicSource;

    [Header("Clips")]
    public AudioClip fly1Loop;
    public AudioClip fly2Transition;
    public AudioClip fly3Loop;

    private bool transitionRequested = false;
    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    public void PlayFly1()
    {
        if (musicSource == null || fly1Loop == null) return;

        StopAllCoroutines();
        transitionRequested = false;
        isTransitioning = false;

        musicSource.clip = fly1Loop;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void ReturnToFly1()
    {
        PlayFly1();
    }

    public void RequestTransitionToGameplay()
    {
        if (musicSource == null || fly2Transition == null || fly3Loop == null) return;
        if (transitionRequested || isTransitioning) return;
        if (musicSource.clip != fly1Loop) return;

        transitionRequested = true;
        StartCoroutine(FinishFly1ThenGoToFly2ThenFly3());
    }

    private IEnumerator FinishFly1ThenGoToFly2ThenFly3()
    {
        isTransitioning = true;

        while (musicSource.clip == fly1Loop)
        {
            float timeLeft = musicSource.clip.length - musicSource.time;

            if (timeLeft <= 0.05f)
                break;

            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = fly2Transition;
        musicSource.loop = false;
        musicSource.Play();

        yield return new WaitForSeconds(fly2Transition.length);

        musicSource.clip = fly3Loop;
        musicSource.loop = true;
        musicSource.Play();

        transitionRequested = false;
        isTransitioning = false;
    }

    public void StopMusic()
    {
        if (musicSource == null) return;

        StopAllCoroutines();
        musicSource.Stop();
        transitionRequested = false;
        isTransitioning = false;
    }
}