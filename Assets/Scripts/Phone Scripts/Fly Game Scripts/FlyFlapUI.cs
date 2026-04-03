using UnityEngine;
using UnityEngine.UI;

public class FlyFlapUI : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite passiveSprite;
    public Sprite flyingSprite;
    public Sprite hitSprite;

    [Header("Audio")]
    public AudioSource flapAudioSource;
    public AudioClip flapClip;

    [Header("Movement (UI space)")]
    public float flapVelocity = 420f;
    public float gravity = 1200f;
    public float maxFallSpeed = 1400f;

    [Header("Look")]
    public float flyingSpriteTime = 0.08f;

    [Header("Refs")]
    public RectTransform gameArea;

    [Header("Bounds")]
    public bool useBounds = true;
    public float topBoundaryY = 180f;
    public float bottomBoundaryY = -180f;

    private RectTransform rt;
    private Image img;
    private Vector2 startPos;
    private float velY;
    private bool dead;
    private float flyTimer;
    private bool isPlaying;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        img = GetComponent<Image>();
        startPos = rt.anchoredPosition;
    }

    void Update()
    {
        if (!isPlaying || dead)
        {
            if (!dead && img != null && passiveSprite != null)
                img.sprite = passiveSprite;
            return;
        }

        if (Input.GetMouseButtonDown(0))
            Flap();

        velY -= gravity * Time.deltaTime;
        velY = Mathf.Max(velY, -maxFallSpeed);
        rt.anchoredPosition += new Vector2(0f, velY * Time.deltaTime);

        if (flyTimer > 0f)
        {
            flyTimer -= Time.deltaTime;
            if (flyTimer <= 0f && passiveSprite != null)
                img.sprite = passiveSprite;
        }

        if (useBounds)
        {
            float y = rt.anchoredPosition.y;

            if (y > topBoundaryY)
            {
                Vector2 p = rt.anchoredPosition;
                p.y = topBoundaryY;
                rt.anchoredPosition = p;
                Hit();
            }
            else if (y < bottomBoundaryY)
            {
                Vector2 p = rt.anchoredPosition;
                p.y = bottomBoundaryY;
                rt.anchoredPosition = p;
                Hit();
            }
        }
    }

    public void StartFly()
    {
        isPlaying = true;
        dead = false;
        velY = 0f;
        rt.anchoredPosition = startPos;

        if (img != null && passiveSprite != null)
            img.sprite = passiveSprite;
    }

    public void StopFly()
    {
        isPlaying = false;
    }

    public void ResetFly()
    {
        dead = false;
        isPlaying = false;
        velY = 0f;
        flyTimer = 0f;
        rt.anchoredPosition = startPos;

        if (img != null && passiveSprite != null)
            img.sprite = passiveSprite;
    }

    void Flap()
    {
        velY = flapVelocity;

        if (img != null && flyingSprite != null)
            img.sprite = flyingSprite;

        flyTimer = flyingSpriteTime;

        PlayFlapSound();
    }

    public void ForceFlap()
    {
        if (dead) return;

        velY = flapVelocity;

        if (img != null && flyingSprite != null)
            img.sprite = flyingSprite;

        flyTimer = flyingSpriteTime;

        PlayFlapSound();
    }

    void PlayFlapSound()
    {
        if (flapAudioSource != null && flapClip != null)
        {
            flapAudioSource.pitch = Random.Range(0.96f, 1.04f);
            flapAudioSource.PlayOneShot(flapClip);
            flapAudioSource.pitch = 1f;
        }
    }

    public void Hit()
    {
        Debug.Log("[Fly] Hit() called");

        if (dead)
        {
            Debug.Log("[Fly] Already dead, ignoring Hit()");
            return;
        }

        dead = true;
        Debug.Log("[Fly] dead set to true");

        if (img != null && hitSprite != null)
            img.sprite = hitSprite;

        if (FlyMinigameManager.Instance != null)
        {
            Debug.Log("[Fly] Calling GameOver()");
            FlyMinigameManager.Instance.GameOver();
        }
        else
        {
            Debug.LogWarning("[Fly] FlyMinigameManager.Instance is NULL");
        }
    }

    public bool IsAlive => !dead;
}