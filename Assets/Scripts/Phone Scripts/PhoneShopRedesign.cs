using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class PhoneShopRedesign : MonoBehaviour
{
    static readonly Color Navy = new Color32(15,24,34,255);
    static readonly Color Card = new Color32(29,43,53,255);
    static readonly Color Teal = new Color32(101,223,177,255);
    static readonly Color Gold = new Color32(237,195,84,255);
    RectTransform screen;
    TMP_FontAsset font;
    CardView[] cards;
    TMP_Text cash;
    float refreshAt;

    sealed class CardView { public ShopUpgradeSlot slot; public TMP_Text name, price, description; public Image icon; public Button button; public Image buttonImage; }

    void Awake() { Build(); }
    void OnEnable() { Build(); Refresh(); }
    void Update()
    {
        if (!gameObject.activeInHierarchy) return;
        if (Time.unscaledTime >= refreshAt) { refreshAt = Time.unscaledTime + .35f; Refresh(); }
    }
    void Build()
    {
        if (screen != null) return;
        font = GetComponentInChildren<TMP_Text>(true)?.font ?? TMP_Settings.defaultFontAsset;
        // Keep the stock slot scripts running, but replace their old presentation.
        foreach(Transform child in transform) {
            var group=child.GetComponent<CanvasGroup>();
            if(group==null) group=child.gameObject.AddComponent<CanvasGroup>();
            group.alpha=0;group.interactable=false;group.blocksRaycasts=false;
        }
        var root = new GameObject("Shop Inventory Redesign", typeof(RectTransform), typeof(Image));
        root.transform.SetParent(transform,false); screen=(RectTransform)root.transform;
        screen.anchorMin=screen.anchorMax=new Vector2(.5f,.5f);screen.sizeDelta=new Vector2(19.2f,29.2f);screen.anchoredPosition=Vector2.zero;
        // The mounted phone is viewed from the canvas's +Z side, like its legacy app text.
        screen.localRotation=Quaternion.Euler(0,180,0);

        screen.SetAsLastSibling();root.GetComponent<Image>().color=Navy;
        var outline=root.AddComponent<Outline>();outline.effectColor=new Color32(87,115,112,255);outline.effectDistance=new Vector2(.12f,-.12f);
        Text(screen,"ODD GOODS",new Vector2(.06f,.87f),new Vector2(.94f,.99f),3.4f,Gold,TextAlignmentOptions.Center);
        cash=Text(screen,"CASH  $0",new Vector2(.08f,.80f),new Vector2(.92f,.88f),2f,Teal,TextAlignmentOptions.Center);
        Text(screen,"UPGRADES FOR YOUR NEXT FARE",new Vector2(.04f,.74f),new Vector2(.96f,.81f),1.25f,Color.white,TextAlignmentOptions.Center);

        var slots=GetComponentsInChildren<ShopUpgradeSlot>(true).OrderBy(s=>s.name,StringComparer.Ordinal).ToArray();
        cards=new CardView[slots.Length];
        for(int i=0;i<slots.Length;i++)
        {
            int col=i%2,row=i/2;float left=.055f+col*.46f,top=.70f-row*.285f;
            var card=MakePanel("Upgrade Card "+(i+1),screen,new Vector2(left,top-.255f),new Vector2(left+.425f,top),Card,Teal);
            var view=new CardView{slot=slots[i]};
            view.icon=MakeImage("Icon",card,new Vector2(.04f,.50f),new Vector2(.32f,.71f),Color.white);
            view.icon.preserveAspect=true;view.icon.raycastTarget=false;
            view.name=Text(card,"UPGRADE",new Vector2(.04f,.72f),new Vector2(.96f,.98f),1.5f,Color.white,TextAlignmentOptions.Center);
            view.price=Text(card,"$0",new Vector2(.34f,.49f),new Vector2(.97f,.71f),1.8f,Teal,TextAlignmentOptions.Center);
            view.description=Text(card,"",new Vector2(.045f,.22f),new Vector2(.955f,.49f),1.1f,new Color32(205,211,221,255),TextAlignmentOptions.Center);
            var buttonRoot=new GameObject("BUY",typeof(RectTransform),typeof(Image),typeof(Button));buttonRoot.transform.SetParent(card,false);
            var br=(RectTransform)buttonRoot.transform;br.anchorMin=new Vector2(.19f,.025f);br.anchorMax=new Vector2(.81f,.205f);br.offsetMin=br.offsetMax=Vector2.zero;
            view.buttonImage=buttonRoot.GetComponent<Image>();view.buttonImage.color=Teal;
            view.button=buttonRoot.GetComponent<Button>();view.button.targetGraphic=view.buttonImage;
            view.button.onClick.AddListener(()=>{if(view.slot!=null){view.slot.BuyUpgrade();ShopHoverUI.Instance?.HideFor(view.slot);}Refresh();});
            Text(br,"BUY",Vector2.zero,Vector2.one,1.35f,Navy,TextAlignmentOptions.Center);
            cards[i]=view;
        }
        var backRoot=new GameObject("PHONE HOME",typeof(RectTransform),typeof(Image),typeof(Button));backRoot.transform.SetParent(screen,false);
        var back=(RectTransform)backRoot.transform;back.anchorMin=new Vector2(.28f,.025f);back.anchorMax=new Vector2(.72f,.105f);back.offsetMin=back.offsetMax=Vector2.zero;
        backRoot.GetComponent<Image>().color=new Color32(45,58,70,255);
        backRoot.GetComponent<Button>().onClick.AddListener(()=>PhoneAppManager.Instance?.OpenHome());
        Text(back,"PHONE HOME",Vector2.zero,Vector2.one,1.35f,Color.white,TextAlignmentOptions.Center);
        foreach(var child in screen.GetComponentsInChildren<Transform>(true)) child.gameObject.layer=gameObject.layer;
        screen.gameObject.SetActive(false);
    }
    RectTransform MakePanel(string name,RectTransform parent,Vector2 min,Vector2 max,Color color,Color edge)
    {
        var go=new GameObject(name,typeof(RectTransform),typeof(Image),typeof(Outline));go.transform.SetParent(parent,false);
        var r=(RectTransform)go.transform;r.anchorMin=min;r.anchorMax=max;r.offsetMin=r.offsetMax=Vector2.zero;
        go.GetComponent<Image>().color=color;var o=go.GetComponent<Outline>();o.effectColor=edge;o.effectDistance=new Vector2(.12f,-.12f);return r;
    }
    Image MakeImage(string name,RectTransform parent,Vector2 min,Vector2 max,Color color)
    {
        var go=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));go.transform.SetParent(parent,false);
        var r=(RectTransform)go.transform;r.anchorMin=min;r.anchorMax=max;r.offsetMin=r.offsetMax=Vector2.zero;var image=go.GetComponent<Image>();image.color=color;return image;
    }
    TMP_Text Text(RectTransform parent,string value,Vector2 min,Vector2 max,float size,Color color,TextAlignmentOptions align)
    {
        var go=new GameObject(value,typeof(RectTransform),typeof(TextMeshProUGUI));go.transform.SetParent(parent,false);
        var r=(RectTransform)go.transform;r.anchorMin=min;r.anchorMax=max;r.offsetMin=r.offsetMax=Vector2.zero;
        var text=go.GetComponent<TMP_Text>();text.font=font;text.text=value;text.fontSize=size;text.enableAutoSizing=true;text.fontSizeMin=size*.65f;text.fontSizeMax=size;text.overflowMode=TextOverflowModes.Ellipsis;text.color=color;text.alignment=align;text.textWrappingMode=TextWrappingModes.Normal;text.raycastTarget=false;return text;
    }
    void Refresh()
    {
        if(screen==null||cards==null)return;
        if(cash!=null)cash.text="CASH  $"+(MoneyManager.Instance!=null?MoneyManager.Instance.currentCash:0);
        foreach(var view in cards)
        {
            var upgrade=view.slot!=null?view.slot.currentUpgrade:null;
            if(upgrade==null){view.name.text="SOLD OUT";view.price.text="";view.description.text="No stock today.";view.icon.enabled=false;view.button.interactable=false;continue;}
            int amount=MoneyManager.Instance!=null?MoneyManager.Instance.GetScaledUpgradePrice(upgrade.basePrice):upgrade.basePrice;
            string name=upgrade.upgradeName.ToUpperInvariant();if(name.Length>19)name=name.Substring(0,17)+"…";
            string description=(upgrade.description??"").Replace('\n',' ').Trim();if(description.Length>48)description=description.Substring(0,45).TrimEnd()+"…";
            if(upgrade.upgradeType==ShopUpgradeType.DriftKit) description="+30% boost, +25% time; -8% speed";
            if(upgrade.upgradeType==ShopUpgradeType.DopamineRig) description="+35% gain; +20% decay, -8% fares";
            if(upgrade.upgradeType==ShopUpgradeType.PremiumMeter) description="+35% fares; -8% steer, -15% gain";
            view.name.text=name;view.price.text="$"+amount;view.description.text=description;
            view.icon.sprite=upgrade.icon;view.icon.enabled=upgrade.icon!=null;
            string reason="";bool allowed=ShopManager.Instance==null||ShopManager.Instance.CanPurchase(upgrade,out reason);
            bool owned=view.slot.IsPurchased || (!upgrade.canBuyMultiple && ShopManager.Instance!=null && ShopManager.Instance.purchasedUpgrades.Contains(upgrade));
            view.button.interactable=allowed&&!owned&&SaveJSONData.Ready;
            view.buttonImage.color=owned?new Color32(76,121,70,255):allowed?Teal:new Color32(101,72,73,255);
            var label=view.button.GetComponentInChildren<TMP_Text>();if(label!=null){label.text=owned?"OWNED":allowed?"BUY":"NEED MORE";label.color=owned?new Color32(211,245,179,255):allowed?Navy:Color.white;}
        }
    }
    public void Show(){Build();screen.gameObject.SetActive(true);screen.SetAsLastSibling();Refresh();}
    public void Hide(){if(screen!=null)screen.gameObject.SetActive(false);}
}
