using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
public sealed class PhoneDisplayRedesign : MonoBehaviour
{
    static readonly Color Navy = new Color32(20,23,39,255);
    static readonly Color Panel = new Color32(31,35,54,255);
    static readonly Color TaxiGold = new Color32(255,205,75,255);
    static readonly Color CashGreen = new Color32(179,243,107,255);
    static readonly Color ShopTeal = new Color32(101,223,177,255);
    Vector2 lastViewport;

    void Start()
    {
        StylePhone();
        LayoutDopamine();
        FitRenderedView();
        RefreshViewport(new Vector2(Screen.width,Screen.height));
    }

    static void FitRenderedView()
    {
        var output=GameObject.Find("PostFXCanvas");if(output==null)return;
        foreach(Transform child in output.transform){
            var image=child.GetComponent<RawImage>();if(image==null||image.texture==null)continue;
            var fit=child.GetComponent<AspectRatioFitter>();if(fit==null)fit=child.gameObject.AddComponent<AspectRatioFitter>();
            fit.aspectMode=AspectRatioFitter.AspectMode.FitInParent;
            fit.aspectRatio=(float)image.texture.width/image.texture.height;
            image.raycastTarget=false;
        }
    }

    void Update(){var size=new Vector2(Screen.width,Screen.height);if(size!=lastViewport)RefreshViewport(size);}

    public void RefreshViewport(Vector2 size)
    {
        lastViewport=size;
        var hud=GameObject.Find("HUDCanvas");if(hud==null)return;
        var scaler=hud.GetComponent<CanvasScaler>();
        if(scaler!=null){scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=size.x<size.y?new Vector2(720,1280):new Vector2(1920,1080);scaler.screenMatchMode=CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;scaler.matchWidthOrHeight=0;}
        LayoutHudText(hud.transform.Find("TimerText") as RectTransform,new Vector2(1,1),new Vector2(-20,-22),new Vector2(130,48),32,TextAlignmentOptions.Right);
        LayoutHudText(hud.transform.Find("ZoneText") as RectTransform,new Vector2(1,1),new Vector2(-20,-85),new Vector2(320,60),22,TextAlignmentOptions.Right);
    }

    static void LayoutHudText(RectTransform rect,Vector2 anchor,Vector2 position,Vector2 size,float fontSize,TextAlignmentOptions alignment)
    {
        if(rect==null)return;rect.localScale=Vector3.one;rect.localRotation=Quaternion.identity;rect.anchorMin=rect.anchorMax=anchor;rect.pivot=anchor;rect.anchoredPosition=position;rect.sizeDelta=size;
        var text=rect.GetComponent<TMP_Text>();if(text==null)return;text.margin=Vector4.zero;text.fontSize=fontSize;text.fontSizeMin=fontSize*.75f;text.fontSizeMax=fontSize;text.enableAutoSizing=true;text.alignment=alignment;
    }

    void StylePhone()
    {
        var manager = GetComponent<PhoneAppManager>();
        if (manager == null) return;
        ColorPanel(manager.homePanel, Navy);
        ColorPanel(manager.uberPanel, Navy);
        ColorPanel(manager.shopPanel, new Color32(18,32,38,255));
        ColorPanel(manager.gamePanel, Navy);

        StyleButtons(manager.homePanel, new Color32(53,61,82,255));
        StyleButtons(manager.uberPanel, TaxiGold);

        if (manager.homePanel != null)
        {
            var home=manager.homePanel.GetComponent<PhoneHomeUI>();
            if(home!=null && home.taxiNotificationText!=null) {
                var label=home.taxiNotificationText;
                var rect=label.rectTransform;
                rect.anchorMin=new Vector2(.20f,.70f);rect.anchorMax=new Vector2(.80f,.93f);
                rect.offsetMin=rect.offsetMax=Vector2.zero;
                rect.localScale=Vector3.one;rect.localRotation=Quaternion.Euler(0,180,0);
                label.margin=Vector4.zero;label.fontSize=2.2f;
                label.enableAutoSizing=true;label.fontSizeMin=1.2f;label.fontSizeMax=2.2f;
                label.alignment=TextAlignmentOptions.Center;label.overflowMode=TextOverflowModes.Ellipsis;
                label.raycastTarget=false;
            }
            TintButton(manager.homePanel.transform.Find("TaxiButton"), TaxiGold);
            TintButton(manager.homePanel.transform.Find("GameButton"), new Color32(239,104,206,255));
            TintButton(manager.homePanel.transform.Find("ShopButton"), ShopTeal);
        }

        foreach (var slot in GetComponentsInChildren<TaxiOfferSlotUI>(true))
        {
            if(slot.titleText!=null)slot.titleText.color=new Color32(241,238,222,255);
            if (slot.portraitImage != null)
            {
                var outline = slot.portraitImage.GetComponent<Outline>(); if(outline==null) outline=slot.portraitImage.gameObject.AddComponent<Outline>();
                outline.effectColor = TaxiGold; outline.effectDistance = new Vector2(.12f,-.12f);
            }
            if (slot.acceptButton != null) TintButton(slot.acceptButton.transform, TaxiGold);
        }
        foreach (var slot in GetComponentsInChildren<ShopUpgradeSlot>(true))
        {
            if (slot.backgroundImage == null) continue;
            var outline = slot.backgroundImage.GetComponent<Outline>(); if(outline==null) outline=slot.backgroundImage.gameObject.AddComponent<Outline>();
            outline.effectColor = ShopTeal; outline.effectDistance = new Vector2(.12f,-.12f);
        }
    }

