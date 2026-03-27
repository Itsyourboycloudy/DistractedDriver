using UnityEngine;

public class PassengerPickup : MonoBehaviour
{
    [Tooltip("Name that will show up in the phone UI")]
    public string passengerName = "Alex";

    [Header("Taxi App Portrait")]
    public Sprite portraitSprite;

    [Header("Difficulty (Stars)")]
    [Range(1f, 5f)]
    public float difficultyStars = 1f; // ex: 1, 3.5, 5

    // DATA ONLY.
    // No trigger logic here anymore.
    // PickupZone handles trigger detection.
}
