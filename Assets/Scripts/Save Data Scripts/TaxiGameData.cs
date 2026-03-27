using UnityEngine;

[System.Serializable]
public class TaxiGameData
{
    public int score;
    public int ridesCompleted;
    public float timePlayed;
    public float averageSpeed;

    public TaxiGameData(int score, int ridesCompleted, float timePlayed, float averageSpeed)
    {
        this.score = score;
        this.ridesCompleted = ridesCompleted;
        this.timePlayed = timePlayed;
        this.averageSpeed = averageSpeed;
    }
}

