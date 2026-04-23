using UnityEngine;
using UnityEngine.Audio;

public class PauseAudioManager : MonoBehaviour
{
    public static PauseAudioManager Instance { get; private set; }

    [Header("Mixers")]
    public AudioMixer masterMixer;
    public AudioMixer worldMusicMixer;

    [Header("Exposed Parameters")]
    public string masterLowpassParameter = "MasterLowpassCutoff";
    public string worldMusicLowpassParameter = "WorldMusicLowpassCutoff";

    [Header("Lowpass Values")]
    public float normalCutoff = 22000f;
    public float pausedCutoff = 700f;

    [Header("Pause Key")]
    public KeyCode pauseKey = KeyCode.Escape;

    [Header("UI")]
    public PauseMenuUI pauseMenuUI;

    public bool IsPaused { get; private set; }

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
        SetPaused(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            SetPaused(!IsPaused);
        }
    }

    public void SetPaused(bool paused)
    {
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;

        if (masterMixer != null)
            masterMixer.SetFloat(masterLowpassParameter, paused ? pausedCutoff : normalCutoff);

        if (worldMusicMixer != null)
            worldMusicMixer.SetFloat(worldMusicLowpassParameter, paused ? pausedCutoff : normalCutoff);

        if (pauseMenuUI != null)
        {
            if (paused)
                pauseMenuUI.ShowPauseMenu();
            else
                pauseMenuUI.HidePauseMenuImmediate();
        }
    }
}