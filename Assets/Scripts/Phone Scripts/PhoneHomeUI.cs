using UnityEngine;
using TMPro;

public class PhoneHomeUI : MonoBehaviour
{
    [Header("Home UI")]
    public TMP_Text taxiNotificationText;
    public GameObject taxiNotificationIcon; // optional

    public void ShowTaxiNotification(string msg)
    {
        if (taxiNotificationText != null)
            taxiNotificationText.text = msg;

        if (taxiNotificationIcon != null)
            taxiNotificationIcon.SetActive(true);
    }

    public void ShowRideAccepted()
    {
        ShowTaxiNotification("Ride accepted");
    }

    public void ShowRideAlreadyAccepted()
    {
        ShowTaxiNotification("You already have a ride accepted");
    }

    public void ClearTaxiNotification()
    {
        if (taxiNotificationText != null)
            taxiNotificationText.text = "";

        if (taxiNotificationIcon != null)
            taxiNotificationIcon.SetActive(false);
    }
}