using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimeStopBubbleOverlay : MonoBehaviour
{
    [Header("Refs")]
    public Camera targetCamera;
    public Transform playerTarget;
    public Image overlayImage;

    [Header("Animation")]
    public float expandDuration = 0.35f;
    public float collapseDuration = 0.45f;
    public float maxRadius = 1.35f;
    public float edgeSoftness = 0.08f;

    private Material runtimeMat;
    private Coroutine currentRoutine;

    void Awake()
    {
        if (overlayImage != null && overlayImage.material != null)
        {
            runtimeMat = new Material(overlayImage.material);
            overlayImage.material = runtimeMat;
        }

        if (overlayImage != null)
            overlayImage.gameObject.SetActive(false);
    }

    public void PlayExpand()
    {
        if (runtimeMat == null || overlayImage == null || targetCamera == null || playerTarget == null)
            return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(AnimateBubble(0f, maxRadius, expandDuration, true));
    }

    public void PlayCollapse()
    {
        if (runtimeMat == null || overlayImage == null || targetCamera == null || playerTarget == null)
            return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(AnimateBubble(maxRadius, 0f, collapseDuration, false));
    }

    IEnumerator AnimateBubble(float fromRadius, float toRadius, float duration, bool stayVisibleAfter)
    {
        overlayImage.gameObject.SetActive(true);
        runtimeMat.SetFloat("_EdgeSoftness", edgeSoftness);

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duration);

            Vector3 vp = targetCamera.WorldToViewportPoint(playerTarget.position);
            runtimeMat.SetVector("_Center", new Vector4(vp.x, vp.y, 0f, 0f));
            runtimeMat.SetFloat("_Radius", Mathf.Lerp(fromRadius, toRadius, k));

            yield return null;
        }

        Vector3 finalVp = targetCamera.WorldToViewportPoint(playerTarget.position);
        runtimeMat.SetVector("_Center", new Vector4(finalVp.x, finalVp.y, 0f, 0f));
        runtimeMat.SetFloat("_Radius", toRadius);

        if (!stayVisibleAfter)
            overlayImage.gameObject.SetActive(false);

        currentRoutine = null;
    }

    public void ForceFullOn()
    {
        if (runtimeMat == null || overlayImage == null || targetCamera == null || playerTarget == null)
            return;

        Vector3 vp = targetCamera.WorldToViewportPoint(playerTarget.position);
        runtimeMat.SetVector("_Center", new Vector4(vp.x, vp.y, 0f, 0f));
        runtimeMat.SetFloat("_Radius", maxRadius);
        runtimeMat.SetFloat("_EdgeSoftness", edgeSoftness);

        overlayImage.gameObject.SetActive(true);
    }

    public void ForceOff()
    {
        if (runtimeMat != null)
            runtimeMat.SetFloat("_Radius", 0f);

        if (overlayImage != null)
            overlayImage.gameObject.SetActive(false);
    }
}