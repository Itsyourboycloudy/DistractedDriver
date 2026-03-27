using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TaxiOfferSlotUI : MonoBehaviour
{
    [Header("UI")]
    public Image portraitImage;
    public TMP_Text titleText;
    public Image[] starImages;   // size 5
    public Button acceptButton;

    [Header("Star Sprites")]
    public Sprite starFullSprite;
    public Sprite starHalfSprite;
    public Sprite starEmptySprite;

    int offerIndex;
    TaxiRideManager rideManager;

    public void Bind(TaxiRideManager manager, int index, string title, float stars, Sprite portrait, bool interactable)
    {
        rideManager = manager;
        offerIndex = index;

        if (titleText != null) titleText.text = title;

        if (portraitImage != null)
        {
            portraitImage.sprite = portrait;
            portraitImage.enabled = (portrait != null);
        }

        SetStars(stars);

        if (acceptButton != null)
        {
            acceptButton.interactable = interactable;
            acceptButton.onClick.RemoveAllListeners();
            acceptButton.onClick.AddListener(() =>
            {
                if (rideManager != null)
                    rideManager.AcceptOffer(offerIndex);
            });
        }
    }

    void SetStars(float stars)
    {
        if (starImages == null || starImages.Length < 5) return;

        stars = Mathf.Clamp(stars, 1f, 5f);

        int full = Mathf.FloorToInt(stars);
        bool half = (stars - full) >= 0.5f;

        for (int i = 0; i < 5; i++)
        {
            if (starImages[i] == null) continue;

            if (i < full) starImages[i].sprite = starFullSprite;
            else if (i == full && half) starImages[i].sprite = starHalfSprite;
            else starImages[i].sprite = starEmptySprite;

            starImages[i].enabled = (starImages[i].sprite != null);
        }
    }
}
