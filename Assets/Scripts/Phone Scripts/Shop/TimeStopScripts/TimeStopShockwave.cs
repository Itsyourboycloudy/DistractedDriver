using System.Collections;
using UnityEngine;

public class TimeStopShockwave : MonoBehaviour
{
    [Header("Scale")]
    public float duration = 0.35f;
    public float startScale = 0.2f;
    public float endScale = 35f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Fade")]
    public Renderer[] targetRenderers;
    public string alphaProperty = "_AlphaMultiplier";
    public float startAlpha = 1f;
    public float endAlpha = 0f;

    [Header("Spin")]
    public Vector3 rotationSpeed = new Vector3(0f, 40f, 0f);

    private Material[] runtimeMaterials;

    private void Awake()
    {
        if (targetRenderers == null || targetRenderers.Length == 0)
            targetRenderers = GetComponentsInChildren<Renderer>();

        runtimeMaterials = new Material[targetRenderers.Length];

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (targetRenderers[i] != null)
                runtimeMaterials[i] = targetRenderers[i].material;
        }
    }

    public void Play()
    {
        StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            float curved = scaleCurve.Evaluate(p);

            float scale = Mathf.Lerp(startScale, endScale, curved);
            transform.localScale = Vector3.one * scale;

            transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);

            float alpha = Mathf.Lerp(startAlpha, endAlpha, p);
            UpdateAlpha(alpha);

            yield return null;
        }

        Destroy(gameObject);
    }

    private void UpdateAlpha(float alpha)
    {
        if (runtimeMaterials == null) return;

        for (int i = 0; i < runtimeMaterials.Length; i++)
        {
            Material mat = runtimeMaterials[i];
            if (mat == null) continue;

            if (mat.HasProperty(alphaProperty))
                mat.SetFloat(alphaProperty, alpha);
        }
    }
}