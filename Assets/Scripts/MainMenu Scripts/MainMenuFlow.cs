using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuFlow : MonoBehaviour
{
    [Header("Panels")]
    public GameObject introPanel;     // Panel_IntroPhone
    public GameObject mainMenuPanel;  // Panel_MainMenu (zoomed phone UI)

    [Header("Intro Objects")]
    public GameObject flashingObj;    // MAIN MENU 1_0
    public GameObject pressObj;       // MAIN MENU 2_0

    [Header("Press Timing")]
    public AnimationClip pressClip;   // drag your Press.anim here

    [Header("Fade")]
    public Image fadeOverlay;         // full-screen black Image
    public float fadeOutTime = 0.12f;
    public float blackHoldTime = 0.08f;
    public float fadeInTime = 0.12f;

    [Header("Audio")]
    public AudioSource uiAudioSource;
    public AudioClip menuClick;

    bool hasPressed = false;

    void Start()
    {
        introPanel.SetActive(true);
        mainMenuPanel.SetActive(false);

        if (flashingObj) flashingObj.SetActive(true);
        if (pressObj) pressObj.SetActive(false);

        if (fadeOverlay != null)
        {
            var c = fadeOverlay.color;
            c.a = 0f;
            fadeOverlay.color = c;
            fadeOverlay.raycastTarget = false;
        }
    }

    public void OnScreenClicked()
    {
        if (hasPressed) return;
        hasPressed = true;

        // Play click sound
        if (uiAudioSource && menuClick)
            uiAudioSource.PlayOneShot(menuClick);

        if (flashingObj) flashingObj.SetActive(false);
        if (pressObj) pressObj.SetActive(true);

        StartCoroutine(WaitThenFadeToMenu());
    }


    IEnumerator WaitThenFadeToMenu()
    {
        // wait for the press animation to finish
        float wait = (pressClip != null) ? pressClip.length : 0.25f;
        yield return new WaitForSeconds(wait);

        // fade out
        yield return FadeAlpha(0f, 1f, fadeOutTime);

        // hold black briefly
        yield return new WaitForSeconds(blackHoldTime);

        // swap panels while black
        introPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        // fade in
        yield return FadeAlpha(1f, 0f, fadeInTime);
    }

    IEnumerator FadeAlpha(float from, float to, float time)
    {
        if (fadeOverlay == null || time <= 0f) yield break;

        float t = 0f;
        var col = fadeOverlay.color;

        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            col.a = Mathf.Lerp(from, to, t / time);
            fadeOverlay.color = col;
            yield return null;
        }

        col.a = to;
        fadeOverlay.color = col;
    }
}
