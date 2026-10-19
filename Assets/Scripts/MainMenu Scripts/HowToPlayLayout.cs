using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Uses the scene's font and back button, with a scrollable responsive reading area.
public class HowToPlayLayout : MonoBehaviour
{
    public Canvas canvas;
    public TMP_Text title;
    public TMP_Text subtitle;
    public TMP_Text textTemplate;
    public Button backButton;
    public GameObject[] oldPanels;
    RectTransform safeRoot, viewport, content;
    ScrollRect scroll;
    readonly System.Collections.Generic.List<TMP_Text> cards = new();
    Vector2 lastSize;
    Rect lastSafeArea;

    static readonly string[] Guide =
    {
        "<color=#FFD34E><b>01  DRIVE & BOOST</b></color>\n\n<b>W / S</b>  Accelerate / reverse\n<b>A / D</b>  Steer\n<b>Left Shift</b>  Hold to drift and charge; release for a boost. Longer charges give stronger boosts.\n\n<b>Esc</b>  Pause, audio options and owned upgrades.",
        "<color=#FFD34E><b>02  COMPLETE A FARE</b></color>\n\nOpen the <b>TAXI</b> app and choose <b>ACCEPT</b> on a passenger offer.\n\nFollow the GPS arrow and enter the pickup zone. Then follow it to the drop-off zone to finish the ride and earn cash.\n\nCheck the preference before accepting: <b>SPEED</b> rewards beating the deadline, <b>SMOOTH</b> rewards gentle driving, and <b>GAMER</b> rewards 5 phone points during the ride. Earn up to +50% as a tip.",
        "<color=#FFD34E><b>03  PHONE & DOPAMINE</b></color>\n\nClick the phone apps to use them. In <b>Fly Away</b>, left-click to flap and avoid swatters. In <b>Whack-a-Fly</b>, click flies as they rise from the pipes: 45 seconds, 3 lives. Use <b>NEXT GAME</b> to switch.\n\nFly Away builds dopamine during play. Whack-a-Fly rewards successful hits. Scoring increases the gain multiplier. Dopamine drains over time.\n\n<b>Higher dopamine means a faster taxi.</b> Split your attention between the road and the phone.",
        "<color=#FFD34E><b>04  BUILD YOUR TAXI</b></color>\n\nOpen <b>SHOP</b>, hover an upgrade to read its effect and price, then click to buy.\n\n<b>Drift Kit</b>: stronger, longer boosts; lower cruising speed. <b>Dopamine Rig</b>: faster dopamine gain; faster decay and lower fares. <b>Premium Meter</b>: higher fares; slower steering and dopamine gain. Each stacks up to 3 times. The shop refreshes each day.\n\nAfter unlocking <b>Time Stop</b>, press <b>E</b> to pause the day clock briefly, once per day.",
        "<color=#FFD34E><b>05  PAY THE DAILY DEBT</b></color>\n\nEarn enough cash before the day ends. Your debt is deducted automatically at day-end.\n\nIf you can pay, the next day begins with a higher debt target. If you cannot, the day fails.\n\n<b>Keep money for debt:</b> buying an upgrade spends the same cash you need to survive.",
        "<color=#FFD34E><b>06  YOUR FIRST RUN</b></color>\n\nThe Main Menu <b>TUTORIAL</b> opens a practice run without changing your save. Follow the on-screen tutorial: buy the highlighted upgrade, accept a fare, collect the passenger and deliver them. <b>F8</b> skips the tutorial.\n\nProgress saves during play and when returning to the menu. <b>PLAY / CONTINUE</b> loads your run.\n\n<b>NEW GAME</b> starts over. If a save exists, confirm with a second click within four seconds."
    };

    void Start() { Build(); }

