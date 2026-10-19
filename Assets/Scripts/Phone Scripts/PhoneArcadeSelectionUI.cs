using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class PhoneArcadeSelectionUI : MonoBehaviour
{
    static readonly Color Navy = new Color32(20, 23, 39, 255);
    static readonly Color Card = new Color32(37, 39, 61, 255);
    static readonly Color Cyan = new Color32(74, 222, 235, 255);
    static readonly Color Pink = new Color32(239, 104, 206, 255);
    GameObject screen;
    PhoneMinigameSelector selector;
    TMP_FontAsset font;

    void Awake()
    {
        selector = GetComponent<PhoneMinigameSelector>();
        font = GetComponentInChildren<TMP_Text>(true)?.font ?? TMP_Settings.defaultFontAsset;
        Build();
    }

    void Build()
    {
        if (screen != null) return;
        var root = new GameObject("Arcade Selection", typeof(RectTransform), typeof(Image));
        root.transform.SetParent(transform, false);
        screen = root;
        var r = (RectTransform)root.transform;
        r.anchorMin = r.anchorMax = new Vector2(.5f,.5f);
        r.sizeDelta = new Vector2(19.2f,29.2f);
        // Match the legacy apps, which face the camera on the canvas's +Z side.
        r.localRotation=Quaternion.Euler(0,180,0);
        var hostScale=transform.localScale;
        r.localScale = new Vector3(1/hostScale.x,1/hostScale.y,1/hostScale.z);
        var homePanel=PhoneAppManager.Instance!=null?PhoneAppManager.Instance.homePanel:null;
        if(homePanel!=null) r.localPosition=transform.InverseTransformPoint(homePanel.transform.position);

        var bg = root.GetComponent<Image>(); bg.color = Navy;

        AddText(r, "POCKET ARCADE", new Vector2(.06f,.84f), new Vector2(.94f,.98f), 3.6f, Color.white, TextAlignmentOptions.Center);
        AddText(r, "PICK A CABINET  •  BUILD DOPAMINE", new Vector2(.05f,.77f), new Vector2(.95f,.85f), 1.7f, Cyan, TextAlignmentOptions.Center);

        var playable = selector != null ? selector.minigames : new List<PhoneMinigameSelector.MinigameEntry>();
        int visible = 0;
        for (int i = 0; i < playable.Count; i++)
        {
            var entry = playable[i];
            if (!selector.IsPlayable(entry)) continue;
            float top = .73f - visible * .245f;
            float bottom = top - .205f;
            var tint = entry.gameType == PhoneMinigameType.FlyFlappy ? Pink : Cyan;
            var card = new GameObject("Arcade Card - " + entry.gameType, typeof(RectTransform), typeof(Image), typeof(Outline));
            card.transform.SetParent(r, false);
            var cr = (RectTransform)card.transform;
            cr.anchorMin = new Vector2(.06f,bottom); cr.anchorMax = new Vector2(.94f,top);
            cr.offsetMin = cr.offsetMax = Vector2.zero;
            card.GetComponent<Image>().color = Card;
            var outline = card.GetComponent<Outline>(); outline.effectColor = tint; outline.effectDistance = new Vector2(.12f, -.12f);
            string gameTitle = entry.gameType == PhoneMinigameType.FlyFlappy ? "FLY AWAY" : "WHACK-A-FLY";
            string description = entry.gameType == PhoneMinigameType.FlyFlappy
                ? "DODGE THE SWATTERS  •  BUILD YOUR STREAK"
                : "6 PIPES  •  3 LIVES  •  45 SECONDS";
            AddText(cr, gameTitle, new Vector2(.05f,.58f), new Vector2(.95f,.96f), 2.8f, tint, TextAlignmentOptions.Center);
            AddText(cr, description, new Vector2(.04f,.32f), new Vector2(.96f,.62f), 1.5f, Color.white, TextAlignmentOptions.Center);
            var go = new GameObject("PLAY", typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(cr, false);
            var br = (RectTransform)go.transform; br.anchorMin = new Vector2(.17f,.035f); br.anchorMax = new Vector2(.83f,.29f); br.offsetMin = br.offsetMax = Vector2.zero;
            go.GetComponent<Image>().color = tint;
            var button = go.GetComponent<Button>(); button.targetGraphic = go.GetComponent<Image>();
            int gameIndex = i; button.onClick.AddListener(() => selector.OpenGameAt(gameIndex));
            AddText(br, "PLAY", Vector2.zero, Vector2.one, 2.2f, Navy, TextAlignmentOptions.Center);
            visible++;
        }
        if (visible == 0)
            AddText(r, "NO GAMES READY", new Vector2(.1f,.42f), new Vector2(.9f,.58f), 2.6f, Color.white, TextAlignmentOptions.Center);

        var home = new GameObject("ARCADE HOME", typeof(RectTransform), typeof(Image), typeof(Button));
        home.transform.SetParent(r, false);
        var hr=(RectTransform)home.transform; hr.anchorMin=new Vector2(.2f,.035f);hr.anchorMax=new Vector2(.8f,.14f);hr.offsetMin=hr.offsetMax=Vector2.zero;
        home.GetComponent<Image>().color = new Color32(46,54,77,255);
        home.GetComponent<Button>().onClick.AddListener(() => PhoneAppManager.Instance?.OpenHome());
        AddText(hr,"PHONE HOME",Vector2.zero,Vector2.one,2f,Color.white,TextAlignmentOptions.Center);
        foreach(var child in screen.GetComponentsInChildren<Transform>(true)) child.gameObject.layer=gameObject.layer;
        screen.SetActive(false);
    }

    TMP_Text AddText(RectTransform parent,string value,Vector2 min,Vector2 max,float size,Color color,TextAlignmentOptions alignment)
    {
        var go=new GameObject(value,typeof(RectTransform),typeof(TextMeshProUGUI));go.transform.SetParent(parent,false);
        var r=(RectTransform)go.transform;r.anchorMin=min;r.anchorMax=max;r.offsetMin=r.offsetMax=Vector2.zero;
        var t=go.GetComponent<TMP_Text>();t.font=font;t.text=value;t.fontSize=size;t.enableAutoSizing=true;t.fontSizeMin=size*.65f;t.fontSizeMax=size;t.overflowMode=TextOverflowModes.Ellipsis;t.color=color;t.alignment=alignment;t.textWrappingMode=TextWrappingModes.Normal;t.raycastTarget=false;
        return t;
    }
    public void Show(){Build();screen.SetActive(true);screen.transform.SetAsLastSibling();}
    public void Hide(){if(screen!=null)screen.SetActive(false);}
}
