using UnityEngine;

public class TaxiPassengerSystem : MonoBehaviour
{
    public Transform pickupPoint;
    public Transform dropoffPoint;
    public GameObject passenger;
    public GameObject pickupZone;
    public GameObject dropoffZone;
    public LineRenderer gpsLine;
    public float enterDistance = 3f;

    [Header("Passenger Exit Settings")]
    public float exitOffsetSide = 2f;   // how far to the side of the car they appear
    public float exitOffsetForward = 1f; // slightly ahead of car
    public float exitOffsetUp = 0f;     // lift slightly if needed

    private bool hasPassenger = false;
    private bool onRide = false;

    void Start()
    {
        if (gpsLine != null)
            gpsLine.enabled = false;

        if (dropoffZone != null)
            dropoffZone.SetActive(false);

        if (pickupZone != null)
            pickupZone.SetActive(true);

        if (passenger != null)
            passenger.SetActive(true);
    }

    void Update()
    {
        if (pickupPoint == null || dropoffPoint == null) return;

        float distToPickup = Vector3.Distance(transform.position, pickupPoint.position);
        float distToDropoff = Vector3.Distance(transform.position, dropoffPoint.position);

        if (!onRide && distToPickup <= enterDistance)
            PickupPassenger();

        if (onRide && hasPassenger && distToDropoff <= enterDistance)
            DropOffPassenger();

        if (onRide && hasPassenger)
            UpdateGPS();
    }

    void PickupPassenger()
    {
        if (passenger != null)
            passenger.SetActive(false);

        if (pickupZone != null)
            pickupZone.SetActive(false);

        if (dropoffZone != null)
            dropoffZone.SetActive(true);

        hasPassenger = true;
        onRide = true;

        if (gpsLine != null)
        {
            gpsLine.enabled = true;
            gpsLine.positionCount = 2;
        }
    }

    void DropOffPassenger()
    {
        hasPassenger = false;
        onRide = false;

        if (gpsLine != null)
            gpsLine.enabled = false;

        if (dropoffZone != null)
            dropoffZone.SetActive(false);

        if (passenger != null)
        {
            // Calculate safe exit position beside the car
            Vector3 exitPos = transform.position
                            + transform.right * exitOffsetSide
                            + transform.forward * exitOffsetForward
                            + Vector3.up * exitOffsetUp;

            passenger.transform.position = exitPos;
            passenger.transform.rotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
            passenger.SetActive(true);
        }
    }

    void UpdateGPS()
    {
        if (gpsLine == null) return;

        gpsLine.SetPosition(0, transform.position + Vector3.up * 0.2f);
        gpsLine.SetPosition(1, dropoffPoint.position + Vector3.up * 0.2f);
    }

    void OnDrawGizmos()
    {
        if (pickupPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pickupPoint.position, enterDistance);
        }

        if (dropoffPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(dropoffPoint.position, enterDistance);
        }
    }
}
