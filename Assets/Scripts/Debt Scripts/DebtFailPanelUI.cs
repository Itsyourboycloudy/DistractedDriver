using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class DebtFailedPanelUI : MonoBehaviour
{
    [Header("Main")]
    public GameObject root;
    public CanvasGroup canvasGroup;

    [Header("UI")]
    public TMP_Text failText;
    public Button tryAgainButton;
    public Button quitButton;

    [Header("Fade")]
    public float fadeDuration = 1f;

    [Header("Scene")]
    public string mainMenuSceneName = "MainMenu";

    private bool showing = false;

    private void Start()
    {
        if (root != null)
            root.SetActive(false);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void Show()
    {
        if (showing)
            return;

        showing = true;

        if (root != null)
            root.SetActive(true);

        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float t = 0f;

        if (canvasGroup == null)
        {
            yield break;
        }

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        Time.timeScale = 0f;
    }

    public void OnTryAgainPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnQuitPressed()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Application.Quit();
            Debug.Log("Quit Game pressed.");
        }
    }
}