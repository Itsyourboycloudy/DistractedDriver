using UnityEngine;

public class TaxiAppOffersUI : MonoBehaviour
{
    public TaxiRideManager rideManager;
    public TaxiOfferSlotUI[] slots;

    public void Refresh()
    {
        if (rideManager == null || slots == null) return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;

            var offer = rideManager.GetOffer(i);
            bool hasOffer = offer.IsValid;

            string title = hasOffer ? $"{offer.passengerName} → {offer.dropoffName}" : "—";
            float stars = hasOffer ? offer.stars : 1f;
            Sprite portrait = hasOffer ? offer.portrait : null;

            slots[i].Bind(
                rideManager,
                i,
                title,
                stars,
                portrait,
                interactable: hasOffer && rideManager.CanAcceptOffers
            );
        }
    }
}
