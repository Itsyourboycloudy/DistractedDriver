using UnityEngine;

// Legacy prefab references are retained. SwatterPairMover alone owns collision,
// and FlyMinigameManager owns death audio so one collision cannot fire twice.
public class FlyCollisionUI : MonoBehaviour
{
    public FlyFlapUI fly;
    public AudioSource sfxSource;
    public AudioClip hitClip;
}
