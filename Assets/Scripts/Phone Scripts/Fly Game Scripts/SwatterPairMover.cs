using UnityEngine;

public class SwatterPairMover : MonoBehaviour
{
    public RectTransform topSwatter;
    public RectTransform bottomSwatter;
    public RectTransform topHitbox;
    public RectTransform bottomHitbox;
    public RectTransform scoreZone;

    public float moveSpeed = 50f;
    public float destroyPadding = 120f;

    private RectTransform rt;
    private RectTransform parent;
    private bool scored;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        parent = rt.parent as RectTransform;
    }

    void Update()
    {
        if (FlyMinigameManager.Instance == null || !FlyMinigameManager.Instance.IsPlaying)
            return;

        Vector2 p = rt.anchoredPosition;
        p.x += moveSpeed * Time.deltaTime;
        rt.anchoredPosition = p;

        CheckHit();
        CheckScore();

        float rightEdgeX = parent.rect.width * 0.5f;
        if (p.x > rightEdgeX + destroyPadding)
            Destroy(gameObject);
    }

    void CheckHit()
    {
        if (FlyMinigameManager.Instance == null || FlyMinigameManager.Instance.fly == null)
        {
            Debug.LogWarning("[Swatter] No fly ref");
            return;
        }

        FlyFlapUI fly = FlyMinigameManager.Instance.fly;
        RectTransform flyRect = fly.GetComponent<RectTransform>();

        if (flyRect == null)
        {
            Debug.LogWarning("[Swatter] flyRect NULL");
            return;
        }

        if (topHitbox == null) Debug.LogWarning("[Swatter] topHitbox NULL");
        if (bottomHitbox == null) Debug.LogWarning("[Swatter] bottomHitbox NULL");

        if (topHitbox != null)
        {
            bool topHit = Overlaps(flyRect, topHitbox);
            if (topHit)
            {
                Debug.Log("[Swatter] TOP overlap detected, calling Hit()");
                fly.Hit();
                return;
            }
        }

        if (bottomHitbox != null)
        {
            bool bottomHit = Overlaps(flyRect, bottomHitbox);
            if (bottomHit)
            {
                Debug.Log("[Swatter] BOTTOM overlap detected, calling Hit()");
                fly.Hit();
                return;
            }
        }
    }

    void CheckScore()
    {
        if (scored || scoreZone == null || FlyMinigameManager.Instance == null || FlyMinigameManager.Instance.fly == null)
            return;

        RectTransform flyRect = FlyMinigameManager.Instance.fly.GetComponent<RectTransform>();
        if (flyRect == null) return;

        if (Overlaps(flyRect, scoreZone))
        {
            scored = true;
            Debug.Log("[Swatter] Passed ScoreZone");
            FlyMinigameManager.Instance.AddScore();
        }
    }

    bool Overlaps(RectTransform a, RectTransform b)
    {
        Rect ra = GetWorldRect(a);
        Rect rb = GetWorldRect(b);

        bool hit = ra.Overlaps(rb);

        if (hit)
            Debug.Log("[Swatter] Overlap: " + a.name + " with " + b.name);

        return hit;
    }

    Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        return new Rect(
            corners[0].x,
            corners[0].y,
            corners[2].x - corners[0].x,
            corners[2].y - corners[0].y
        );
    }
}