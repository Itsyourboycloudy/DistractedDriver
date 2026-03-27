using UnityEngine;
using System.Collections;

public class TaxiRideManager : MonoBehaviour
{
    [Header("UI References")]
    public PhoneHomeUI phoneHomeUI;
    public PhoneTaxiUI phoneTaxiUI;

    [Header("Taxi App Offers UI")]
    public TaxiAppOffersUI offersUI;   // drag in

    [Header("Taxi App Views (prevents GPS jumping)")]
    public GameObject offersView;      // UI parent that contains the 3 offer slots
    public GameObject activeRideView;  // UI parent that contains ride info + GPS arrow

    [Header("Dialogue Popup UI (sprite pops up)")]
    public PassengerDialogueUI dialogueUI; // optional (Canvas object)

    [Header("World References")]
    public PickupZone[] pickupZones;         // green squares (world triggers)
    public DropoffZone[] dropoffs;           // dropoff triggers
    public PassengerPickup[] passengers;     // DATA ONLY (name, stars, portrait)

    [Header("Offers Settings")]
    public int offersCount = 3;

    [Header("Flow Tuning")]
    public float nextRideDelay = 0.5f;

    private enum RideState { Idle, ChoosingOffer, GoingToPickup, GoingToDropoff }
    private RideState state = RideState.Idle;

    private int currentPassengerIndex = -1;
    private int currentDropoffIndex = -1;

    private Coroutine nextRideRoutine;

    // ===== Offer Data =====
    [System.Serializable]
    public struct RideOffer
    {
        public bool IsValid;
        public int passengerIndex;
        public int dropoffIndex;
        public float stars;
        public string passengerName;
        public string dropoffName;
        public Sprite portrait;
    }

    private RideOffer[] offers;

    // Cache so Taxi app + GPS can always refresh cleanly
    private string currentRideMsg = "";
    private Transform currentGpsTarget = null;

    public bool CanAcceptOffers => state == RideState.ChoosingOffer;

    void Start()
    {
        offers = new RideOffer[Mathf.Max(1, offersCount)];
        BuildNewOffers();
    }

    // =========================
    // VIEWS
    // =========================
    private void SetTaxiViews(bool showOffers)
    {
        if (offersView != null) offersView.SetActive(showOffers);
        if (activeRideView != null) activeRideView.SetActive(!showOffers);
    }

    // =========================
    // OFFERS
    // =========================

    private void BuildNewOffers()
    {
        if (nextRideRoutine != null)
        {
            StopCoroutine(nextRideRoutine);
            nextRideRoutine = null;
        }

        if (passengers == null || passengers.Length == 0 ||
            dropoffs == null || dropoffs.Length == 0 ||
            pickupZones == null || pickupZones.Length == 0)
        {
            Debug.LogWarning("TaxiRideManager: Need passengers, pickupZones, and dropoffs assigned.");
            return;
        }

        state = RideState.ChoosingOffer;

        // Show offers view, hide active ride view
        SetTaxiViews(true);

        // Turn OFF all pickup zones while choosing
        for (int i = 0; i < pickupZones.Length; i++)
        {
            if (pickupZones[i] != null)
                pickupZones[i].SetActive(false);
        }

        // Hide all dropoffs
        for (int i = 0; i < dropoffs.Length; i++)
        {
            if (dropoffs[i] != null)
                dropoffs[i].gameObject.SetActive(false);
        }

        // Build random offers (WITH replacement, duplicates allowed)
        for (int i = 0; i < offers.Length; i++)
        {
            int pIndex = Random.Range(0, passengers.Length);
            int dIndex = Random.Range(0, dropoffs.Length);

            PassengerPickup p = passengers[pIndex];
            DropoffZone d = dropoffs[dIndex];

            offers[i] = new RideOffer
            {
                IsValid = (p != null && d != null),
                passengerIndex = pIndex,
                dropoffIndex = dIndex,
                stars = p != null ? p.difficultyStars : 1f,
                passengerName = p != null ? p.passengerName : "Passenger",
                dropoffName = d != null ? d.dropoffName : "Dropoff",
                portrait = (p != null) ? p.portraitSprite : null
            };
        }

        // Home notification: generic
        if (phoneHomeUI != null)
            phoneHomeUI.ShowTaxiNotification($"{offers.Length} ride requests waiting — open Taxi app to choose.");

        // Taxi app: show offers list (no GPS yet)
        currentRideMsg = "Choose a ride:";
        currentGpsTarget = null;
        PushTaxiUI();

        if (offersUI != null)
            offersUI.Refresh();

        Debug.Log("[TaxiRideManager] Built new offers.");
    }

    public RideOffer GetOffer(int index)
    {
        if (offers == null || index < 0 || index >= offers.Length)
            return new RideOffer { IsValid = false };
        return offers[index];
    }

