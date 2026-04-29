using UnityEngine;

public class PassengerPickup : MonoBehaviour
{
    [Tooltip("Name that will show up in the phone UI")]
    public string passengerName = "Alex";

    [Header("Taxi App Portrait")]
    public Sprite portraitSprite;

    [Header("Difficulty (Stars)")]
    [Range(1f, 5f)]
    public float difficultyStars = 1f;

    [Header("Pickup Dialogue Options (pick 1 of these 3)")]
    [TextArea(2, 6)] public string pickupDialogue1;
    [TextArea(2, 6)] public string pickupDialogue2;
    [TextArea(2, 6)] public string pickupDialogue3;

    [Header("Dropoff Dialogue Options (pick 1 of these 3)")]
    [TextArea(2, 6)] public string dropoffDialogue1;
    [TextArea(2, 6)] public string dropoffDialogue2;
    [TextArea(2, 6)] public string dropoffDialogue3;

    [Header("Future Hit Reaction Options")]
    [TextArea(2, 6)] public string hitDialogue1;
    [TextArea(2, 6)] public string hitDialogue2;
    [TextArea(2, 6)] public string hitDialogue3;

    public string GetRandomPickupDialogue()
    {
        return GetRandomFromThree(pickupDialogue1, pickupDialogue2, pickupDialogue3, "Hey, let's get going.");
    }

    public string GetRandomDropoffDialogue()
    {
        return GetRandomFromThree(dropoffDialogue1, dropoffDialogue2, dropoffDialogue3, "Alright, this is my stop.");
    }

    public string GetRandomHitDialogue()
    {
        return GetRandomFromThree(hitDialogue1, hitDialogue2, hitDialogue3, "Whoa, watch it!");
    }

    private string GetRandomFromThree(string a, string b, string c, string fallback)
    {
        System.Collections.Generic.List<string> valid = new System.Collections.Generic.List<string>();

        if (!string.IsNullOrWhiteSpace(a)) valid.Add(a);
        if (!string.IsNullOrWhiteSpace(b)) valid.Add(b);
        if (!string.IsNullOrWhiteSpace(c)) valid.Add(c);

        if (valid.Count == 0)
            return fallback;

        return valid[Random.Range(0, valid.Count)];
    }
}