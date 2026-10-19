using UnityEngine;

public class SwatterPairMover : MonoBehaviour
{
    public RectTransform topSwatter, bottomSwatter, topHitbox, bottomHitbox, scoreZone;
    public float moveSpeed = 50f;
    public float destroyPadding = 120f;
    RectTransform rect, playfield;
    bool scored;
    void Awake() { rect = (RectTransform)transform; playfield = rect.parent as RectTransform; }
    void Update()
    {
        var game = FlyMinigameManager.Instance;
        if (game == null || !game.IsPlaying || Time.timeScale == 0) return;
        rect.anchoredPosition += Vector2.right * (moveSpeed * Time.deltaTime);
        if (playfield != null && rect.anchoredPosition.x > playfield.rect.xMax + destroyPadding) Destroy(gameObject);
    }
    void LateUpdate()
    {
        var game = FlyMinigameManager.Instance;
        if (game == null || !game.IsPlaying || game.fly == null || !game.fly.IsAlive || Time.timeScale == 0) return;
        var space = game.gameArea != null ? game.gameArea : playfield;
        if (space == null || !transform.IsChildOf(space)) return;
        var fly = (RectTransform)game.fly.transform;
        if (PhoneRectCollision.Overlaps(fly, topHitbox, space) || PhoneRectCollision.Overlaps(fly, bottomHitbox, space))
        { game.fly.Hit(); return; }
        if (!scored && PhoneRectCollision.Overlaps(fly, scoreZone, space))
        { scored = true; game.AddScore(); }
    }
}
