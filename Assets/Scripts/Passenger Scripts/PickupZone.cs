using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PickupZone : MonoBehaviour
{
    public TaxiRideManager rideManager;

    [Header("Visual Root (floor quad, glow base, etc)")]
    public GameObject visualRoot; // drag your VisualRoot here

    private BoxCollider col;

    void Awake()
    {
        col = GetComponent<BoxCollider>();
        col.isTrigger = true;
        SetActive(false);
    }

    public void SetActive(bool active)
    {
        if (visualRoot != null)
            visualRoot.SetActive(active);

        if (col != null)
            col.enabled = active;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!col.enabled) return;
        if (!other.CompareTag("Player")) return;

        if (rideManager != null)
            rideManager.OnPickupZoneEntered(this);
    }
}
