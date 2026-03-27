using UnityEngine;

public class ZoneHoverPulse : MonoBehaviour
{
    [Header("Hover")]
    public float hoverHeight = 0.02f;     // how much it bobs up/down
    public float hoverSpeed = 2.0f;

    [Header("Scale Pulse")]
    public Transform pulseTarget;         // assign floor OR VisualRoot
    public float pulseAmount = 0.03f;     // how much it grows/shrinks
    public float pulseSpeed = 2.0f;

    Vector3 startPos;
    Vector3 startScale;

    void Start()
    {
        startPos = transform.localPosition;

        if (pulseTarget == null)
            pulseTarget = transform;

        startScale = pulseTarget.localScale;
    }

    void Update()
    {
        // hover
        float h = Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        transform.localPosition = startPos + new Vector3(0f, h, 0f);

        // pulse
        float p = (Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f);
        float scale = 1f + (p - 0.5f) * 2f * pulseAmount;
        pulseTarget.localScale = startScale * scale;
    }
}
