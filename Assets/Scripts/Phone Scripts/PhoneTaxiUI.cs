using UnityEngine;
using TMPro;

public class PhoneTaxiUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject taxiAppPanel;      // Taxi app screen panel (Uber panel)
    public TMP_Text rideInfoText;        // Text inside the app
    public RectTransform gpsArrow;       // Arrow image (tip points UP in the sprite)

    [Header("App Manager (Gate UI to Taxi App Only)")]
    public PhoneAppManager phoneAppManager;   // drag in your PhoneAppManager

    [Header("World References")]
    public Transform player;             // Your car

    [Header("Tuning")]
    [Tooltip("Extra rotation if arrow is visually off (try 0, 90, -90, 180)")]
    public float arrowForwardOffset = 0f;

    [Tooltip("How fast the arrow can turn (degrees per second)")]
    public float rotationSpeed = 720f;

    private Transform gpsTarget;
    private Vector3 initialArrowLocalEuler;
    private bool capturedInitialEuler = false;

    void Awake()
    {
        // Hide by default
        if (gpsArrow != null)
            gpsArrow.gameObject.SetActive(false);

        // Try to auto-find player if not assigned
        if (player == null)
        {
            var p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Start()
    {
        if (gpsArrow != null)
        {
            initialArrowLocalEuler = gpsArrow.localEulerAngles;
            capturedInitialEuler = true;
        }
    }

    // --- GATING ---

    bool TaxiAppIsOpen()
    {
        // Strongest check: panel is active
        if (taxiAppPanel != null && taxiAppPanel.activeInHierarchy)
            return true;

        // If you want to be stricter, require PhoneAppManager says Uber is open:
        // return phoneAppManager != null && phoneAppManager.IsUberOpen();

        return false;
    }

    void SetGpsVisible(bool visible)
    {
        if (gpsArrow != null)
            gpsArrow.gameObject.SetActive(visible);
    }

    public void ForceHideTaxiUI()
    {
        SetGpsVisible(false);
        // optional: clear text too
        // if (rideInfoText != null) rideInfoText.text = "";
    }

    // --- PUBLIC API ---

    public void SetRideInfo(string msg)
    {
        // ONLY update taxi text if taxi app is open
        if (!TaxiAppIsOpen()) return;

        if (rideInfoText != null)
            rideInfoText.text = msg;
    }

    public void SetGpsTarget(Transform target)
    {
        gpsTarget = target;

        // ONLY show GPS if taxi app is open
        if (!TaxiAppIsOpen())
        {
            SetGpsVisible(false);
            return;
        }

        if (gpsArrow != null && gpsTarget != null)
            SetGpsVisible(true);

        Debug.Log("[PhoneTaxiUI] GPS target set to: " + (target != null ? target.name : "null"));
    }

    public void ClearGpsTarget()
    {
        gpsTarget = null;
        SetGpsVisible(false);
    }

    void Update()
    {
        // if taxi app isn't open, hide gps and stop updating
        if (!TaxiAppIsOpen())
        {
            SetGpsVisible(false);
            return;
        }

        if (gpsArrow == null || gpsTarget == null)
        {
            SetGpsVisible(false);
            return;
        }

        if (player == null)
        {
            Debug.LogWarning("[PhoneTaxiUI] Player is not assigned on PhoneTaxiUI!");
            return;
        }

        // ensure we have initial Euler captured if gpsArrow got assigned after Start
        if (!capturedInitialEuler)
        {
            initialArrowLocalEuler = gpsArrow.localEulerAngles;
            capturedInitialEuler = true;
        }

        // show while active + has target
        SetGpsVisible(true);

        // Direction to target in player local space
        Vector3 localDir = player.InverseTransformPoint(gpsTarget.position);
        localDir.y = 0f;

        if (localDir.sqrMagnitude < 0.0001f)
            return;

        float angle = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;
        float finalZ = -angle + arrowForwardOffset;

        Quaternion targetRot = Quaternion.Euler(
            initialArrowLocalEuler.x,
            initialArrowLocalEuler.y,
            finalZ
        );

        gpsArrow.localRotation = Quaternion.RotateTowards(
            gpsArrow.localRotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );
    }
}

