using System;

[Serializable]
public class MinigameResult
{
    public int score;
    public float dopamineMultiplierEarned; // how much you boosted multiplier by
    public int coinsEarned;                // for your shop later
    public string gameId;                  // "fly_flappy", etc.
}