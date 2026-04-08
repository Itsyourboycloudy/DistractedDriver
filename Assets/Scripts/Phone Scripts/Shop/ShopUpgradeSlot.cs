using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShopUpgradeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI")]
    public RawImage backgroundImage;
    public Image iconImage;
    public TMP_Text nameText;
    public Button upgradeButton;
    public GameObject boughtOverlay;

    [Header("Optional Overlay Text")]
    public TMP_Text overlayText; // put the text inside your overlay here if you have one

    [Header("Rarity Backgrounds")]
    public Texture commonBackground;
    public Texture uncommonBackground;
    public Texture rareBackground;
    public Texture epicBackground;
    public Texture legendaryBackground;

    [Header("Current Upgrade")]
    public ShopUpgradeData currentUpgrade;

    private string cachedDescription = "";
    private bool purchased = false;

    void Awake()
    {
        CacheDescription();

        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveListener(BuyUpgrade);
            upgradeButton.onClick.AddListener(BuyUpgrade);
        }
    }

    void Update()
    {
        // keeps duplicate slots updated after one copy gets bought
        RefreshSlotState();
    }

    public void SetUpgrade(ShopUpgradeData upgrade)
    {
        currentUpgrade = upgrade;
        purchased = false;

        if (currentUpgrade == null)
        {
            ClearSlot();
            return;
        }

        if (iconImage != null)
            iconImage.sprite = currentUpgrade.icon;

        if (nameText != null)
            nameText.text = currentUpgrade.upgradeName;

        if (backgroundImage != null)
            backgroundImage.texture = GetBackgroundForRarity(currentUpgrade.rarity);

        CacheDescription();
        RefreshSlotState();
    }

    public void ClearSlot()
    {
        currentUpgrade = null;
        purchased = false;
        cachedDescription = "";

        if (iconImage != null)
            iconImage.sprite = null;

        if (nameText != null)
            nameText.text = "";

        if (backgroundImage != null)
            backgroundImage.texture = null;

        if (upgradeButton != null)
            upgradeButton.interactable = false;

        if (boughtOverlay != null)
            boughtOverlay.SetActive(false);

        if (overlayText != null)
            overlayText.text = "";
    }

    void CacheDescription()
    {
        if (currentUpgrade != null && !string.IsNullOrWhiteSpace(currentUpgrade.description))
            cachedDescription = currentUpgrade.description;
        else
            cachedDescription = "";
    }

    Texture GetBackgroundForRarity(UpgradeRarity rarity)
    {
        switch (rarity)
        {
            case UpgradeRarity.Common: return commonBackground;
            case UpgradeRarity.Uncommon: return uncommonBackground;
            case UpgradeRarity.Rare: return rareBackground;
            case UpgradeRarity.Epic: return epicBackground;
            case UpgradeRarity.Legendary: return legendaryBackground;
        }

        return commonBackground;
    }

    bool IsOwnedInShopManager()
    {
        return currentUpgrade != null &&
               ShopManager.Instance != null &&
               !currentUpgrade.canBuyMultiple &&
               ShopManager.Instance.purchasedUpgrades.Contains(currentUpgrade);
    }

    bool IsDuplicateBlocked()
    {
        // another copy is owned, but this exact slot was not the one purchased
        return !purchased && IsOwnedInShopManager();
    }

    void RefreshSlotState()
    {
        if (currentUpgrade == null) return;

        bool duplicateBlocked = IsDuplicateBlocked();

        if (upgradeButton != null)
            upgradeButton.interactable = !purchased && !duplicateBlocked;

        if (boughtOverlay != null)
            boughtOverlay.SetActive(purchased || duplicateBlocked);

        if (overlayText != null)
        {
            if (purchased)
                overlayText.text = "OWNED";
            else if (duplicateBlocked)
                overlayText.text = "NO DUPLICATES";
            else
                overlayText.text = "";
        }
    }

    void ShowHover()
    {
        if (ShopHoverUI.Instance == null) return;
        if (currentUpgrade == null) return;

        bool alreadyOwned = IsOwnedInShopManager();
        bool thisSlotPurchased = purchased;
        bool duplicateBlocked = IsDuplicateBlocked();

        int scaledPrice = currentUpgrade.basePrice;
        if (MoneyManager.Instance != null)
            scaledPrice = MoneyManager.Instance.GetScaledUpgradePrice(currentUpgrade.basePrice);

        bool canAfford = MoneyManager.Instance != null && MoneyManager.Instance.currentCash >= scaledPrice;

        string costLine = "";

        if (thisSlotPurchased)
        {
            costLine = "<color=#9A9A9A>OWNED</color>";
        }
        else if (duplicateBlocked)
        {
            costLine = "<color=#9A9A9A>NO DUPLICATES</color>";
        }
        else
        {
            string labelColor = "#FFD54A";
            string valueColor = canAfford ? "#00FF66" : "#FF3B30";

            costLine = "<color=" + labelColor + ">COST:</color> " +
                       "<color=" + valueColor + ">$" + scaledPrice + "</color>";

            if (!currentUpgrade.canBuyMultiple && !alreadyOwned)
                costLine += "   <color=#9A9A9A>NO DUPLICATES</color>";
        }

        ShopHoverUI.Instance.Show(cachedDescription, costLine);
    }

    void HideHover()
    {
        if (ShopHoverUI.Instance == null) return;
        ShopHoverUI.Instance.Hide();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideHover();
    }

    public void HoverEnter()
    {
        ShowHover();
    }

    public void HoverExit()
    {
        HideHover();
    }

    public void BuyUpgrade()
    {
        if (currentUpgrade == null || purchased) return;
        if (IsDuplicateBlocked()) return;

        if (ShopManager.Instance != null)
        {
            bool bought = ShopManager.Instance.PurchaseUpgrade(currentUpgrade);
            if (!bought) return;
        }

        purchased = true;
        RefreshSlotState();
        ShowHover();
    }
}