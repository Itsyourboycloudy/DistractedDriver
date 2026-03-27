using TMPro;
using UnityEngine;

public class FlyMinigameManager : MonoBehaviour
{
    public static FlyMinigameManager Instance;

    [Header("Refs")]
    public FlyFlapUI fly;
    public RectTransform gameArea;
    public TMP_Text scoreText;
    public GameObject gameOverPanel;
    public SwatterSpawner spawner;

    [Header("Menu UI")]
    public GameObject mainMenuPanel;
    public GameObject startPromptPanel;

    public bool IsPlaying { get; private set; }
    public bool WaitingForFirstClick { get; private set; }

    private int score;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ShowMainMenu();
    }

    void Update()
    {
        if (WaitingForFirstClick && Input.GetMouseButtonDown(0))
        {
            BeginActualRun();
        }
    }

    public void ShowMainMenu()
    {
        IsPlaying = false;
        WaitingForFirstClick = false;

        if (fly != null)
            fly.ResetFly();

        if (spawner != null)
        {
            spawner.enabled = false;
            spawner.ResetSpawner();
        }

        ClearExistingSwatters();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (startPromptPanel != null)
            startPromptPanel.SetActive(false);

        score = 0;
        UpdateUI();

        if (DopamineManager.Instance != null)
        {
            DopamineManager.Instance.SetPlayingOnPhone(false);
            DopamineManager.Instance.ResetPhoneGameMultiplier();
        }

        Debug.Log("[FlyGame] ShowMainMenu()");
    }

    public void OnPlayButtonPressed()
    {
        IsPlaying = false;
        WaitingForFirstClick = true;

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (startPromptPanel != null)
            startPromptPanel.SetActive(true);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (fly != null)
            fly.ResetFly();

        if (spawner != null)
        {
            spawner.enabled = false;
            spawner.ResetSpawner();
        }

        ClearExistingSwatters();

        score = 0;
        UpdateUI();

        Debug.Log("[FlyGame] Waiting for first click");
    }

    void BeginActualRun()
    {
        WaitingForFirstClick = false;
        IsPlaying = true;

        if (startPromptPanel != null)
            startPromptPanel.SetActive(false);

        if (DopamineManager.Instance != null)
        {
            Debug.Log("[FlyGame] Turning dopamine ON");
            DopamineManager.Instance.ResetPhoneGameMultiplier();
            DopamineManager.Instance.SetPlayingOnPhone(true);
        }

        if (fly != null)
        {
            fly.gameArea = gameArea;
            fly.StartFly();
            fly.ForceFlap(); // first click also makes the fly go up
        }

        if (spawner != null)
            spawner.enabled = true;

        Debug.Log("[FlyGame] BeginActualRun()");
    }

    public void StartGame()
    {
        ShowMainMenu();
    }

    public void RestartGame()
    {
        Debug.Log("[FlyGame] Restart button clicked");
        OnPlayButtonPressed();
    }

    public void ResetGame()
    {
        IsPlaying = false;
        WaitingForFirstClick = false;

        if (fly != null)
            fly.ResetFly();

        if (spawner != null)
        {
            spawner.enabled = false;
            spawner.ResetSpawner();
        }

        if (DopamineManager.Instance != null)
        {
            Debug.Log("[FlyGame] Turning dopamine OFF");
            DopamineManager.Instance.SetPlayingOnPhone(false);
            DopamineManager.Instance.ResetPhoneGameMultiplier();
        }

        ClearExistingSwatters();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (startPromptPanel != null)
            startPromptPanel.SetActive(false);

        score = 0;
        UpdateUI();

        Debug.Log("[FlyGame] ResetGame()");
    }

    public void GameOver()
    {
        if (!IsPlaying) return;

        IsPlaying = false;
        WaitingForFirstClick = false;

        if (fly != null)
            fly.StopFly();

        if (spawner != null)
            spawner.enabled = false;

        if (DopamineManager.Instance != null)
        {
            Debug.Log("[FlyGame] Turning dopamine OFF");
            DopamineManager.Instance.SetPlayingOnPhone(false);
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        float mult = 1f;
        if (DopamineManager.Instance != null)
            mult = DopamineManager.Instance.GetPhoneGameMultiplier();

        Debug.Log("[FlyGame] Game Over | Score: " + score + " | Multiplier: " + mult + "x");
    }

    public void AddScore()
    {
        if (!IsPlaying) return;

        score++;

        if (DopamineManager.Instance != null)
            DopamineManager.Instance.IncreasePhoneGameMultiplier();

        if (spawner != null)
            spawner.IncreaseSpawnSpeed();

        UpdateUI();

        float mult = 1f;
        if (DopamineManager.Instance != null)
            mult = DopamineManager.Instance.GetPhoneGameMultiplier();

        Debug.Log("[FlyGame] Score: " + score + " | Multiplier: " + mult + "x");
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            float mult = 1f;
            if (DopamineManager.Instance != null)
                mult = DopamineManager.Instance.GetPhoneGameMultiplier();

            scoreText.text = "Score: " + score + "\nMultiplier: " + mult.ToString("0.00") + "x";
        }
    }

    void ClearExistingSwatters()
    {
        SwatterPairMover[] swatters = FindObjectsByType<SwatterPairMover>(FindObjectsSortMode.None);
        foreach (SwatterPairMover s in swatters)
            Destroy(s.gameObject);
    }

    void OnDisable()
    {
        ResetGame();
    }
}