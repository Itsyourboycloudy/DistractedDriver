using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class BeaconDistanceTrigger : MonoBehaviour
{
    [Header("References")]
    public Transform player;              // assign your Player/Car transform
    private AudioSource audioSource;

    [Header("Distance (meters)")]
    public float minDistance = 2f;        // closest distance you care about
    public float maxDistance = 40f;       // farthest distance you care about

    [Header("Trigger Interval (seconds)")]
    public float fastestInterval = 0.15f; // when player is very close (fast beeps)
    public float slowestInterval = 1.5f;  // when player is far (slow beeps)

    [Header("Smoothing")]
    public float intervalLerpSpeed = 6f;  // bigger = faster smoothing

    private float timer = 0f;
    private float currentInterval;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        currentInterval = slowestInterval;
    }

    void Update()
    {
        if (player == null || audioSource.clip == null)
            return;

        float d = Vector3.Distance(player.position, transform.position);

        // Normalize distance: 0 = close, 1 = far
        float t = Mathf.InverseLerp(minDistance, maxDistance, d);

        // Convert to interval: close -> fastest, far -> slowest
        float targetInterval = Mathf.Lerp(fastestInterval, slowestInterval, t);

        // Smooth it so it doesn't jitter if player wiggles
        currentInterval = Mathf.Lerp(currentInterval, targetInterval, Time.deltaTime * intervalLerpSpeed);

        timer += Time.deltaTime;
        if (timer >= currentInterval)
        {
            timer = 0f;
            audioSource.PlayOneShot(audioSource.clip);
        }
    }

    // Optional: see ranges in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minDistance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}