    public void AcceptOffer(int offerIndex)
    {
        if (state != RideState.ChoosingOffer) return;
        if (offers == null || offerIndex < 0 || offerIndex >= offers.Length) return;
        if (!offers[offerIndex].IsValid) return;

        var chosen = offers[offerIndex];
        currentPassengerIndex = chosen.passengerIndex;
        currentDropoffIndex = chosen.dropoffIndex;

        if (currentPassengerIndex < 0 || currentPassengerIndex >= passengers.Length) return;
        if (currentDropoffIndex < 0 || currentDropoffIndex >= dropoffs.Length) return;
        if (currentPassengerIndex < 0 || currentPassengerIndex >= pickupZones.Length) return;

        PassengerPickup currentPassenger = passengers[currentPassengerIndex];
        DropoffZone currentDropoff = dropoffs[currentDropoffIndex];
        PickupZone currentPickupZone = pickupZones[currentPassengerIndex];

        if (currentPassenger == null || currentDropoff == null || currentPickupZone == null) return;

        state = RideState.GoingToPickup;

        // Switch to active ride UI (GPS lives here so it won't jump)
        SetTaxiViews(false);

        // Turn OFF all pickup zones, then enable ONLY the accepted one
        for (int i = 0; i < pickupZones.Length; i++)
        {
            if (pickupZones[i] != null)
                pickupZones[i].SetActive(false);
        }

        // Ensure the pickup zone knows who to call
        currentPickupZone.rideManager = this;
        currentPickupZone.SetActive(true);

        // Clear home notif
        if (phoneHomeUI != null)
            phoneHomeUI.ClearTaxiNotification();

        string msg = $"{currentPassenger.passengerName} accepted — go to the pickup square";
        currentRideMsg = msg;

        // GPS goes to pickup zone center (not a passenger sprite)
        currentGpsTarget = currentPickupZone.transform;
        PushTaxiUI();

        if (offersUI != null)
            offersUI.Refresh();

        Debug.Log($"[TaxiRideManager] Accepted offer {offerIndex}: {currentPassenger.passengerName} -> {currentDropoff.dropoffName}");
    }

    // =========================
    // PICKUP / DROPOFF FLOW
    // =========================

    // Called by PickupZone when player enters green square
    public void OnPickupZoneEntered(PickupZone zone)
    {
        if (state != RideState.GoingToPickup)
            return;

        if (currentPassengerIndex < 0 || currentPassengerIndex >= pickupZones.Length)
            return;

        // Only accept pickup if it's the active zone
        if (zone != pickupZones[currentPassengerIndex])
            return;

        state = RideState.GoingToDropoff;

        // Turn off pickup zone
        zone.SetActive(false);

        // Show dialogue popup with the passenger sprite
        PassengerPickup currentPassenger = passengers[currentPassengerIndex];
        if (dialogueUI != null && currentPassenger != null)
            dialogueUI.Show(currentPassenger.passengerName, currentPassenger.portraitSprite);

        if (currentDropoffIndex < 0 || currentDropoffIndex >= dropoffs.Length)
            return;

        DropoffZone currentDropoff = dropoffs[currentDropoffIndex];
        if (currentDropoff == null) return;

        // Activate dropoff
        currentDropoff.gameObject.SetActive(true);

        string msg = $"Taking {currentPassenger.passengerName} to {currentDropoff.dropoffName}";
        currentRideMsg = msg;
        currentGpsTarget = currentDropoff.transform;
        PushTaxiUI();

        Debug.Log($"Picked up {currentPassenger.passengerName}, heading to {currentDropoff.dropoffName}");
    }

    // DropoffZone calls this when player enters dropoff square
    public void OnDropoffTrigger(DropoffZone zone)
    {
        if (state != RideState.GoingToDropoff)
            return;

        if (currentDropoffIndex < 0 || currentDropoffIndex >= dropoffs.Length)
            return;

        DropoffZone currentDropoff = dropoffs[currentDropoffIndex];
        if (currentDropoff == null) return;

        if (zone != currentDropoff)
            return;

        state = RideState.Idle;

        PassengerPickup currentPassenger =
            (currentPassengerIndex >= 0 && currentPassengerIndex < passengers.Length)
            ? passengers[currentPassengerIndex]
            : null;

        string passengerName = currentPassenger != null ? currentPassenger.passengerName : "Passenger";
        string msg = $"Ride complete! {passengerName} is at {currentDropoff.dropoffName}.";

        currentRideMsg = msg;
        currentGpsTarget = null;
        PushTaxiUI();

        if (GameManager.Instance != null)
            GameManager.Instance.AddRideScore();

        // Rebuild offers after delay (and switch back to offers view inside BuildNewOffers)
        if (nextRideRoutine != null)
            StopCoroutine(nextRideRoutine);

        nextRideRoutine = StartCoroutine(BuildOffersAfterDelay(nextRideDelay));
    }

    private IEnumerator BuildOffersAfterDelay(float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        BuildNewOffers();
        nextRideRoutine = null;
    }

    // =========================
    // TAXI UI PUSH
    // =========================

    private void PushTaxiUI()
    {
        if (phoneTaxiUI != null)
        {
            phoneTaxiUI.SetRideInfo(currentRideMsg);

            if (currentGpsTarget != null)
                phoneTaxiUI.SetGpsTarget(currentGpsTarget);
            else
                phoneTaxiUI.ClearGpsTarget();
        }
    }
}
