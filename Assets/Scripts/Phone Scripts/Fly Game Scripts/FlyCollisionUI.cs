using UnityEngine;

public class FlyCollisionUI : MonoBehaviour
{
    public FlyFlapUI fly;

    void Update()
    {
        if (fly == null)
        {
            Debug.LogWarning("[Collision] fly is NULL");
            return;
        }

        if (!fly.IsAlive) return;
        if (FlyMinigameManager.Instance == null || !FlyMinigameManager.Instance.IsPlaying) return;

        RectTransform liveFlyRect = fly.GetComponent<RectTransform>();
        if (liveFlyRect == null)
        {
            Debug.LogWarning("[Collision] liveFlyRect is NULL");
            return;
        }

        var movers = FindObjectsByType<SwatterPairMover>(FindObjectsSortMode.None);

        foreach (var m in movers)
        {
            if (m.topHitbox == null) Debug.LogWarning("[Collision] topHitbox NULL on " + m.name);
            if (m.bottomHitbox == null) Debug.LogWarning("[Collision] bottomHitbox NULL on " + m.name);

            if (m.topHitbox != null && Overlaps(liveFlyRect, m.topHitbox))
            {
                Debug.Log("[Collision] Hit TOP");
                fly.Hit();
                return;
            }

            if (m.bottomHitbox != null && Overlaps(liveFlyRect, m.bottomHitbox))
            {
                Debug.Log("[Collision] Hit BOTTOM");
                fly.Hit();
                return;
            }
        }
    }

    bool Overlaps(RectTransform a, RectTransform b)
    {
        Rect ra = GetWorldRect(a);
        Rect rb = GetWorldRect(b);

        bool hit = ra.Overlaps(rb);

        if (hit)
            Debug.Log("[Collision] Overlap detected between " + a.name + " and " + b.name);

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