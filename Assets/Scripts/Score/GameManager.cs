using UnityEngine;
using UnityEngine.VFX;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Timer")]
    public float startTimeSeconds = 300f;
    public TMP_Text timerText;

    private float timeRemaining;
    private bool timerRunning = true;

    [Header("Score")]
    public TMP_Text scoreText;
    public int scorePerRide = 100;
    private int score = 0;

    [Header("Ride Stats")]
    public int ridesCompleted = 0;

    [Header("End Screen")]
    public GameObject endScreenPanel;
    public TMP_Text finalScoreText;

    [Header("Saving")]
    public SaveJSONData saveSystem;

    [Header("Driving Stats")]
    public Transform player;
    public float totalDistanceDriven = 0f;
    private Vector3 lastPosition;
    public float averageSpeed = 0f;

    [Header("Win VFX")]
    public VisualEffect confettiVFX;
    public float confettiPlaySeconds = 2f; // let it play before freezing

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        timeRemaining = startTimeSeconds;
        UpdateTimerUI();
        UpdateScoreUI();

        if (endScreenPanel != null)
            endScreenPanel.SetActive(false);

        Time.timeScale = 1f;

        if (player != null)
            lastPosition = player.position;
    }

    private void Update()
    {
        if (!timerRunning) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            timerRunning = false;
            UpdateTimerUI();
            OnTimeUp();
        }
        else
        {
            UpdateTimerUI();
        }

        if (player != null)
        {
            float frameDistance = Vector3.Distance(player.position, lastPosition);
            totalDistanceDriven += frameDistance;
            lastPosition = player.position;
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        timerText.text = $"{minutes:0}:{seconds:00}";
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    public void AddRideScore()
    {
        score += scorePerRide;
        ridesCompleted++;

        if (ScorePopupUI.Instance != null)
            ScorePopupUI.Instance.ShowPopup(scorePerRide);

        Debug.Log("Total Score: " + score + " | Rides: " + ridesCompleted);
        UpdateScoreUI();
    }

    private void OnTimeUp()
    {
        Debug.Log("Time’s up!");

        // Calculate average speed
        float timePlayed = GetTimePlayed();
        averageSpeed = (timePlayed > 0f) ? (totalDistanceDriven / timePlayed) : 0f;

        Debug.Log($"Driving stats: distance={totalDistanceDriven:F1}m, avgSpeed={averageSpeed:F2} m/s");

        // Save
        if (saveSystem != null) saveSystem.SaveDataNow();
        else Debug.LogWarning("GameManager: saveSystem is null, not saving.");

        // Show end screen
        if (endScreenPanel != null)
            endScreenPanel.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = "Score: " + score;

        // Play confetti NOW (before freeze)
        if (confettiVFX != null)
            confettiVFX.Play();

        // Freeze after a short delay so VFX can play
        StartCoroutine(FreezeAfter(confettiPlaySeconds));
    }

    private System.Collections.IEnumerator FreezeAfter(float delay)
    {
        // Use unscaled time so it still counts even if timeScale changes later
        float t = 0f;
        while (t < delay)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public float GetTimePlayed()
    {
        return startTimeSeconds - timeRemaining;
    }

    public TaxiGameData CreateSaveData()
    {
        return new TaxiGameData(
            score,
            ridesCompleted,
            GetTimePlayed(),
            averageSpeed
        );
    }
}
