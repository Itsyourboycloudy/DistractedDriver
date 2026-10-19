using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuButtons : MonoBehaviour
{
    TMP_Text newGameLabel;
    float confirmUntil;
    Button[] menuButtons;
    MenuHandCursor cursor;
    CanvasScaler menuScaler;

    public void PlayGame() { Time.timeScale = 1f; SceneManager.LoadScene("GameScene"); }
    public void HowToPlay() { Time.timeScale = 1f; SceneManager.LoadScene("HowToPlay"); }
    public void BackToMenu() { Time.timeScale = 1f; SceneManager.LoadScene("MainMenu"); }
    public void QuitGame() { Application.Quit(); }

    void Start() { SaveJSONData.EndTutorial(); ConfigureMenu(); }
    public void Tutorial() { SaveJSONData.StartTutorial(); }
    public void ConfigureMenu()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu" || menuButtons != null) return;
        Button play = null;
        foreach (var button in FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (button.name == "PlayButton") { play = button; break; }
        if (play == null) return;
        menuScaler = play.GetComponentInParent<CanvasScaler>(true);
        var parent = play.transform.parent;
        // Keep the zoomed artwork in its authored coordinate system. Stretching it
        // independently on each axis moves the phone away from its button column.
        var background = parent.parent.Find("MainMenuBackground") as RectTransform;
        if (background != null)
        {
            background.anchorMin = background.anchorMax = new Vector2(0.5f, 0.5f);
            background.sizeDelta = new Vector2(3840, 2160);
        }
        var newGame = Instantiate(play, parent);
        newGame.name = "NewGameButton";
        newGame.onClick = new Button.ButtonClickedEvent();
        newGame.onClick.AddListener(ConfirmNewGame);
        var tutorial = Instantiate(play, parent);
        tutorial.name = "TutorialButton";
        tutorial.onClick = new Button.ButtonClickedEvent();
        tutorial.onClick.AddListener(Tutorial);
        menuButtons = new[] { play, newGame, tutorial, parent.Find("HowToPlayButton").GetComponent<Button>(), parent.Find("QuitButton").GetComponent<Button>() };
        string[] labels = { SaveJSONData.Progress.hasRun ? "CONTINUE" : "PLAY", "NEW GAME", "TUTORIAL", "HOW TO PLAY", "QUIT" };
        for (int i = 0; i < menuButtons.Length; i++)
        {
            var button = menuButtons[i];
            var rect = (RectTransform)button.transform;
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;
            rect.anchorMin = new Vector2(0.14f, 0.72f - i * 0.13f);
            rect.anchorMax = new Vector2(0.86f, 0.83f - i * 0.13f);
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            var label = button.GetComponentInChildren<TMP_Text>(true);
            var lr = label.rectTransform;
            lr.localRotation = Quaternion.identity;
            // Counter the artwork's non-uniform scale, while keeping text inside the button.
            float aspectCorrection = parent.localScale.x / parent.localScale.y;
            lr.localScale = new Vector3(1, aspectCorrection, 1);
            lr.anchorMin = Vector2.zero;
            lr.anchorMax = Vector2.one;
            lr.offsetMin = new Vector2(2, 0);
            lr.offsetMax = new Vector2(-2, 0);
            label.margin = Vector4.zero;
            label.text = labels[i];
            label.alignment = TextAlignmentOptions.Center;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.enableAutoSizing = true;
            label.fontSizeMin = 3;
            label.fontSizeMax = 8;
            label.overflowMode = TextOverflowModes.Ellipsis;
            label.raycastTarget = false;
            var navigation = new Navigation { mode = Navigation.Mode.Explicit };
            navigation.selectOnUp = menuButtons[(i + menuButtons.Length - 1) % menuButtons.Length];
            navigation.selectOnDown = menuButtons[(i + 1) % menuButtons.Length];
            button.navigation = navigation;
        }
        newGameLabel = newGame.GetComponentInChildren<TMP_Text>(true);
        // EventSystem owns keyboard submit, so a single Enter cannot confirm a reset twice.
        foreach (var hand in FindObjectsByType<MenuHandCursor>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            cursor = hand;
            hand.enabled = false;
            hand.buttons = menuButtons;
        }
        foreach (var button in menuButtons)
        {
            var target = button.GetComponent<MenuCursorTarget>();
            if (target != null) target.enabled = false;
        }
    }

    void Update()
    {
        if (confirmUntil > 0 && Time.unscaledTime > confirmUntil)
        {
            confirmUntil = 0;
            if (newGameLabel != null) newGameLabel.text = "NEW GAME";
        }
    }

    void LateUpdate()
    {
        if (menuScaler != null)
        {
            menuScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            menuScaler.referenceResolution = new Vector2(Screen.width < Screen.height ? 1920 : 3840, 2160);
        }
        if (menuButtons == null || !menuButtons[0].gameObject.activeInHierarchy || EventSystem.current == null) return;
        var selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null || System.Array.Find(menuButtons, b => b.gameObject == selected) == null)
        {
            selected = menuButtons[0].gameObject;
            EventSystem.current.SetSelectedGameObject(selected);
        }
        if (cursor == null || cursor.handCursor == null) return;
        var rect = selected.GetComponent<RectTransform>();
        // Local-space placement follows the column when the canvas resizes.
        Vector3 edge = rect.TransformPoint(new Vector3(rect.rect.xMin - 5, 0));
        cursor.handCursor.position = edge;
    }

    public void ConfirmNewGame()
    {
        if (!SaveJSONData.Progress.hasRun || (confirmUntil > 0 && Time.unscaledTime <= confirmUntil))
        { SaveJSONData.NewGame(); return; }
        confirmUntil = Time.unscaledTime + 4f;
        if (newGameLabel != null) newGameLabel.text = "RESET SAVE?";
    }
}