    public void Build()
    {
        if (safeRoot != null) return;
        var background = canvas.transform.Find("Background")?.GetComponent<Image>();
        if (background != null) { background.sprite = null; background.color = new Color(0.025f, 0.035f, 0.055f, 1); }
        foreach (var panel in oldPanels) if (panel != null) panel.SetActive(false);
        safeRoot = Rect("Guide Safe Area", canvas.transform);
        title.transform.SetParent(safeRoot, false);
        subtitle.transform.SetParent(safeRoot, false);
        backButton.transform.SetParent(safeRoot, false);
        Format(title, 38, TextAlignmentOptions.Center);
        title.text = "HOW TO PLAY";
        Format(subtitle, 19, TextAlignmentOptions.Center);
        subtitle.text = "Earn fares. Feed your dopamine. Pay the debt.\nScroll to explore the guide.";
        viewport = Rect("Guide Viewport", safeRoot);
        viewport.gameObject.AddComponent<Image>().color = new Color(0.025f, 0.035f, 0.055f, 0.95f);
        viewport.gameObject.AddComponent<RectMask2D>();
        scroll = viewport.gameObject.AddComponent<ScrollRect>();
        content = Rect("Guide Cards", viewport);
        scroll.viewport = viewport;
        scroll.content = content;
        scroll.horizontal = false;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 45;
        content.anchorMin = new Vector2(0, 1);
        content.anchorMax = Vector2.one;
        content.pivot = new Vector2(0.5f, 1);
        foreach (string copy in Guide)
        {
            var card = Rect("Guide Card", content);
            card.gameObject.AddComponent<Image>().color = new Color(0.07f, 0.09f, 0.13f, 1);
            var label = Instantiate(textTemplate, card);
            label.gameObject.SetActive(true);
            Format(label, 22, TextAlignmentOptions.TopLeft);
            label.text = copy;
            Stretch(label.rectTransform, new Vector2(24, 24), new Vector2(-24, -24));
            cards.Add(label);
        }
        Format(backButton.GetComponentInChildren<TMP_Text>(true), 22, TextAlignmentOptions.Center);
        Stretch(backButton.GetComponentInChildren<TMP_Text>(true).rectTransform, new Vector2(12, 6), new Vector2(-12, -6));
        var image = backButton.GetComponent<Image>();
        if (image != null) { image.sprite = null; image.color = new Color(0.16f, 0.19f, 0.25f); }
        var colors = backButton.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 0.84f, 0.4f);
        colors.selectedColor = colors.highlightedColor;
        colors.pressedColor = new Color(0.7f, 0.75f, 0.8f);
        backButton.colors = colors;
        RefreshLayout();
        if (UnityEngine.EventSystems.EventSystem.current != null)
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(backButton.gameObject);
    }

    void Update()
    {
        if (scroll == null) return;
        if (Input.GetKeyDown(KeyCode.Escape)) backButton.onClick.Invoke();
        float direction = (Input.GetKey(KeyCode.UpArrow) ? 1f : 0f) - (Input.GetKey(KeyCode.DownArrow) ? 1f : 0f);
        float distance = Mathf.Max(1, content.rect.height - viewport.rect.height);
        if (direction != 0) scroll.verticalNormalizedPosition = Mathf.Clamp01(scroll.verticalNormalizedPosition + direction * Time.unscaledDeltaTime * 500 / distance);
    }

    void LateUpdate()
    {
        if (safeRoot != null && (lastSize != new Vector2(Screen.width, Screen.height) || lastSafeArea != Screen.safeArea))
            RefreshLayout();
    }

    public void RefreshLayout()
    {
        RefreshLayout(new Vector2(Screen.width, Screen.height), Screen.safeArea);
    }

    public void RefreshLayout(Vector2 screenSize, Rect safeArea)
    {
        var scaler = canvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(screenSize.x < screenSize.y ? 480 : 1280, 720);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        // CanvasScaler normally updates next frame; apply its Expand formula now as well.
        canvas.scaleFactor = Mathf.Min(screenSize.x / scaler.referenceResolution.x, screenSize.y / scaler.referenceResolution.y);
        lastSize = screenSize;
        lastSafeArea = safeArea;
        safeRoot.anchorMin = lastSafeArea.min / lastSize;
        safeRoot.anchorMax = lastSafeArea.max / lastSize;
        safeRoot.offsetMin = safeRoot.offsetMax = Vector2.zero;
        Canvas.ForceUpdateCanvases();
        float width = Mathf.Min(safeRoot.rect.width - 32, 1400);
        Place(title.rectTransform, new Vector2(0.5f, 1), new Vector2(0, -28), new Vector2(width, 48));
        Place(subtitle.rectTransform, new Vector2(0.5f, 1), new Vector2(0, -83), new Vector2(width, 56));
        Stretch(viewport, new Vector2((safeRoot.rect.width - width) / 2, 82), new Vector2(-(safeRoot.rect.width - width) / 2, -124));
        Place((RectTransform)backButton.transform, new Vector2(0.5f, 0), new Vector2(0, 40), new Vector2(Mathf.Min(width, 320), 52));
        int columns = width >= 1000 ? 3 : width >= 700 ? 2 : 1;
        float gap = 16;
        float cardWidth = (width - gap * (columns - 1)) / columns;
        float y = 0;
        for (int row = 0; row < cards.Count; row += columns)
        {
            float height = 0;
            for (int i = row; i < Mathf.Min(row + columns, cards.Count); i++)
                height = Mathf.Max(height, cards[i].GetPreferredValues(cards[i].text, cardWidth - 48, Mathf.Infinity).y + 56);
            for (int i = row; i < Mathf.Min(row + columns, cards.Count); i++)
            {
                var rect = (RectTransform)cards[i].transform.parent;
                Place(rect, new Vector2(0, 1), new Vector2((i - row) * (cardWidth + gap) + cardWidth / 2, -y - height / 2), new Vector2(cardWidth, height));
            }
            y += height + gap;
        }
        content.sizeDelta = new Vector2(0, y - gap);
        Canvas.ForceUpdateCanvases();
        scroll.verticalNormalizedPosition = 1;
    }

    static RectTransform Rect(string name, Transform parent)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.gameObject.layer = parent.gameObject.layer;
        return rect;
    }
    static void Format(TMP_Text text, float size, TextAlignmentOptions alignment)
    {
        text.transform.localScale = Vector3.one;
        text.transform.localRotation = Quaternion.identity;
        text.fontSize = size;
        text.fontStyle = FontStyles.Normal;
        text.enableAutoSizing = false;
        text.color = new Color(0.94f, 0.95f, 0.98f);
        text.alignment = alignment;
        text.margin = Vector4.zero;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.overflowMode = TextOverflowModes.Overflow;
        text.raycastTarget = false;
    }
    static void Place(RectTransform rect, Vector2 anchor, Vector2 position, Vector2 size)
    {
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
        rect.anchorMin = rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
    }
    static void Stretch(RectTransform rect, Vector2 min, Vector2 max)
    {
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = min;
        rect.offsetMax = max;
    }
}
