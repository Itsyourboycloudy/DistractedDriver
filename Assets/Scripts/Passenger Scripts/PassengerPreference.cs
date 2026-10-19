using UnityEngine;

public enum PassengerPreference { Speed, Smooth, PhoneGame }

[System.Serializable]
public class PassengerRidePerformance
{
    public PassengerPreference preference;
    public float elapsed, timeTarget = 60;
    public int roughEvents, phonePoints;
    public bool tracking;
    public string Goal => preference == PassengerPreference.Speed ? $"SPEED: deliver within {timeTarget:0}s for +50%"
        : preference == PassengerPreference.Smooth ? "SMOOTH: no hard turns / impacts for up to +50%"
        : "GAMER: score 5 phone points this ride for +50%";
    public string Status => preference == PassengerPreference.Speed ? $"SPEED {Mathf.Max(0, timeTarget - elapsed):0}s left"
        : preference == PassengerPreference.Smooth ? $"SMOOTH {roughEvents} rough moves"
        : $"GAMER {phonePoints}/5 points";
    public float Bonus => preference == PassengerPreference.Speed ? (elapsed <= timeTarget ? 0.5f : 0)
        : preference == PassengerPreference.Smooth ? Mathf.Max(0, 0.5f - roughEvents * 0.2f)
        : phonePoints >= 5 ? 0.5f : 0;
    public int Tip(int fare) => Bonus <= 0 ? 0 : Mathf.Max(1, Mathf.RoundToInt(fare * Bonus));
}
