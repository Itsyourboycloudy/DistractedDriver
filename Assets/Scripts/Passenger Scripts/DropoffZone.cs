using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DropoffZone : MonoBehaviour
{
    [Tooltip("Name that appears in phone UI, e.g. '4th Street'")]
    public string dropoffName = "4th Street";

    public TaxiRideManager rideManager;

    [Header("Visual Root (floor quad, glow base, etc)")]
    public GameObject visualRoot;

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
            rideManager.OnDropoffTrigger(this);
    }
}
