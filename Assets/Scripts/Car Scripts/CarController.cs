using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Speed Settings")]
    public float moveSpeed = 22f;
    public float acceleration = 8f;
    public float brakeForce = 10f;

    [Header("Steering")]
    public float turnSpeed = 120f;
    public float driftTurnMultiplier = 1.6f;
    public float driftSideFriction = 0.5f;

    [Header("Drift (Boost Charge)")]
    public KeyCode driftKey = KeyCode.LeftShift;

    [Tooltip("How many boost levels (1 sec each).")]
    public int boostLevels = 5;

    [Tooltip("Seconds per level.")]
    public float secondsPerLevel = 1f;

    [Tooltip("How long the boost lasts after releasing drift.")]
    public float boostDuration = 1f;

    [Tooltip("Extra speed added per level (in m/s).")]
    public float[] boostSpeedAddByLevel = new float[5] { 2f, 3.5f, 5f, 6.5f, 8f };

    [Tooltip("Optional: tiny control slowdown while drifting.")]
    public float driftForwardSlowPercent = 0.08f;

    [Header("Charge SFX")]
    [Tooltip("Delay before playing level 1 charge sound after you start holding drift.")]
    public float firstChargeSoundDelay = 0.20f;

    [Tooltip("When you reach the max level and keep holding, replay max level sound every X seconds.")]
    public float maxLevelLoopSfxInterval = 0.75f;

    [Header("Boost Sounds (5 levels)")]
    public AudioSource boostAudioSource;
    public AudioClip[] boostClipsByLevel = new AudioClip[5]; // assign 5 clips

    [Header("Skid")]
    public float skidThreshold = 8f;
    public ParticleSystem skidParticles;
    public AudioSource skidSound;

    private Rigidbody rb;
    private float currentSpeed = 0f;
    private bool isDrifting = false;

    // Dopamine speed boost
    private float dopamineBoost = 1f;

    // Drift charge
    private float driftHoldTime = 0f;
    private int chargedLevel = 0; // 0..5

    // Charge SFX state
    private int lastPlayedChargeLevel = 0;   // last level we played a charge sound for
    private float maxLevelSfxTimer = 0f;     // timer to loop lvl 5 sound

    // Active boost
    private float boostTimer = 0f;
    private float activeBoostAdd = 0f;
    public AudioSource engineAudio;
    public float minPitch = 0.8f;
    public float maxPitch = 1.3f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (boostSpeedAddByLevel == null || boostSpeedAddByLevel.Length < boostLevels)
            Debug.LogWarning("boostSpeedAddByLevel needs at least 'boostLevels' elements.");

        if (boostClipsByLevel == null || boostClipsByLevel.Length < boostLevels)
            Debug.LogWarning("boostClipsByLevel needs at least 'boostLevels' elements.");
    }

    void Update()
    {
        float speedPercent = rb.linearVelocity.magnitude / moveSpeed;
        engineAudio.pitch = Mathf.Lerp(minPitch, maxPitch, speedPercent);
        if (DopamineManager.Instance != null)
            dopamineBoost = DopamineManager.Instance.GetSpeedMultiplier();
        else
            dopamineBoost = 1f;

        HandleMovement();
        HandleSteering();
        HandleDriftChargeAndBoost();
        HandleSkidEffects();
    }

    void HandleMovement()
    {
        float vertical = Input.GetAxis("Vertical");

        float boostedMoveSpeed = moveSpeed * dopamineBoost;

        // boost countdown
        if (boostTimer > 0f)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                boostTimer = 0f;
                activeBoostAdd = 0f;
            }
        }

        float finalTopSpeed = boostedMoveSpeed + activeBoostAdd;

        if (!isDrifting)
        {
            if (vertical != 0)
                currentSpeed = Mathf.Lerp(currentSpeed, vertical * finalTopSpeed, Time.deltaTime * acceleration);
            else
                currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * brakeForce);
        }
        else
        {
            if (vertical != 0)
            {
                float target = vertical * finalTopSpeed;
                target *= (1f - driftForwardSlowPercent);
                currentSpeed = Mathf.Lerp(currentSpeed, target, Time.deltaTime * acceleration);
            }
            else
            {
                currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * brakeForce);
            }
        }
    }

    void HandleSteering()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float steerMultiplier = isDrifting ? driftTurnMultiplier : 1f;

        transform.Rotate(Vector3.up * horizontal * turnSpeed * steerMultiplier * Time.deltaTime);
    }

    void HandleDriftChargeAndBoost()
    {
        if (Input.GetKey(driftKey))
        {
            // first frame of drift start
            if (!isDrifting)
            {
                isDrifting = true;
                driftHoldTime = 0f;
                chargedLevel = 0;

                // reset charge SFX state
                lastPlayedChargeLevel = 0;
                maxLevelSfxTimer = 0f;
            }

            // slide physics
            Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);
            localVel.x = Mathf.Lerp(localVel.x, localVel.x * driftSideFriction, Time.deltaTime * 5f);
            rb.linearVelocity = transform.TransformDirection(localVel);

            // charge time
            driftHoldTime += Time.deltaTime;

            // compute level (1..5)
            int level = Mathf.FloorToInt(driftHoldTime / secondsPerLevel) + 1;
            chargedLevel = Mathf.Clamp(level, 1, boostLevels);

            // play charge SFX while holding (with delay on first)
            HandleChargeSfxWhileHolding();
        }
        else
        {
            if (isDrifting)
            {
                TriggerBoost(chargedLevel);
            }

            isDrifting = false;
            driftHoldTime = 0f;
            chargedLevel = 0;

            // reset SFX
            lastPlayedChargeLevel = 0;
            maxLevelSfxTimer = 0f;
        }
    }

    void HandleChargeSfxWhileHolding()
    {
        if (boostAudioSource == null || boostClipsByLevel == null) return;

        // Delay the first one so quick taps don’t chirp instantly
        bool firstAllowed = driftHoldTime >= firstChargeSoundDelay;

        // If we entered a new level, play it once
        if (firstAllowed && chargedLevel > lastPlayedChargeLevel)
        {
            PlayLevelClip(chargedLevel);
            lastPlayedChargeLevel = chargedLevel;

            // if we just hit max, reset loop timer so it doesn't instantly double-play
            if (chargedLevel >= boostLevels)
                maxLevelSfxTimer = 0f;
        }

        // If we're at max level and still holding, loop the max level sound
        if (chargedLevel >= boostLevels && firstAllowed)
        {
            maxLevelSfxTimer += Time.deltaTime;
            if (maxLevelSfxTimer >= maxLevelLoopSfxInterval)
            {
                maxLevelSfxTimer = 0f;
                PlayLevelClip(boostLevels);
            }
        }
    }

    void PlayLevelClip(int level)
    {
        int idx = Mathf.Clamp(level - 1, 0, boostLevels - 1);
        if (idx >= boostClipsByLevel.Length) return;

        AudioClip clip = boostClipsByLevel[idx];
        if (clip != null)
            boostAudioSource.PlayOneShot(clip);
    }

    void TriggerBoost(int level)
    {
        if (level <= 0) return;

        int idx = Mathf.Clamp(level - 1, 0, boostLevels - 1);

        float add = (boostSpeedAddByLevel != null && boostSpeedAddByLevel.Length > idx)
            ? boostSpeedAddByLevel[idx]
            : 0f;

        activeBoostAdd = add;
        boostTimer = boostDuration;

        // OPTIONAL:
        // If you *don't* want the boost sound here (because charge sound already played),
        // comment this out. If you DO want a second sound on release, keep it.
        //PlayLevelClip(level);
    }

    void HandleSkidEffects()
    {
        float sidewaysSpeed = Mathf.Abs(transform.InverseTransformDirection(rb.linearVelocity).x);
        bool isSkidding = sidewaysSpeed > skidThreshold;

        if (isSkidding)
        {
            if (skidParticles != null && !skidParticles.isPlaying)
                skidParticles.Play();

            if (skidSound != null && !skidSound.isPlaying)
                skidSound.Play();
        }
        else
        {
            if (skidParticles != null && skidParticles.isPlaying)
                skidParticles.Stop();

            if (skidSound != null && skidSound.isPlaying)
                skidSound.Stop();
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = transform.forward * currentSpeed;
    }
}