    void LayoutDopamine()
    {
        var hud = GameObject.Find("HUDCanvas");
        var dopamine = DopamineManager.Instance;
        if (hud == null || dopamine == null || dopamine.portraitImage == null || dopamine.barSlider == null) return;
        var root = hud.transform.Find("DopamineHUD") as RectTransform;
        if (root == null) return;
        root.localScale=Vector3.one;root.localRotation=Quaternion.identity;
        root.anchorMin = root.anchorMax = new Vector2(.5f,1f);
        root.pivot = new Vector2(.5f,1f);
        root.anchoredPosition = new Vector2(0,-22f);
        root.sizeDelta = new Vector2(400,58);
        var background = root.GetComponent<Image>();
        if (background != null)
        {
            background.sprite = null; background.color = new Color32(20,23,39,220);
            var outline = background.GetComponent<Outline>(); if(outline==null) outline=background.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color32(82,92,119,255); outline.effectDistance = new Vector2(2,-2);
        }

        var face = dopamine.portraitImage.rectTransform;
        face.SetParent(root,false);face.localScale=Vector3.one;face.localRotation=Quaternion.identity; face.anchorMin=face.anchorMax=new Vector2(.5f,.5f);
        face.pivot=new Vector2(.5f,.5f);face.anchoredPosition=new Vector2(-164,0);face.sizeDelta=new Vector2(40,40);
        face.GetComponent<Image>().preserveAspect=true;

        var bar=dopamine.barSlider.GetComponent<RectTransform>();
        bar.SetParent(root,false);bar.localScale=Vector3.one;bar.localRotation=Quaternion.identity;bar.anchorMin=bar.anchorMax=new Vector2(.5f,.5f);
        bar.pivot=new Vector2(.5f,.5f);bar.anchoredPosition=new Vector2(-20,0);bar.sizeDelta=new Vector2(230,22);
        dopamine.barSlider.interactable=false;
        Stretch(bar.Find("Background") as RectTransform,0);
        var fill=dopamine.barSlider.fillRect;
        if(fill!=null){Stretch(fill.parent as RectTransform,3);fill.localScale=Vector3.one;fill.localRotation=Quaternion.identity;fill.offsetMin=fill.offsetMax=Vector2.zero;}
        var percent=dopamine.percentText;
        if(percent!=null){var p=percent.rectTransform;p.SetParent(root,false);p.localScale=Vector3.one;p.localRotation=Quaternion.identity;p.anchorMin=p.anchorMax=new Vector2(1,.5f);p.pivot=new Vector2(1,.5f);p.anchoredPosition=new Vector2(-9,0);p.sizeDelta=new Vector2(64,36);percent.margin=Vector4.zero;percent.fontSize=36;percent.textWrappingMode=TextWrappingModes.NoWrap;percent.alignment=TextAlignmentOptions.Right;percent.color=CashGreen;percent.enableAutoSizing=true;percent.fontSizeMin=24;percent.fontSizeMax=36;}
    }

    static void Stretch(RectTransform rect,float padding)
    {
        if(rect==null)return;
        rect.localScale=Vector3.one;rect.localRotation=Quaternion.identity;
        rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;
        rect.offsetMin=new Vector2(padding,padding);rect.offsetMax=-rect.offsetMin;
    }

    static void ColorPanel(GameObject panel, Color color)
    {
        if(panel==null)return;
        var image=panel.GetComponent<Image>();if(image==null)return;
        image.color=color;
        var outline=image.GetComponent<Outline>();if(outline==null)outline=image.gameObject.AddComponent<Outline>();
        outline.effectColor=new Color32(71,80,106,255);outline.effectDistance=new Vector2(.12f,-.12f);
    }
    static void StyleButtons(GameObject panel, Color accent)
    {if(panel==null)return;foreach(var b in panel.GetComponentsInChildren<Button>(true))TintButton(b.transform,accent);}
    static void TintButton(Transform t,Color accent)
    {
        if(t==null)return;var button=t.GetComponent<Button>();var image=button!=null?button.targetGraphic as Image:t.GetComponent<Image>();
        if(image==null)return;image.color=Color.white;
        if(button!=null){var c=button.colors;c.normalColor=accent;c.highlightedColor=Color.Lerp(accent,Color.white,.2f);c.pressedColor=Color.Lerp(accent,Color.black,.2f);c.selectedColor=c.highlightedColor;c.disabledColor=new Color32(82,88,104,220);c.colorMultiplier=1;c.fadeDuration=.08f;button.colors=c;}
        var outline=image.GetComponent<Outline>();if(outline==null)outline=image.gameObject.AddComponent<Outline>();outline.effectColor=new Color32(8,10,17,255);outline.effectDistance=new Vector2(.12f,-.12f);
    }
}
