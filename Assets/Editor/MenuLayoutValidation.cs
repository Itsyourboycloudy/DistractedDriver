#if UNITY_EDITOR
using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class MenuLayoutValidation
{
    [MenuItem("Tools/Validate Menu Layouts")]
    public static void Run()
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        string output = Path.Combine(Path.GetDirectoryName(Application.dataPath), "MenuValidation");
        Directory.CreateDirectory(output);
        var setup = EditorSceneManager.GetSceneManagerSetup();
        int failures = 0;
        try
        {
            foreach (string sceneName in new[] { "MainMenu", "HowToPlay" })
            {
                EditorSceneManager.OpenScene("Assets/Scenes/" + sceneName + ".unity");
                var canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
                var camera = Camera.main;
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera;
                canvas.planeDistance = 1;
                var guide = UnityEngine.Object.FindFirstObjectByType<HowToPlayLayout>();
                if (guide != null) guide.Build();
                else
                {
                    UnityEngine.Object.FindFirstObjectByType<MenuButtons>().ConfigureMenu();
                    var flow = UnityEngine.Object.FindFirstObjectByType<MainMenuFlow>();
                    flow.introPanel.SetActive(false);
                    flow.mainMenuPanel.SetActive(true);
                    if (flow.fadeOverlay != null) flow.fadeOverlay.gameObject.SetActive(false);
                }
                canvas.GetComponent<CanvasScaler>().enabled = false;
                foreach (var size in new[] { new Vector2Int(1920,1080), new Vector2Int(1280,720), new Vector2Int(1024,768), new Vector2Int(2560,1080), new Vector2Int(640,360), new Vector2Int(390,844) })
                {
                    var target = new RenderTexture(size.x, size.y, 24);
                    camera.targetTexture = target;
                    canvas.scaleFactor = Mathf.Min(size.x / (size.x < size.y ? 1920f : 3840f), size.y / 2160f);
                    Canvas.ForceUpdateCanvases();
                    if (guide != null) guide.RefreshLayout(size, new Rect(0, 0, size.x, size.y));
                    Canvas.ForceUpdateCanvases();
                    if (guide == null) UnityEngine.Object.FindFirstObjectByType<MenuButtons>().SendMessage("LateUpdate");
                    foreach (var text in canvas.GetComponentsInChildren<TMP_Text>())
                    {
                        text.ForceMeshUpdate();
                        if (text.isTextOverflowing) { Debug.LogError(sceneName + " " + size + " text overflows: " + text.name); failures++; }
                    }
                    foreach (var button in canvas.GetComponentsInChildren<Button>())
                    {
                        var corners = new Vector3[4];
                        ((RectTransform)button.transform).GetWorldCorners(corners);
                        foreach (var corner in corners)
                        {
                            var point = camera.WorldToViewportPoint(corner);
                            if (point.x < -0.001 || point.x > 1.001 || point.y < -0.001 || point.y > 1.001)
                            { Debug.LogError(sceneName + " " + size + " button offscreen: " + button.name); failures++; break; }
                        }
                    }
                    if (guide == null)
                    {
                        var fresh = GameObject.Find("NewGameButton").GetComponentInChildren<TMP_Text>();
                        string before = fresh.text;
                        fresh.text = "RESET SAVE?";
                        fresh.ForceMeshUpdate();
                        if (fresh.isTextOverflowing) { Debug.LogError("New game confirmation overflows " + size); failures++; }
                        fresh.text = before;
                    }
                    camera.Render();
                    RenderTexture.active = target;
                    var texture = new Texture2D(size.x, size.y, TextureFormat.RGB24, false);
                    texture.ReadPixels(new Rect(0,0,size.x,size.y),0,0);
                    texture.Apply();
                    File.WriteAllBytes(Path.Combine(output, sceneName + "-" + size.x + "x" + size.y + ".png"), texture.EncodeToPNG());
                    RenderTexture.active = null;
                    camera.targetTexture = null;
                    UnityEngine.Object.DestroyImmediate(texture);
                    UnityEngine.Object.DestroyImmediate(target);
                }
                if (guide != null)
                {
                    var music = UnityEngine.Object.FindFirstObjectByType<MainMenuMusicPlayer>();
                    if (music == null || music.menuLoop == null || music.menuLoop.name != "Shop Music")
                        throw new Exception("HowToPlay Shop Music is not connected.");
                }
            }
            File.WriteAllText(Path.Combine(output, "result.txt"), "Layout failures: " + failures + "\nSix resolutions checked for each scene; Shop Music reference verified.");
            Debug.Log("MENU_VALIDATION_COMPLETE failures=" + failures);
        }
        finally
        {
            if (setup.Length > 0) EditorSceneManager.RestoreSceneManagerSetup(setup);
            else EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }
        if (Application.isBatchMode) EditorApplication.Exit(failures == 0 ? 0 : 1);
    }
}
#endif
