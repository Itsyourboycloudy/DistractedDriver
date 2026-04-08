using System.Collections;
using UnityEngine;

public class TimeStopVisualController : MonoBehaviour
{
    [Header("Material")]
    public Material worldEffectMaterial;

    [Header("Camera")]
    public Camera worldCamera;

    [Header("Tracked Target")]
    public Transform effectOrigin;

    [Header("Shader Property Names")]
    public string centerUVProperty = "_EffectCenterUV";
    public string radiusProperty = "_WaveRadius";
    public string widthProperty = "_WaveWidth";
    public string invertProperty = "_InvertStrength";
    public string bwProperty = "_BWBlend";
    public string contrastProperty = "_Contrast";

    [Header("Wave Settings")]
    public float waveDuration = 0.35f;
    public float waveMaxRadius = 1.4f;
    public float waveWidth = 0.08f;

    [Header("Hold / Return")]
    public float bwHoldTime = 0f;
    public float returnDuration = 0.25f;

    [Header("Look")]
    public float normalContrast = 1f;
    public float timeStopContrast = 1.08f;
    public bool flipY = false;

    private Coroutine activeRoutine;

    private void Start()
    {
        ResetEffectInstant();
    }

    public void BeginTimeStop()
    {
        if (worldEffectMaterial == null || worldCamera == null || effectOrigin == null)
            return;

        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        activeRoutine = StartCoroutine(BeginRoutine());
    }

    public void EndTimeStop()
    {
        if (worldEffectMaterial == null)
            return;

        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        activeRoutine = StartCoroutine(EndRoutine());
    }

    public void ResetEffectInstant()
    {
        if (worldEffectMaterial == null)
            return;

        worldEffectMaterial.SetFloat(radiusProperty, 0f);
        worldEffectMaterial.SetFloat(widthProperty, waveWidth);
        worldEffectMaterial.SetFloat(invertProperty, 0f);
        worldEffectMaterial.SetFloat(bwProperty, 0f);

        if (worldEffectMaterial.HasProperty(contrastProperty))
            worldEffectMaterial.SetFloat(contrastProperty, normalContrast);

        worldEffectMaterial.SetVector(centerUVProperty, new Vector4(0.5f, 0.5f, 0f, 0f));
    }

    private IEnumerator BeginRoutine()
    {
        Vector3 screenPos = worldCamera.WorldToViewportPoint(effectOrigin.position);

        float y = flipY ? 1f - screenPos.y : screenPos.y;

        worldEffectMaterial.SetVector(
            centerUVProperty,
            new Vector4(screenPos.x, y, 0f, 0f)
        );

        worldEffectMaterial.SetFloat(radiusProperty, 0f);
        worldEffectMaterial.SetFloat(widthProperty, waveWidth);
        worldEffectMaterial.SetFloat(invertProperty, 1f);
        worldEffectMaterial.SetFloat(bwProperty, 0f);

        if (worldEffectMaterial.HasProperty(contrastProperty))
            worldEffectMaterial.SetFloat(contrastProperty, timeStopContrast);

        float t = 0f;

        while (t < waveDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / waveDuration);

            worldEffectMaterial.SetFloat(radiusProperty, Mathf.Lerp(0f, waveMaxRadius, p));
            worldEffectMaterial.SetFloat(invertProperty, 1f);
            worldEffectMaterial.SetFloat(bwProperty, p);

            yield return null;
        }

        worldEffectMaterial.SetFloat(radiusProperty, waveMaxRadius);
        worldEffectMaterial.SetFloat(invertProperty, 0f);
        worldEffectMaterial.SetFloat(bwProperty, 1f);

        if (bwHoldTime > 0f)
            yield return new WaitForSeconds(bwHoldTime);

        activeRoutine = null;
    }

    private IEnumerator EndRoutine()
    {
        float startBW = worldEffectMaterial.GetFloat(bwProperty);
        float startContrast = worldEffectMaterial.HasProperty(contrastProperty)
            ? worldEffectMaterial.GetFloat(contrastProperty)
            : normalContrast;

        float t = 0f;

        while (t < returnDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / returnDuration);

            worldEffectMaterial.SetFloat(bwProperty, Mathf.Lerp(startBW, 0f, p));

            if (worldEffectMaterial.HasProperty(contrastProperty))
                worldEffectMaterial.SetFloat(contrastProperty, Mathf.Lerp(startContrast, normalContrast, p));

            yield return null;
        }

        worldEffectMaterial.SetFloat(bwProperty, 0f);
        worldEffectMaterial.SetFloat(invertProperty, 0f);
        worldEffectMaterial.SetFloat(radiusProperty, 0f);

        if (worldEffectMaterial.HasProperty(contrastProperty))
            worldEffectMaterial.SetFloat(contrastProperty, normalContrast);

        activeRoutine = null;
    }
}