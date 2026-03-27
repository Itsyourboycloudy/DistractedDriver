using UnityEngine;
using UnityEngine.UI;

public class RetroDisplayToggle : MonoBehaviour
{
    [Header("References")]
    public Camera targetCamera;
    public RawImage retroRawImage;
    public RenderTexture retroRenderTexture;

    [Header("Settings")]
    public bool retroEnabled = true;
    public KeyCode toggleKey = KeyCode.F;

    void Start()
    {
        ApplyMode();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            retroEnabled = !retroEnabled;
            ApplyMode();
        }
    }

    public void SetRetroMode(bool enabled)
    {
        retroEnabled = enabled;
        ApplyMode();
    }

    void ApplyMode()
    {
        if (targetCamera == null) return;

        if (retroEnabled)
        {
            targetCamera.targetTexture = retroRenderTexture;

            if (retroRawImage != null)
                retroRawImage.gameObject.SetActive(true);
        }
        else
        {
            targetCamera.targetTexture = null;

            if (retroRawImage != null)
                retroRawImage.gameObject.SetActive(false);
        }
    }
}
