using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WhackAFlyGame : MonoBehaviour
{
    [System.Serializable]
    public class Hole
    {
        public Image pipeImage;
        public Image flyImage;
        public Button hitButton;
        [System.NonSerialized] public float age;
        [System.NonSerialized] public bool up;
    }
    [Header("Art - replace Pipe Sprite with your finished pipe")]
    public Sprite pipeSprite, flySprite, hitSprite;
    public Hole[] holes;
    public GameObject board, mainMenuPanel, gameOverPanel;
    public TMP_Text scoreText, resultText;
    public AudioSource scoreSource, hitSource;
    public float roundSeconds = 45f;
    public int startingLives = 3;
    public bool IsPlaying { get; private set; }
    public int Score { get; private set; }
    public int Lives { get; private set; }
    float remaining, nextSpawn;
    public void NextGame() { PhoneAppManager.Instance?.minigameSelector?.NextGame(); }

    void Awake()
    {
        for (int i = 0; i < holes.Length; i++)
        {
            int index = i;
            holes[i].hitButton.onClick = new Button.ButtonClickedEvent();
            holes[i].hitButton.onClick.AddListener(() => Whack(index));
        }
    }
    public void StartGame()
    {
        ResetGame();
        mainMenuPanel.SetActive(true);
        FlyMusicManager.Instance?.ReturnToFly1();
    }
    public void BeginRun()
    {
        Score = 0; Lives = startingLives; remaining = roundSeconds; nextSpawn = 0.4f;
        IsPlaying = true;
        mainMenuPanel.SetActive(false); gameOverPanel.SetActive(false); board.SetActive(true);
        foreach (var hole in holes) { hole.up = false; RenderHole(hole); }
        DopamineManager.Instance?.SetPlayingOnPhone(false);
        DopamineManager.Instance?.ResetPhoneGameMultiplier();
        FlyMusicManager.Instance?.RequestTransitionToGameplay();
        UpdateUI();
    }
    public void ResetGame()
    {
        IsPlaying = false;
        if (board != null) board.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (holes != null) foreach (var hole in holes) { hole.up = false; RenderHole(hole); }
        DopamineManager.Instance?.SetPlayingOnPhone(false);
        DopamineManager.Instance?.ResetPhoneGameMultiplier();
        FlyMusicManager.Instance?.StopMusic();
    }
    void OnDisable() { ResetGame(); }
    void Update()
    {
        if (!IsPlaying || Time.timeScale == 0) return;
        remaining -= Time.deltaTime;
        if (remaining <= 0) { Finish(); return; }
        float visibleTime = Mathf.Max(0.7f, 1.65f - Score * 0.025f);
        foreach (var hole in holes)
        {
            if (!hole.up) continue;
            hole.age += Time.deltaTime;
            if (hole.age >= visibleTime)
            {
                hole.up = false; Lives--;
                if (hitSource != null) hitSource.Play();
                if (Lives <= 0) { Finish(); return; }
            }
            RenderHole(hole);
        }
        nextSpawn -= Time.deltaTime;
        if (nextSpawn <= 0)
        {
            int start = Random.Range(0, holes.Length);
            for (int i = 0; i < holes.Length; i++)
            {
                var hole = holes[(start + i) % holes.Length];
                if (hole.up) continue;
                hole.up = true; hole.age = 0; RenderHole(hole); break;
            }
            nextSpawn = Mathf.Max(0.4f, 0.95f - Score * 0.015f);
        }
        UpdateUI();
    }
    public void Whack(int index)
    {
        if (!IsPlaying || Time.timeScale == 0 || index < 0 || index >= holes.Length) return;
        var hole = holes[index];
        if (!hole.up || hole.age < 0.12f) return;
        hole.up = false; Score++;
        RenderHole(hole);
        if (scoreSource != null) scoreSource.Play();
        var dopamine = DopamineManager.Instance;
        if (dopamine != null)
        {
            dopamine.IncreasePhoneGameMultiplier();
            dopamine.AddDopamine(6f * dopamine.GetPhoneGameMultiplier() * TaxiBuildStats.DopamineGain);
        }
        TaxiRideManager.Instance?.RecordPhonePoint();
        UpdateUI();
    }
    void RenderHole(Hole hole)
    {
        if (hole.pipeImage != null && pipeSprite != null) hole.pipeImage.sprite = pipeSprite;
        if (hole.flyImage == null) return;
        hole.flyImage.sprite = flySprite;
        hole.flyImage.gameObject.SetActive(hole.up);
        hole.hitButton.interactable = IsPlaying && hole.up && hole.age >= 0.12f;
        float rise = Mathf.Clamp01(hole.age / 0.16f);
        hole.flyImage.rectTransform.anchoredPosition = new Vector2(0, -hole.flyImage.rectTransform.rect.height * (1 - rise));
    }
    void Finish()
    {
        IsPlaying = false;
        foreach (var hole in holes) { hole.up = false; RenderHole(hole); }
        gameOverPanel.SetActive(true);
        resultText.text = (Lives > 0 ? "TIME UP!" : "GAME OVER") + "\nSCORE: " + Score;
        FlyMusicManager.Instance?.ReturnToFly1();
        UpdateUI();
    }
    void UpdateUI() { scoreText.text = $"SCORE: {Score}   LIVES: {Lives}\nTIME: {Mathf.CeilToInt(Mathf.Max(0, remaining))}"; }
}
