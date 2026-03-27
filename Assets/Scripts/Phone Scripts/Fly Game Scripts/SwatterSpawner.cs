using UnityEngine;

public class SwatterSpawner : MonoBehaviour
{
    public RectTransform playfield;
    public RectTransform swatterPairPrefab;

    [Header("Spawn Timing")]
    public float firstSpawnDelay = 0.1f;
    public float baseSpawnInterval = 1.8f;
    public float intervalDecreasePerPass = 0.12f;
    public int maxSpeedupStacks = 5;
    public float minSpawnInterval = 1.0f;

    [Header("Spawn Position")]
    public float firstSpawnPadding = 10f;
    public float spawnPadding = 80f;

    [Header("Vertical Range")]
    public float minY = -200f;
    public float maxY = 200f;

    private float timer;
    private bool spawnedFirst;
    private int speedupStacks;

    void OnEnable()
    {
        ResetSpawner();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!spawnedFirst)
        {
            if (timer >= firstSpawnDelay)
            {
                Spawn(firstSpawnPadding);
                spawnedFirst = true;
                timer = 0f;
            }
            return;
        }

        if (timer >= GetCurrentSpawnInterval())
        {
            timer = 0f;
            Spawn(spawnPadding);
        }
    }

    public void ResetSpawner()
    {
        timer = 0f;
        spawnedFirst = false;
        speedupStacks = 0;
    }

    public void IncreaseSpawnSpeed()
    {
        speedupStacks++;
        speedupStacks = Mathf.Clamp(speedupStacks, 0, maxSpeedupStacks);

        Debug.Log("[Spawner] Speedup stacks: " + speedupStacks +
                  " | Current interval: " + GetCurrentSpawnInterval());
    }

    float GetCurrentSpawnInterval()
    {
        float interval = baseSpawnInterval - (speedupStacks * intervalDecreasePerPass);
        return Mathf.Max(interval, minSpawnInterval);
    }

    void Spawn(float padding)
    {
        float y = Random.Range(minY, maxY);

        float leftEdgeX = -playfield.rect.width * 0.5f;

        RectTransform pair = Instantiate(swatterPairPrefab, playfield);
        pair.name = "SwatterPair";
        pair.localScale = Vector3.one;
        pair.anchoredPosition = new Vector2(leftEdgeX - padding, y);
    }
}