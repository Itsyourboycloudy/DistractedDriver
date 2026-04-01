using System.Collections;
using UnityEngine;

public class TimeStopBubble3D : MonoBehaviour
{
    [Header("Refs")]
    public Transform followTarget;
    public Transform bubbleVisual;

    [Header("Scale")]
    public float startScale = 0.1f;
    public float endScale = 80f;
    public float expandDuration = 0.35f;
    public float collapseDuration = 0.35f;

    private Coroutine currentRoutine;

    void Awake()
    {
        if (bubbleVisual != null)
        {
            bubbleVisual.gameObject.SetActive(false);
            bubbleVisual.localScale = Vector3.one * startScale;
        }
    }

    void LateUpdate()
    {
        if (followTarget != null)
            transform.position = followTarget.position;
    }

    public void PlayExpand()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(AnimateScale(startScale, endScale, expandDuration, true));
    }

    public void PlayCollapse()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(AnimateScale(endScale, startScale, collapseDuration, false));
    }

    IEnumerator AnimateScale(float from, float to, float duration, bool stayActive)
    {
        if (bubbleVisual == null)
            yield break;

        bubbleVisual.gameObject.SetActive(true);

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duration);

            float scale = Mathf.Lerp(from, to, k);
            bubbleVisual.localScale = Vector3.one * scale;

            yield return null;
        }

        bubbleVisual.localScale = Vector3.one * to;

        if (!stayActive)
            bubbleVisual.gameObject.SetActive(false);

        currentRoutine = null;
    }

    public void ForceOff()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        if (bubbleVisual != null)
        {
            bubbleVisual.localScale = Vector3.one * startScale;
            bubbleVisual.gameObject.SetActive(false);
        }
    }
}