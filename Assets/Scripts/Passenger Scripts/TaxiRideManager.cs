using UnityEngine;
using System.Collections;

public class TaxiRideManager : MonoBehaviour
{
    public static TaxiRideManager Instance { get; private set; }

    [Header("UI References")]
    public PhoneHomeUI phoneHomeUI;
    public PhoneTaxiUI phoneTaxiUI;

    [Header("Taxi App Offers UI")]
    public TaxiAppOffersUI offersUI;

    [Header("Taxi App Views (prevents GPS jumping)")]
    public GameObject offersView;
    public GameObject activeRideView;

    [Header("Dialogue Popup UI (sprite pops up)")]
    public PassengerDialogueUI dialogueUI;

    [Header("World References")]
    public PickupZone[] pickupZones;
    public DropoffZone[] dropoffs;
    public PassengerPickup[] passengers;

    [Header("Offers Settings")]
    public int offersCount = 3;

    [Header("Flow Tuning")]
    public float nextRideDelay = 0.5f;

    [Header("Fare Upgrades")]
    public float permanentFareBonus = 0f;
    public float temporaryFareBonus = 0f;
    public int surgeDaysRemaining = 0;

    private enum RideState { Idle, ChoosingOffer, GoingToPickup, GoingToDropoff }
    private RideState state = RideState.Idle;

    private int currentPassengerIndex = -1;
    private int currentDropoffIndex = -1;

    private Coroutine nextRideRoutine;

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

    private string currentRideMsg = "";
    private Transform currentGpsTarget = null;

    public bool CanAcceptOffers => state == RideState.ChoosingOffer;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        offers = new RideOffer[Mathf.Max(1, offersCount)];
        BuildNewOffers();
    }

    private void SetTaxiViews(bool showOffers)
    {
        if (offersView != null) offersView.SetActive(showOffers);
        if (activeRideView != null) activeRideView.SetActive(!showOffers);
    }

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

        SetTaxiViews(true);

        for (int i = 0; i < pickupZones.Length; i++)
        {
            if (pickupZones[i] != null)
                pickupZones[i].SetActive(false);
        }

        for (int i = 0; i < dropoffs.Length; i++)
        {
            if (dropoffs[i] != null)
                dropoffs[i].gameObject.SetActive(false);
        }

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

        if (phoneHomeUI != null)
            phoneHomeUI.ShowTaxiNotification($"{offers.Length} ride requests waiting — open Taxi app to choose.");

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

        SetTaxiViews(false);

        for (int i = 0; i < pickupZones.Length; i++)
        {
            if (pickupZones[i] != null)
                pickupZones[i].SetActive(false);
        }

        currentPickupZone.rideManager = this;
        currentPickupZone.SetActive(true);

        if (phoneHomeUI != null)
            phoneHomeUI.ClearTaxiNotification();

        string msg = $"{currentPassenger.passengerName} accepted — go to the pickup square";
        currentRideMsg = msg;

        currentGpsTarget = currentPickupZone.transform;
        PushTaxiUI();

        if (offersUI != null)
            offersUI.Refresh();

        Debug.Log($"[TaxiRideManager] Accepted offer {offerIndex}: {currentPassenger.passengerName} -> {currentDropoff.dropoffName}");
    }

    public void OnPickupZoneEntered(PickupZone zone)
    {
        if (state != RideState.GoingToPickup)
            return;

        if (currentPassengerIndex < 0 || currentPassengerIndex >= pickupZones.Length)
            return;

        if (zone != pickupZones[currentPassengerIndex])
            return;

        state = RideState.GoingToDropoff;

        zone.SetActive(false);

        PassengerPickup currentPassenger = passengers[currentPassengerIndex];
        if (dialogueUI != null && currentPassenger != null)
            dialogueUI.Show(currentPassenger.passengerName, currentPassenger.portraitSprite);

        if (currentDropoffIndex < 0 || currentDropoffIndex >= dropoffs.Length)
            return;

        DropoffZone currentDropoff = dropoffs[currentDropoffIndex];
        if (currentDropoff == null) return;

        currentDropoff.gameObject.SetActive(true);

        string msg = $"Taking {currentPassenger.passengerName} to {currentDropoff.dropoffName}";
        currentRideMsg = msg;
        currentGpsTarget = currentDropoff.transform;
        PushTaxiUI();

        Debug.Log($"Picked up {currentPassenger.passengerName}, heading to {currentDropoff.dropoffName}");
    }

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

        Debug.Log("[TaxiRideManager] Current fare multiplier = " + GetCurrentFareMultiplier());

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

    public void AddFareMultiplier(float amount)
    {
        permanentFareBonus += amount;
        Debug.Log("[TaxiRideManager] Permanent fare bonus now = " + permanentFareBonus);
    }

    public void AddTemporaryFareMultiplier(float amount, int days)
    {
        temporaryFareBonus += amount;
        surgeDaysRemaining = Mathf.Max(surgeDaysRemaining, days);

        Debug.Log("[TaxiRideManager] Temporary fare bonus now = " + temporaryFareBonus +
                  " for " + surgeDaysRemaining + " day(s)");
    }

    public float GetCurrentFareMultiplier()
    {
        return 1f + permanentFareBonus + temporaryFareBonus;
    }

    public void AdvanceUpgradeDay()
    {
        if (surgeDaysRemaining > 0)
        {
            surgeDaysRemaining--;

            if (surgeDaysRemaining <= 0)
            {
                temporaryFareBonus = 0f;
                surgeDaysRemaining = 0;
                Debug.Log("[TaxiRideManager] Surge pricing expired.");
            }
        }
    }
}