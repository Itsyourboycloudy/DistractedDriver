using UnityEngine;
using TMPro;
using System.Collections;

public class ScorePopupUI : MonoBehaviour
{
    public static ScorePopupUI Instance { get; private set; }

    [Header("UI")]
    public TMP_Text popupText;
    public CanvasGroup canvasGroup;

    [Header("Animation")]
    public float showDuration = 1f;      // how long the popup animates
    public float moveUpDistance = 50f;   // how far it floats up

    private RectTransform rectTransform;
    private Vector2 startAnchoredPos;
    private Coroutine popupRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        rectTransform = GetComponent<RectTransform>();

        if (popupText != null)
        {
            startAnchoredPos = popupText.rectTransform.anchoredPosition;
            popupText.gameObject.SetActive(false);
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    public void ShowPopup(int amount)
    {
        if (popupText == null || canvasGroup == null) return;

        // If a popup is already animating, stop it so we can restart
        if (popupRoutine != null)
            StopCoroutine(popupRoutine);

        popupRoutine = StartCoroutine(PopupRoutine(amount));
    }

    private IEnumerator PopupRoutine(int amount)
    {
        popupText.text = "+" + amount;
        popupText.gameObject.SetActive(true);

        canvasGroup.alpha = 1f;
        popupText.rectTransform.anchoredPosition = startAnchoredPos;

        float elapsed = 0f;

        Vector2 endPos = startAnchoredPos + new Vector2(0f, moveUpDistance);

        while (elapsed < showDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / showDuration;

            // Move up
            popupText.rectTransform.anchoredPosition = Vector2.Lerp(startAnchoredPos, endPos, t);
            // Fade out
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        canvasGroup.alpha = 0f;
        popupText.gameObject.SetActive(false);
        popupRoutine = null;
    }
}
